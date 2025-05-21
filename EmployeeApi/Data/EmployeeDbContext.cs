using Microsoft.EntityFrameworkCore; 
using EmployeeApi.Models;
namespace EmployeeApi.Data
{
        public class EmployeeDbContext : DbContext
        {
            public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : base(options)
            {
            }
            public DbSet<Employee> Employees { get; set; }

        }
    }
