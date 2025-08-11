using Microsoft.EntityFrameworkCore;
using employee_management.Server.Models.Entities;
using employee_management.Server.Models.Common;

namespace employee_management.Server.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
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
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            
            entity.HasOne(e => e.Department)
                  .WithMany(d => d.Employees)
                  .HasForeignKey(e => e.DepartmentId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            // Configure audit fields
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.RowVersion).IsRowVersion();
            
            // Global query filter for soft delete
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure Department entity
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Id).ValueGeneratedOnAdd();
            
            // Configure audit fields
            entity.Property(d => d.CreatedAt).IsRequired();
            entity.Property(d => d.CreatedBy).IsRequired().HasMaxLength(100);
            entity.Property(d => d.UpdatedBy).HasMaxLength(100);
            entity.Property(d => d.DeletedBy).HasMaxLength(100);
            entity.Property(d => d.RowVersion).IsRowVersion();
            
            // Global query filter for soft delete
            entity.HasQueryFilter(d => !d.IsDeleted);
        });

        // Seed initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Departments
        var departments = new[]
        {
            new Department
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Engineering",
                Description = "Software development and engineering team",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsDeleted = false
            },
            new Department
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Human Resources",
                Description = "HR and recruitment team",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsDeleted = false
            },
            new Department
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Marketing",
                Description = "Marketing and communications team",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsDeleted = false
            }
        };

        modelBuilder.Entity<Department>().HasData(departments);

        // Seed Employees
        var employees = new[]
        {
            new Employee
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Name = "John Doe",
                Email = "john.doe@company.com",
                DateOfBirth = new DateTime(1990, 5, 15),
                DepartmentId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsDeleted = false
            },
            new Employee
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Name = "Jane Smith",
                Email = "jane.smith@company.com",
                DateOfBirth = new DateTime(1988, 8, 22),
                DepartmentId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsDeleted = false
            },
            new Employee
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                Name = "Bob Johnson",
                Email = "bob.johnson@company.com",
                DateOfBirth = new DateTime(1992, 3, 10),
                DepartmentId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsDeleted = false
            }
        };

        modelBuilder.Entity<Employee>().HasData(employees);
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries<IAuditable>();

        foreach (var entry in entries)
        {
            if (entry.Entity is IAuditable auditable)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        auditable.CreatedAt = DateTime.UtcNow;
                        auditable.CreatedBy = "System"; // TODO: Get from current user context
                        auditable.IsDeleted = false;
                        break;
                    
                    case EntityState.Modified:
                        auditable.UpdatedAt = DateTime.UtcNow;
                        auditable.UpdatedBy = "System"; // TODO: Get from current user context
                        break;
                    
                    case EntityState.Deleted:
                        // Convert delete to soft delete
                        entry.State = EntityState.Modified;
                        auditable.IsDeleted = true;
                        auditable.DeletedAt = DateTime.UtcNow;
                        auditable.DeletedBy = "System"; // TODO: Get from current user context
                        break;
                }
            }
        }
    }
} 