﻿using System;

namespace Registration.Contracts
{
    public interface ISubmitRegistration
    {
        Guid SubmissionId { get; }
        DateTime Timestamp { get; }

        string ParticipantEmailAddress { get; }
        string ParticipantLicenseNumber { get; }
        string ParticipantCategory { get; }

        string CardNumber { get; }

        string EventId { get; }
        string RaceId { get; }
    }
}