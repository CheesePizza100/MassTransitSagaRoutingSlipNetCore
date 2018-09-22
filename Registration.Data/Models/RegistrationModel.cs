using Registration.Contracts;
using System;

namespace Registration.Data.Models
{
    public class RegistrationModel : ISubmitRegistration
    {
        public Guid SubmissionId { get; set; }
        public DateTime Timestamp { get; set; }
        public string ParticipantEmailAddress { get; set; }
        public string ParticipantLicenseNumber { get; set; }
        public string ParticipantCategory { get; set; }
        public string EventId { get; set; }
        public string RaceId { get; set; }
        public string Status { get; set; }
        public string CardNumber { get; set; }
    }
}