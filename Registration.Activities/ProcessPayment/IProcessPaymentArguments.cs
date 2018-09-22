namespace Registration.Activities.ProcessPayment
{
    public interface IProcessPaymentArguments
    {
        string CardNumber { get; }
        string VerificationCode { get; }
        string CardholderName { get; }
        int ExpirationMonth { get; }
        int ExpirationYear { get; }
        decimal Amount { get; }
    }
}