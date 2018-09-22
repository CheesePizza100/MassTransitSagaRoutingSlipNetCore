namespace Registration.Consumers
{
    public interface ISecurePaymentInfoService
    {
        ISecurePaymentInfo GetPaymentInfo(string emailAddress, string cardNumber);
    }
}