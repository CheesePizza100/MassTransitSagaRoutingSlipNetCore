using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Registration.Data.Models;
using RegistrationState;
using System;
using System.Threading.Tasks;

namespace Registration.Data
{
    public class RegistrationStateReader : IRegistrationStateReader
    {
        private readonly SagaDbContext<RegistrationStateInstance, RegistrationStateInstanceMap> _sagaDbContext;

        public RegistrationStateReader(string connectionString)
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlServer(@"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=RegistrationDemo;Integrated Security=True");
            _sagaDbContext = new SagaDbContext<RegistrationStateInstance, RegistrationStateInstanceMap>(optionsBuilder.Options);
        }

        public async Task<RegistrationModel> Get(Guid submissionId)
        {
            RegistrationStateInstance instance = await _sagaDbContext.Set<RegistrationStateInstance>().SingleAsync(x => x.CorrelationId == submissionId);

            return new RegistrationModel
            {
                SubmissionId = instance.CorrelationId,
                ParticipantEmailAddress = instance.ParticipantEmailAddress,
                ParticipantCategory = instance.ParticipantCategory,
                ParticipantLicenseNumber = instance.ParticipantLicenseNumber,
                EventId = instance.EventId,
                RaceId = instance.RaceId,
                Status = instance.CurrentState
            };
        }
    }
}