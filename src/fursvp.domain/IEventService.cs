namespace fursvp.domain
{
    public interface IEventService
    {
        void AddMember(Event @event, Member member);
        Event CreateNewEvent(string authenticatedEmailAddress, string emailAddress, string name);
    }
}