using MassTransit;
using MassTransit.Courier;
using System;
using System.Threading.Tasks;
using static System.Console;

namespace Registration.Activities.EventRegistration
{
    public class EventRegistrationActivity : Activity<IEventRegistrationArguments, IEventRegistrationLog>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<IEventRegistrationArguments> context)
        {
            WriteLine("Registering for event: {0} ({1})", context.Arguments.EventId, context.Arguments.ParticipantEmailAddress);

            var registrationTotal = 25.00m;

            var registrationId = NewId.NextGuid();

            WriteLine("Registered for event: {0} ({1})", registrationId, context.Arguments.ParticipantEmailAddress);

            return context.CompletedWithVariables(new Log(registrationId, context.Arguments.ParticipantEmailAddress), new
            {
                Amount = registrationTotal
            });
        }

        public async Task<CompensationResult> Compensate(CompensateContext<IEventRegistrationLog> context)
        {
            WriteLine("Removing registration for event: {0} ({1})", context.Log.RegistrationId, context.Log.ParticipantEmailAddress);

            return context.Compensated();
        }
    }

    public class Log : IEventRegistrationLog
    {
        public Guid RegistrationId { get; }
        public string ParticipantEmailAddress { get; }

        public Log(Guid registrationId, string participantEmailAddress)
        {
            RegistrationId = registrationId;
            ParticipantEmailAddress = participantEmailAddress;
        }

    }
}