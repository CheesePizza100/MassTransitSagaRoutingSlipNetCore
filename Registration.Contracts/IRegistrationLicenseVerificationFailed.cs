using MassTransit;
using System;

namespace Registration.Contracts
{
    public interface IRegistrationLicenseVerificationFailed
    {
        Guid SubmissionId { get; }
        DateTime Timestamp { get; }

        ExceptionInfo ExceptionInfo { get; }
    }
}