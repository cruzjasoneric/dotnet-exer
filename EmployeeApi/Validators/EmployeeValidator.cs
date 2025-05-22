using EmployeeApi.Models;
using FluentValidation;

namespace EmployeeApi.Validators
{
    public class EmployeeValidator : AbstractValidator<Employee>
    {
        public EmployeeValidator()
        {
            RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("FirstName is required.")
            .Matches("^[a-zA-Z ]+$").WithMessage("Name must contain only letters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("LastName is required.")
            .Matches("^[a-zA-Z ]+$").WithMessage("Name must contain only letters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Department)
                .NotEmpty().WithMessage("Department is required.");

            RuleFor(x => x.HireDate)
                .LessThanOrEqualTo(DateTime.Today)
            .WithMessage("HireDate cannot be in the future.");

            RuleFor(x => x.Salary)
                .GreaterThan(0).WithMessage("Salary must be greater than 0.");


        }
    }
}
