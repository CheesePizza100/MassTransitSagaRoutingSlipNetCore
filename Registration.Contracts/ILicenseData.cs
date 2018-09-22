using System.Collections.Generic;

namespace Registration.Contracts
{
    public interface ILicenseData
    {
        string LicenseNumber { get; set; }
        List<ILicenseCategory> Categories { get; set; }
    }
}