using Northwind.Application.Notifications.Models;
using System.Threading.Tasks;

namespace Northwind.Application.Interfaces
{
    public interface INotificationService
    {
        Task Send(Message message);
    }
}
