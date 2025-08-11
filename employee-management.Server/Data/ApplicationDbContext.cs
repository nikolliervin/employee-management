using Microsoft.EntityFrameworkCore;
using employee_management.Server.Models.Entities;

namespace employee_management.Server.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Employee entity
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.Property(e => e.DateOfBirth).IsRequired();
            
            // Configure relationship with Department
            entity.HasOne(e => e.Department)
                  .WithMany(d => d.Employees)
                  .HasForeignKey(e => e.DepartmentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Department entity
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name).IsRequired().HasMaxLength(100);
            entity.Property(d => d.Description).HasMaxLength(500);
        });

        // Seed initial departments
        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Name = "Engineering", Description = "Software development and engineering" },
            new Department { Id = 2, Name = "Marketing", Description = "Marketing and communications" },
            new Department { Id = 3, Name = "Human Resources", Description = "HR and employee management" },
            new Department { Id = 4, Name = "Finance", Description = "Financial planning and accounting" },
            new Department { Id = 5, Name = "Sales", Description = "Sales and business development" }
        );

        // Seed sample employees
        modelBuilder.Entity<Employee>().HasData(
            new Employee 
            { 
                Id = 1, 
                Name = "John Doe", 
                Email = "john.doe@company.com", 
                DateOfBirth = new DateTime(1990, 5, 15), 
                DepartmentId = 1,
                CreatedAt = DateTime.UtcNow
            },
            new Employee 
            { 
                Id = 2, 
                Name = "Jane Smith", 
                Email = "jane.smith@company.com", 
                DateOfBirth = new DateTime(1988, 8, 22), 
                DepartmentId = 2,
                CreatedAt = DateTime.UtcNow
            },
            new Employee 
            { 
                Id = 3, 
                Name = "Mike Johnson", 
                Email = "mike.johnson@company.com", 
                DateOfBirth = new DateTime(1992, 3, 10), 
                DepartmentId = 1,
                CreatedAt = DateTime.UtcNow
            }
        );
    }
} 