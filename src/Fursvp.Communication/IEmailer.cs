using System.Threading.Tasks;

namespace Fursvp.Communication
{
    public interface IEmailer
    {
        void Send(Email email);
        Task SendAsync(Email email);
    }
}
