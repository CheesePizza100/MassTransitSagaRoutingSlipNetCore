namespace Registration.Activities.EventRegistration
{
    public interface IEventRegistrationArguments
    {
        string ParticipantEmailAddress { get; }
        string ParticipantLicenseNumber { get; }
        string ParticipantCategory { get; }

        string EventId { get; }
        string RaceId { get; }
    }
}