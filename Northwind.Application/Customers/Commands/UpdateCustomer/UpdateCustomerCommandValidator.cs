using FluentValidation;
using FluentValidation.Validators;

namespace Northwind.Application.Customers.Commands.UpdateCustomer
{
    public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
    {
        public UpdateCustomerCommandValidator()
        {
            RuleFor(x => x.Id).MaximumLength(5).NotEmpty();
            RuleFor(x => x.Address).MaximumLength(60);
            RuleFor(x => x.City).MaximumLength(15);
            RuleFor(x => x.CompanyName).MaximumLength(40).NotEmpty();
            RuleFor(x => x.ContactName).MaximumLength(30);
            RuleFor(x => x.ContactTitle).MaximumLength(30);
            RuleFor(x => x.Country).MaximumLength(15);
            RuleFor(x => x.Fax).MaximumLength(24).NotEmpty();
            RuleFor(x => x.Phone).MaximumLength(24).NotEmpty();
            RuleFor(x => x.PostalCode).MaximumLength(10);
            RuleFor(x => x.Region).MaximumLength(15);

            RuleFor(c => c.PostalCode).Matches(@"^\d{5}$")
                .When(c => c.Country == "Serbia")
                .WithMessage("Serbian Postcodes have 5 digits");

            RuleFor(c => c.Phone)
                .Must(HaveBelgradeLandLine)
                .When(c => c.Country == "Serbia" && c.City == "Belgrade")
                .WithMessage("Customers from Belgrade, Serbia require at least one valid landline.");
        }

        private static bool HaveBelgradeLandLine(UpdateCustomerCommand model, string phoneValue, PropertyValidatorContext ctx)
        {
            return model.Phone.StartsWith("011") || model.Fax.StartsWith("011");
        }
    }
}
