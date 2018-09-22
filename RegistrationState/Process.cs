using Registration.Contracts;
using System;

namespace RegistrationState
{
    public class Process : IProcessRegistration
    {
        public Guid SubmissionId { get; }
        public DateTime Timestamp { get; }
        public string ParticipantEmailAddress { get; }
        public string ParticipantLicenseNumber { get; }
        public string ParticipantCategory { get; }
        public string EventId { get; }
        public string RaceId { get; }
        public string CardNumber { get; }

        public Process(Guid submissionId, string participantEmailAddress, string participantLicenseNumber, string participantCategory, string eventId,
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
