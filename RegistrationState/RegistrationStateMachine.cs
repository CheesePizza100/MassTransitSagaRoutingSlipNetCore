using Automatonymous;
using GreenPipes;
using MassTransit;
using MassTransit.Util;
using Registration.Contracts;
using System;
using System.Threading.Tasks;
using static System.Console;

namespace RegistrationState
{
    public class RegistrationStateMachine : MassTransitStateMachine<RegistrationStateInstance>
    {
        public State Received { get; private set; }
        public State Registered { get; private set; }
        public State Suspended { get; private set; }

        public Event<IRegistrationReceived> EventRegistrationReceived { get; private set; }
        public Event<IRegistrationCompleted> EventRegistrationCompleted { get; private set; }
        public Event<IRegistrationLicenseVerificationFailed> LicenseVerificationFailed { get; private set; }
        public Event<IRegistrationPaymentFailed> PaymentFailed { get; private set; }

        public RegistrationStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => EventRegistrationReceived, x =>
            {
                x.CorrelateById(m => m.Message.SubmissionId);
                x.SelectId(m => m.Message.SubmissionId);
            });

            Event(() => EventRegistrationCompleted, x =>
            {
                x.CorrelateById(m => m.Message.SubmissionId);
            });

            Event(() => LicenseVerificationFailed, x =>
            {
                x.CorrelateById(m => m.Message.SubmissionId);
            });

            Event(() => PaymentFailed, x =>
            {
                x.CorrelateById(m => m.Message.SubmissionId);
            });

            Initially(
                When(EventRegistrationReceived)
                    .Then(Initialize)
                    .ThenAsync(InitiateProcessing)
                    .TransitionTo(Received));

            During(Received,
                When(EventRegistrationCompleted)
                    .Then(Register)
                    .TransitionTo(Registered),
                When(LicenseVerificationFailed)
                    .Then(InvalidLicense)
                    .TransitionTo(Suspended),
                When(PaymentFailed)
                    .Then(PaymentFailure)
                    .TransitionTo(Suspended));

            During(Suspended,
                When(EventRegistrationReceived)
                    .Then(Initialize)
                    .ThenAsync(InitiateProcessing)
                    .TransitionTo(Received));
        }

        private void Initialize(BehaviorContext<RegistrationStateInstance, IRegistrationReceived> context) => InitializeInstance(context.Instance, context.Data);

        private static void InitializeInstance(RegistrationStateInstance instance, IRegistrationReceived data)
        {
            WriteLine("Initializing: {0} ({1})", data.SubmissionId, data.ParticipantEmailAddress);

            instance.ParticipantEmailAddress = data.ParticipantEmailAddress;
            instance.ParticipantLicenseNumber = data.ParticipantLicenseNumber;
            instance.ParticipantCategory = data.ParticipantCategory;

            instance.EventId = data.EventId;
            instance.RaceId = data.RaceId;
        }

        private async Task InitiateProcessing(BehaviorContext<RegistrationStateInstance, IRegistrationReceived> context)
        {
            var registration = CreateProcessRegistration(context.Data);

            //Uri destinationAddress;
            if (!EndpointConvention.TryGetDestinationAddress<IProcessRegistration>(out Uri destinationAddress))
            {
                throw new ConfigurationException($"The endpoint convention was not configured: {TypeMetadataCache<IProcessRegistration>.ShortName}");
            }

            await context.GetPayload<ConsumeContext>().Send(destinationAddress, registration);

            WriteLine("Processing: {0} ({1})", context.Data.SubmissionId, context.Data.ParticipantEmailAddress);
        }

        private static IProcessRegistration CreateProcessRegistration(IRegistrationReceived message) => new Process(message.SubmissionId, message.ParticipantEmailAddress, message.ParticipantLicenseNumber, message.ParticipantCategory,
                message.EventId, message.RaceId, message.CardNumber);

        private void Register(BehaviorContext<RegistrationStateInstance, IRegistrationCompleted> context)
        {
            Write("Look here dickhead...Registered: {0} ({1})", context.Data.SubmissionId, context.Instance.ParticipantEmailAddress);
        }

        private void InvalidLicense(BehaviorContext<RegistrationStateInstance, IRegistrationLicenseVerificationFailed> context)
        {
            Write("Invalid License: {0} ({1}) - {2}", context.Data.SubmissionId, context.Instance.ParticipantLicenseNumber, context.Data.ExceptionInfo.Message);
        }

        private void PaymentFailure(BehaviorContext<RegistrationStateInstance, IRegistrationPaymentFailed> context)
        {
            Write("Payment Failed: {0} ({1}) - {2}", context.Data.SubmissionId, context.Instance.ParticipantEmailAddress, context.Data.ExceptionInfo.Message);
        }
    }
}
