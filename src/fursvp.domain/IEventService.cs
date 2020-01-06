namespace fursvp.domain
{
    public interface IEventService
    {
        void AddMember(Event @event, Member member);
        Event CreateNewEvent(string emailAddress, string name);
        bool RsvpOpen(Event @event);
    }
}