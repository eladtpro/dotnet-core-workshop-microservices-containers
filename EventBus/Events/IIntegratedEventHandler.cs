using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace EventBus.Events
{
    public interface IIntegratedEventHandler
    {
        Task HandleAsync(Message message);
    }
}