namespace Registration.Consumers
{
    public class SecurePaymentInfoService : ISecurePaymentInfoService
    {
        public ISecurePaymentInfo GetPaymentInfo(string emailAddress, string cardNumber)
        {
            return new Info(cardNumber, "123", "FRANK UNDERHILL", 12, 2019);
        }
    }

    public class Info : ISecurePaymentInfo
    {
        public string CardNumber { get; }
        public string VerificationCode { get; }
        public string CardholderName { get; }
        public int ExpirationMonth { get; }
        public int ExpirationYear { get; }

        public Info(string cardNumber, string verificationCode, string cardholderName, int expirationMonth, int expirationYear)
        {
            CardNumber = cardNumber;
            VerificationCode = verificationCode;
            CardholderName = cardholderName;
            ExpirationMonth = expirationMonth;
            ExpirationYear = expirationYear;
        }
    }
}
