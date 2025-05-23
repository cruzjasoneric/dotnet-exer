using EmployeeApi.Models;

namespace EmployeeBFF.Models.Requests
{
    public class EmployeeCreateRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Department { get; set; }
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }

    }
    public class BffEmployeeCreateRequest
    {
        public EmployeeCreateRequest Data { get; set; }
    }
}
