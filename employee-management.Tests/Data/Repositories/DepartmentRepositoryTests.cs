using Microsoft.EntityFrameworkCore;
using employee_management.Server.Data;
using employee_management.Server.Data.Repositories;
using employee_management.Server.Models.Entities;
using employee_management.Server.Models.DTOs;

namespace employee_management.Tests.Data.Repositories
{
    public class DepartmentRepositoryTests : IDisposable
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly ApplicationDbContext _context;
        private readonly DepartmentRepository _repository;

        public DepartmentRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(_options);
            _repository = new DepartmentRepository(_context);
        }



        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedResults_WhenDepartmentsExist()
        {
            // Arrange
            var request = new PaginationRequest
            {
                PageNumber = 1,
                PageSize = 10,
                SortBy = "Name",
                SortOrder = "asc"
            };

            // Seed test data
            var departments = new List<Department>
            {
                new Department
                {
                    Id = Guid.NewGuid(),
                    Name = "Engineering",
                    Description = "Software Engineering Department",
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    CreatedBy = "System",
                    IsDeleted = false
                },
                new Department
                {
                    Id = Guid.NewGuid(),
                    Name = "Marketing",
                    Description = "Marketing Department",
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    CreatedBy = "System",
                    IsDeleted = false
                }
            };

            _context.Departments.AddRange(departments);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Data.Count());
            Assert.Equal(1, result.PageNumber);
            Assert.Equal(10, result.PageSize);

            // Check sorting
            var departmentsList = result.Data.ToList();
            Assert.Equal("Engineering", departmentsList[0].Name);
            Assert.Equal("Marketing", departmentsList[1].Name);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnCorrectPageSize_WhenPageSizeIsLimited()
        {
            // Arrange
            var request = new PaginationRequest
            {
                PageNumber = 1,
                PageSize = 1,
                SortBy = "Name",
                SortOrder = "asc"
            };

            // Seed test data
            var departments = new List<Department>
            {
                new Department
                {
                    Id = Guid.NewGuid(),
                    Name = "Engineering",
                    Description = "Software Engineering Department",
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    CreatedBy = "System",
                    IsDeleted = false
                },
                new Department
                {
                    Id = Guid.NewGuid(),
                    Name = "Marketing",
                    Description = "Marketing Department",
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    CreatedBy = "System",
                    IsDeleted = false
                }
            };

            _context.Departments.AddRange(departments);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.Single(result.Data);
            Assert.Equal("Engineering", result.Data.First().Name);
        }

        [Fact]
        public async Task GetAllAsync_ShouldSortDescending_WhenSortOrderIsDesc()
        {
            // Arrange
            var request = new PaginationRequest
            {
                PageNumber = 1,
                PageSize = 10,
                SortBy = "Name",
                SortOrder = "desc"
            };

            // Seed test data
            var departments = new List<Department>
            {
                new Department
                {
                    Id = Guid.NewGuid(),
                    Name = "Engineering",
                    Description = "Software Engineering Department",
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    CreatedBy = "System",
                    IsDeleted = false
                },
                new Department
                {
                    Id = Guid.NewGuid(),
                    Name = "Marketing",
                    Description = "Marketing Department",
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    CreatedBy = "System",
                    IsDeleted = false
                }
            };

            _context.Departments.AddRange(departments);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync(request);

            // Assert
            var departmentsList = result.Data.ToList();
            Assert.Equal("Marketing", departmentsList[0].Name);
            Assert.Equal("Engineering", departmentsList[1].Name);
        }

        [Fact]
        public async Task SearchAsync_ShouldReturnMatchingDepartments_WhenSearchTermMatches()
        {
            // Arrange
            var searchRequest = new SearchRequest
            {
                SearchTerm = "Engineering",
                PageNumber = 1,
                PageSize = 10,
                SortBy = "Name",
                SortOrder = "asc"
            };

            // Seed test data
            var departments = new List<Department>
            {
                new Department
                {
                    Id = Guid.NewGuid(),
                    Name = "Engineering",
                    Description = "Software Engineering Department",
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    CreatedBy = "System",
                    IsDeleted = false
                },
                new Department
                {
                    Id = Guid.NewGuid(),
                    Name = "Marketing",
                    Description = "Marketing Department",
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    CreatedBy = "System",
                    IsDeleted = false
                }
            };

            _context.Departments.AddRange(departments);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.SearchAsync(searchRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Data);
            Assert.Equal("Engineering", result.Data.First().Name);
        }

        [Fact]
        public async Task SearchAsync_ShouldReturnEmptyResults_WhenSearchTermDoesNotMatch()
        {
            // Arrange
            var searchRequest = new SearchRequest
            {
                SearchTerm = "NonExistent",
                PageNumber = 1,
                PageSize = 10,
                SortBy = "Name",
                SortOrder = "asc"
            };

            // Act
            var result = await _repository.SearchAsync(searchRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.TotalCount);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDepartment_WhenDepartmentExists()
        {
            // Arrange
            var department = new Department
            {
                Id = Guid.NewGuid(),
                Name = "Test Department",
                Description = "Test Description",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsDeleted = false
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(department.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(department.Id, result.Id);
            Assert.Equal(department.Name, result.Name);
            Assert.Equal(department.Description, result.Description);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenDepartmentDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.GetByIdAsync(nonExistentId);

            // Assert
            Assert.Null(result);
        }



        [Fact]
        public async Task AddAsync_ShouldAddDepartment_WhenDepartmentIsValid()
        {
            // Arrange
            var newDepartment = new Department
            {
                Id = Guid.NewGuid(),
                Name = "Human Resources",
                Description = "HR Department",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "TestUser"
            };

            // Act
            var result = await _repository.CreateAsync(newDepartment);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newDepartment.Id, result.Id);
            Assert.Equal(newDepartment.Name, result.Name);

            // Verify it was actually saved
            var savedDepartment = await _context.Departments.FindAsync(newDepartment.Id);
            Assert.NotNull(savedDepartment);
            Assert.Equal("Human Resources", savedDepartment.Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateDepartment_WhenDepartmentExists()
        {
            // Arrange
            var department = new Department
            {
                Id = Guid.NewGuid(),
                Name = "Original Name",
                Description = "Original Description",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsDeleted = false
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            var originalName = department.Name;
            
            department.Name = "Updated Name";
            department.Description = "Updated Description";
            department.UpdatedAt = DateTime.UtcNow;
            department.UpdatedBy = "TestUser";

            // Act
            var result = await _repository.UpdateAsync(department);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Name", result.Name);
            Assert.Equal("Updated Description", result.Description);
            Assert.NotEqual(originalName, result.Name);

            // Verify it was actually updated
            var updatedDepartment = await _context.Departments.FindAsync(department.Id);
            Assert.Equal("Updated Name", updatedDepartment.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldSoftDeleteDepartment_WhenDepartmentExistsAndHasNoEmployees()
        {
            // Arrange - Create a fresh department to delete
            var departmentToDelete = new Department
            {
                Id = Guid.NewGuid(),
                Name = "Department To Delete",
                Description = "This department will be deleted",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsDeleted = false
            };

            _context.Departments.Add(departmentToDelete);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteAsync(departmentToDelete.Id);

            // Assert
            Assert.True(result);

            // Verify soft delete
            var deletedDepartment = await _context.Departments
                .IgnoreQueryFilters()
                .FirstAsync(d => d.Id == departmentToDelete.Id);
            
            Assert.True(deletedDepartment.IsDeleted);
            Assert.NotNull(deletedDepartment.DeletedAt);
            Assert.NotNull(deletedDepartment.DeletedBy);

            // Verify it doesn't appear in normal queries
            // Since in-memory DB doesn't handle global query filters, we'll check manually
            var normalQuery = await _context.Departments
                .Where(d => !d.IsDeleted)
                .FirstOrDefaultAsync(d => d.Id == departmentToDelete.Id);
            Assert.Null(normalQuery);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowException_WhenDepartmentHasEmployees()
        {
            // Arrange - Create a department with an employee
            var departmentWithEmployees = new Department
            {
                Id = Guid.NewGuid(),
                Name = "Department With Employees",
                Description = "This department has employees",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsDeleted = false
            };

            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                Name = "John Doe",
                Email = "john@example.com",
                DateOfBirth = DateTime.Now.AddYears(-30),
                DepartmentId = departmentWithEmployees.Id,
                Department = departmentWithEmployees,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsDeleted = false
            };

            _context.Departments.Add(departmentWithEmployees);
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _repository.DeleteAsync(departmentWithEmployees.Id));
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenDepartmentDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.DeleteAsync(nonExistentId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenDepartmentExists()
        {
            // Arrange
            var department = new Department
            {
                Id = Guid.NewGuid(),
                Name = "Test Department",
                Description = "Test Description",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsDeleted = false
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(department.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnFalse_WhenDepartmentDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.ExistsAsync(nonExistentId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task NameExistsAsync_ShouldReturnTrue_WhenNameExists()
        {
            // Arrange
            var department = new Department
            {
                Id = Guid.NewGuid(),
                Name = "Engineering",
                Description = "Engineering Department",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsDeleted = false
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.NameExistsAsync("Engineering");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task NameExistsAsync_ShouldReturnFalse_WhenNameDoesNotExist()
        {
            // Arrange
            var nonExistentName = "NonExistentDepartment";

            // Act
            var result = await _repository.NameExistsAsync(nonExistentName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task NameExistsAsync_ShouldExcludeSpecifiedId_WhenExcludeIdIsProvided()
        {
            // Arrange
            var department = new Department
            {
                Id = Guid.NewGuid(),
                Name = "Engineering",
                Description = "Engineering Department",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsDeleted = false
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.NameExistsAsync("Engineering", department.Id);

            // Assert
            Assert.False(result); // Should return false because we're excluding the department with this name
        }





        [Fact]
        public async Task GetDeletedAsync_ShouldReturnEmptyList_WhenNoDeletedDepartments()
        {
            // Arrange - Create a fresh context with no deleted departments
            var freshContext = new ApplicationDbContext(_options);
            var freshRepository = new DepartmentRepository(freshContext);
            
            // Add only active departments
            var activeDepartments = new List<Department>
            {
                new Department
                {
                    Id = Guid.NewGuid(),
                    Name = "Active Department",
                    Description = "Active Department",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    IsDeleted = false
                }
            };
            
            freshContext.Departments.AddRange(activeDepartments);
            await freshContext.SaveChangesAsync();

            // Act
            var result = await freshRepository.GetDeletedAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            
            // Cleanup
            freshContext.Dispose();
        }



        [Fact]
        public async Task RestoreAsync_ShouldReturnFalse_WhenDepartmentDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.RestoreAsync(nonExistentId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task RestoreAsync_ShouldReturnFalse_WhenDepartmentIsNotDeleted()
        {
            // Arrange
            var activeDepartment = new Department
            {
                Id = Guid.NewGuid(),
                Name = "Active Department",
                Description = "This department is active",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsDeleted = false
            };

            _context.Departments.Add(activeDepartment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.RestoreAsync(activeDepartment.Id);

            // Assert
            Assert.False(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
