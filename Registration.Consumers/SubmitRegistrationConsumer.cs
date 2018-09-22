using MassTransit;
using Registration.Contracts;
using System;
using System.Threading.Tasks;
using static System.Console;

namespace Registration.Consumers
{
    public class SubmitRegistrationConsumer : IConsumer<ISubmitRegistration>
    {
        public async Task Consume(ConsumeContext<ISubmitRegistration> context)
        {
            WriteLine("Registration received: {0} ({1})", context.Message.SubmissionId, context.Message.ParticipantEmailAddress);

            ValidateRegistration(context.Message);

            var received = CreateReceivedEvent(context.Message);

            await context.Publish(received);

            WriteLine("Registration accepted: {0} ({1})", context.Message.SubmissionId, context.Message.ParticipantEmailAddress);
        }

        private static IRegistrationReceived CreateReceivedEvent(ISubmitRegistration message)
        {
            return new Received(message.SubmissionId, message.ParticipantEmailAddress, message.ParticipantLicenseNumber,
                message.ParticipantCategory, message.EventId, message.RaceId, message.CardNumber);
        }

        private void ValidateRegistration(ISubmitRegistration message)
        {
            if (string.IsNullOrWhiteSpace(message.EventId))
                throw new ArgumentNullException(nameof(message.EventId));
            if (string.IsNullOrWhiteSpace(message.RaceId))
                throw new ArgumentNullException(nameof(message.RaceId));

            if (string.IsNullOrWhiteSpace(message.ParticipantEmailAddress))
                throw new ArgumentNullException(nameof(message.ParticipantEmailAddress));
            if (string.IsNullOrWhiteSpace(message.ParticipantCategory))
                throw new ArgumentNullException(nameof(message.ParticipantCategory));
        }
    }

    public class Received : IRegistrationReceived
    {
        public Guid SubmissionId { get; }
        public DateTime Timestamp { get; }
        public string ParticipantEmailAddress { get; }
        public string ParticipantLicenseNumber { get; }
        public string ParticipantCategory { get; }
        public string EventId { get; }
        public string RaceId { get; }
        public string CardNumber { get; }

        public Received(Guid submissionId, string participantEmailAddress, string participantLicenseNumber, string participantCategory, string eventId,
            string raceId, string cardNumber)
        {
            SubmissionId = submissionId;
            ParticipantEmailAddress = participantEmailAddress;
            ParticipantLicenseNumber = participantLicenseNumber;
            ParticipantCategory = participantCategory;
            EventId = eventId;
            RaceId = raceId;
            CardNumber = cardNumber;

            Timestamp = DateTime.UtcNow;
        }
    }
}