using MassTransit;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;
using Registration.Contracts;
using System.Threading.Tasks;
using static System.Console;

namespace Registration.Consumers
{
    public class ProcessRegistrationConsumer : IConsumer<IProcessRegistration>
    {
        readonly ISecurePaymentInfoService _paymentInfoService;

        public ProcessRegistrationConsumer()
        {
            _paymentInfoService = new SecurePaymentInfoService();
        }

        public async Task Consume(ConsumeContext<IProcessRegistration> context)
        {
            WriteLine("Processing registration: {0} ({1})", context.Message.SubmissionId, context.Message.ParticipantEmailAddress);

            var routingSlip = CreateRoutingSlip(context);

            await context.Execute(routingSlip);
        }

        private RoutingSlip CreateRoutingSlip(ConsumeContext<IProcessRegistration> context)
        {
            var builder = new RoutingSlipBuilder(NewId.NextGuid());

            if (!string.IsNullOrWhiteSpace(context.Message.ParticipantLicenseNumber))
            {
                // context.GetDestinationAddress("execute-licenseverification"),
                builder.AddActivity("LicenseVerificiation", context.DestinationAddress, new
                {
                    LicenseNumber = context.Message.ParticipantLicenseNumber,
                    EventType = "Road",
                    Category = context.Message.ParticipantCategory
                });

                builder.AddSubscription(context.SourceAddress, RoutingSlipEvents.ActivityFaulted, RoutingSlipEventContents.None, "LicenseVerificiation",
                    x => x.Send<IRegistrationLicenseVerificationFailed>(new
                    {
                        context.Message.SubmissionId
                    }));
            }

            // context.GetDestinationAddress("execute-eventregistration")
            builder.AddActivity("EventRegistration", context.DestinationAddress, new
            {
                context.Message.ParticipantEmailAddress,
                context.Message.ParticipantLicenseNumber,
                context.Message.ParticipantCategory,
                context.Message.EventId,
                context.Message.RaceId
            });

            var paymentInfo = _paymentInfoService.GetPaymentInfo(context.Message.ParticipantEmailAddress, context.Message.CardNumber);

            //context.GetDestinationAddress("execute-processpayment")
            builder.AddActivity("ProcessPayment", context.DestinationAddress, paymentInfo);

            builder.AddSubscription(context.SourceAddress, RoutingSlipEvents.ActivityFaulted, RoutingSlipEventContents.None, "ProcessPayment",
                x => x.Send<IRegistrationPaymentFailed>(new
                {
                    context.Message.SubmissionId
                }));


            builder.AddSubscription(context.SourceAddress, RoutingSlipEvents.Completed, x => x.Send<IRegistrationCompleted>(new
            {
                context.Message.SubmissionId
            }));


            return builder.Build();
        }
    }
}