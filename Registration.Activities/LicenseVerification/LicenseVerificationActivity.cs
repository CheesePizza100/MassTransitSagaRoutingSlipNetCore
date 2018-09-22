using MassTransit.Courier;
using MassTransit.Courier.Exceptions;
using System;
using System.Threading.Tasks;
using static System.Console;

namespace Registration.Activities.LicenseVerification
{
    public class LicenseVerificationActivity : ExecuteActivity<ILicenseVerificationArguments>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<ILicenseVerificationArguments> context)
        {
            WriteLine("Verifying license: {0}", context.Arguments.LicenseNumber);

            // verify license with remote service
            if (context.Arguments.LicenseNumber == "8675309")
            {
                throw new RoutingSlipException($"The license number is invalid: {context.Arguments.LicenseNumber}");
            }

            var expirationDate = DateTime.Today + TimeSpan.FromDays(90);

            return context.CompletedWithVariables(new
            {
                LicenseExpirationDate = expirationDate
            });
        }
    }
}