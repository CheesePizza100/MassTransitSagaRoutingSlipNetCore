using MassTransit.Courier;
using MassTransit.Courier.Exceptions;
using System;
using System.Threading.Tasks;
using static System.Console;

namespace Registration.Activities.ProcessPayment
{
    public class ProcessPaymentActivity : Activity<IProcessPaymentArguments, IProcessPaymentLog>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<IProcessPaymentArguments> context)
        {
            WriteLine("Processing Payment: {0}", context.Arguments.Amount);

            if (context.Arguments.CardNumber == "4147")
            {
                throw new RoutingSlipException("The card number is invalid");
            }

            var authorizationCode = "ABC123";

            return context.Completed(new Log(authorizationCode, context.Arguments.Amount));
        }

        public async Task<CompensationResult> Compensate(CompensateContext<IProcessPaymentLog> context)
        {
            return context.Compensated();
        }
    }

    public class Log : IProcessPaymentLog
    {
        public DateTime ChargeDate { get; }
        public string AuthorizationCode { get; }
        public decimal Amount { get; }

        public Log(string authorizationCode, decimal amount)
        {
            AuthorizationCode = authorizationCode;
            Amount = amount;
            ChargeDate = DateTime.Today;
        }

    }
}