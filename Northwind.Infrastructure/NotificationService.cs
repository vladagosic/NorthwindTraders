using Northwind.Application.Interfaces;
using Northwind.Application.Notifications.Models;
using System.Threading.Tasks;

namespace Northwind.Infrastructure
{
    public class NotificationService : INotificationService
    {
        public async Task Send(Message message)
        {
			// Do nothing			
        }
    }
}
