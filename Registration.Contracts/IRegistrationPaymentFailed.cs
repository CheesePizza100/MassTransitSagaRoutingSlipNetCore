using MassTransit;
using System;

namespace Registration.Contracts
{
    public interface IRegistrationPaymentFailed
    {
        Guid SubmissionId { get; }
        DateTime Timestamp { get; }

        ExceptionInfo ExceptionInfo { get; }
    }
}