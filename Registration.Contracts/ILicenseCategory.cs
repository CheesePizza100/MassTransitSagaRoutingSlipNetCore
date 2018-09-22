namespace Registration.Contracts
{
    public interface ILicenseCategory
    {
        LicenseCategoryType CategoryType { get; }

        string Category { get; }
    }
}