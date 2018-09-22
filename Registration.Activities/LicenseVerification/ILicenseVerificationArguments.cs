namespace Registration.Activities.LicenseVerification
{
    public interface ILicenseVerificationArguments
    {
        string LicenseNumber { get; }
        string EventType { get; }
        string Category { get; }
    }
}