using System;

namespace Registration.Activities.EventRegistration
{
    public interface IEventRegistrationLog
    {
        Guid RegistrationId { get; }
        string ParticipantEmailAddress { get; }
    }
}