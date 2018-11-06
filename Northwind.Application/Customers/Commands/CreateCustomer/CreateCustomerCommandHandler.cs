using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Northwind.Application.Interfaces;
using Northwind.Application.Notifications.Models;
using Northwind.Domain.Entities;
using Northwind.Persistence;

namespace Northwind.Application.Customers.Commands.CreateCustomer
{
    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Unit>
    {
        private readonly NorthwindDbContext _context;
        private readonly INotificationService _notificationService;

        public CreateCustomerCommandHandler(
            NorthwindDbContext context,
            INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<Unit> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var entity = new Customer
            {
                CustomerId = request.Id,
                Address = request.Address,
                City = request.City,
                CompanyName = request.CompanyName,
                ContactName = request.ContactName,
                ContactTitle = request.ContactTitle,
                Country = request.Country,
                Fax = request.Fax,
                Phone = request.Phone,
                PostalCode = request.PostalCode
            };

            _context.Customers.Add(entity);

            await _context.SaveChangesAsync(cancellationToken);

			await _notificationService.Send(new Message
			{
				From = "noreply@acumenics.com",
				To = "vladimir.gosic@acumenics.com",
				Subject = "Northwind - TODO: MediatR - Events",
				Body = "Remember to update the create customer command handler demo to use a MediatR event."
			});

			return Unit.Value;
        }
    }
}
