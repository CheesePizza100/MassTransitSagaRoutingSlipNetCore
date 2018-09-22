using MassTransit;
using MassTransit.Courier;
using MassTransit.RabbitMqTransport;
using Registration.Activities.EventRegistration;
using Registration.Activities.LicenseVerification;
using Registration.Activities.ProcessPayment;
using System;
using static System.Console;

namespace RegistrationActivityService
{
    class Program
    {
        static void Main(string[] args)
        {
            Title = "Registration Activity Service";
            WriteLine("Registration Activity Service starting...");
            Run();
        }

        private static void Run()
        {
            IBusControl busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>

            {

                IRabbitMqHost host = cfg.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                ConfigureExecuteActivity<LicenseVerificationActivity, ILicenseVerificationArguments>(cfg, host);
                ConfigureActivity<EventRegistrationActivity, IEventRegistrationArguments, IEventRegistrationLog>(cfg, host);
                ConfigureActivity<ProcessPaymentActivity, IProcessPaymentArguments, IProcessPaymentLog>(cfg, host);

            });

            busControl.StartAsync();
            ReadKey();

            busControl.StopAsync();
        }

        private static void ConfigureExecuteActivity<TActivity, TArguments>(IRabbitMqBusFactoryConfigurator cfg, IRabbitMqHost host)
            where TActivity : class, ExecuteActivity<TArguments>, new()
            where TArguments : class
        {
            cfg.ReceiveEndpoint(host, GetExecuteActivityQueueName(typeof(TActivity)), endPointConfigurator =>
            {
                endPointConfigurator.PrefetchCount = 16;
                endPointConfigurator.ExecuteActivityHost<TActivity, TArguments>();
            });
        }

        private static void ConfigureActivity<TActivity, TArguments, TLog>(IRabbitMqBusFactoryConfigurator cfg, IRabbitMqHost host)
             where TActivity : class, Activity<TArguments, TLog>, new()
             where TArguments : class
             where TLog : class
        {
            Uri compensateAddress = null;

            cfg.ReceiveEndpoint(host, GetCompensateActivityQueueName(typeof(TActivity)), endPointConfigurator =>
            {
                endPointConfigurator.PrefetchCount = 16;
                endPointConfigurator.CompensateActivityHost<TActivity, TLog>();
                compensateAddress = endPointConfigurator.InputAddress;

            });

            cfg.ReceiveEndpoint(host, GetExecuteActivityQueueName(typeof(TActivity)), endpointConfigurator =>
            {
                endpointConfigurator.PrefetchCount = 16;
                endpointConfigurator.ExecuteActivityHost<TActivity, TArguments>(compensateAddress);
            });
        }

        private static string GetExecuteActivityQueueName(Type activityType)
        {
            string queueName = $"execute-{activityType.Name.Replace("Activity", "").ToLowerInvariant()}";
            WriteLine(queueName);
            return queueName;
        }



        private static string GetCompensateActivityQueueName(Type activityType)
        {
            string queueName = $"compensate-{activityType.Name.Replace("Activity", "").ToLowerInvariant()}";
            WriteLine(queueName);
            return queueName;
        }
    }
}