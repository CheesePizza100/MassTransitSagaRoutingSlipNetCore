using GreenPipes;
using GreenPipes.Partitioning;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.EntityFrameworkCoreIntegration.Saga;
using MassTransit.RabbitMqTransport;
using MassTransit.Saga;
using Microsoft.EntityFrameworkCore;
using Registration.Contracts;
using Registration.Data;
using RegistrationState;
using System;
using static System.Console;

namespace StateService
{
    class Program
    {
        static void Main(string[] args)
        {
            Title = "State Service";
            WriteLine("State Service starting...");
            Run();
        }

        private static void Run()
        {
            ISagaRepository<RegistrationStateInstance> sagaRepository = new EntityFrameworkSagaRepository<RegistrationStateInstance>(SagaDbContextFactory);

            IBusControl busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                IRabbitMqHost host = cfg.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                EndpointConvention.Map<IProcessRegistration>(new Uri("rabbitmq://localhost/registration.state.queue"));

                cfg.ReceiveEndpoint(host, "registration.state.queue", endpointConfigurator =>
                {
                    endpointConfigurator.PrefetchCount = 16;

                    IPartitioner partitioner = cfg.CreatePartitioner(8);

                    RegistrationStateMachine machine = new RegistrationStateMachine();

                    endpointConfigurator.StateMachineSaga(machine, sagaRepository, sagaConfigurator =>
                    {
                        sagaConfigurator.Message<IRegistrationReceived>(m => m.UsePartitioner(partitioner, p => p.Message.SubmissionId));
                        sagaConfigurator.Message<IRegistrationCompleted>(m => m.UsePartitioner(partitioner, p => p.Message.SubmissionId));
                    });
                });
            });

            busControl.StartAsync();
            ReadKey();

            busControl.StopAsync();

            DbContext SagaDbContextFactory()
            {
                DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
                optionsBuilder.UseSqlServer(@"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=RegistrationDemo;Integrated Security=True");
                SagaDbContext<RegistrationStateInstance, RegistrationStateInstanceMap> context = new SagaDbContext<RegistrationStateInstance, RegistrationStateInstanceMap>(optionsBuilder.Options);
                context.Database.EnsureCreated();
                return context;
            }
        }
    }
}