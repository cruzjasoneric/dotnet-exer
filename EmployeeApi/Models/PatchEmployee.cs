namespace EmployeeApi.Models
{
    public class PatchEmployee
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Department { get; set; }
        public DateTime? HireDate { get; set; }
        public decimal? Salary { get; set; }

    }
}