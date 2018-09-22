using System;

namespace Registration.Activities.ProcessPayment
{
    public interface IProcessPaymentLog
    {
        DateTime ChargeDate { get; }
        string AuthorizationCode { get; }
        decimal Amount { get; }
    }
}