using EmployeeBFF.Models.Requests;
using FluentValidation;

namespace EmployeeBFF.Validators
{
    public class BffUpdateEmployeeValidator: AbstractValidator<EmployeeUpdateRequest>
    {
        public BffUpdateEmployeeValidator()
        {
            RuleFor(x => x.FirstName)
            .Matches("^[A-Za-z]+(?:[ -][A-Za-z]+)*$").WithMessage("Invalid FirstName")
            .When(x => !string.IsNullOrEmpty(x.FirstName));

            RuleFor(x => x.LastName)
            .Matches("^[A-Za-z]+(?:[ -][A-Za-z]+)*$").WithMessage("Invalid LastName")
            .When(x => !string.IsNullOrEmpty(x.LastName));

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format.")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Phone)
                .Matches("^\\+?[0-9]{7,15}$").WithMessage("Invalid phone format")
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.HireDate)
                .LessThanOrEqualTo(DateTime.Today)
            .WithMessage("HireDate cannot be in the future.")
            .When(x => x.HireDate.HasValue);

            RuleFor(x => x.Salary)
                .GreaterThan(0).WithMessage("Salary must be greater than 0.")
                .When(x => x.Salary.HasValue);


        }
    }
}
