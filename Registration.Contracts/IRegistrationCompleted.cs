using System;

namespace Registration.Contracts
{
    public interface IRegistrationCompleted
    {
        Guid SubmissionId { get; }
        DateTime Timestamp { get; }
        Guid TrackingNumber { get; }
    }
}