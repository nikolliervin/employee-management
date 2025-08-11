using Microsoft.EntityFrameworkCore;
using employee_management.Server.Data;
using employee_management.Server.Data.Repositories;
using employee_management.Server.Models.Entities;
using employee_management.Server.Models.Common;

namespace employee_management.Tests.Data.Repositories
{
    public class EmployeeRepositoryTests : IDisposable
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly ApplicationDbContext _context;
        private readonly EmployeeRepository _repository;

        public EmployeeRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(_options);
            _repository = new EmployeeRepository(_context);

            // Seed test data
            SeedTestData();
        }

        private void SeedTestData()
        {
            var department = new Department
            {
                Id = Guid.NewGuid(),
                Name = "Engineering",
                Description = "Software Engineering Department"
            };

            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                Name = "John Doe",
                Email = "john@example.com",
                DateOfBirth = DateTime.Now.AddYears(-25),
                DepartmentId = department.Id,
                Department = department
            };

            _context.Departments.Add(department);
            _context.Employees.Add(employee);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedEmployees()
        {
            // Act
            var result = await _repository.GetAllAsync(1, 5, "Name", "asc");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            Assert.Equal(1, result.PageNumber);
            Assert.Equal(5, result.PageSize);
            Assert.Single(result.Data);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyWhenNoEmployees()
        {
            // Arrange
            _context.Employees.RemoveRange(_context.Employees);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync(1, 5, "Name", "asc");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.TotalCount);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task GetAllAsync_ShouldSortByNameAscending()
        {
            // Arrange
            var employee2 = new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Alice Smith",
                Email = "alice@example.com",
                DateOfBirth = DateTime.Now.AddYears(-30),
                DepartmentId = _context.Departments.First().Id
            };
            _context.Employees.Add(employee2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync(1, 10, "Name", "asc");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            var employees = result.Data.ToList();
            Assert.Equal("Alice Smith", employees[0].Name);
            Assert.Equal("John Doe", employees[1].Name);
        }

        [Fact]
        public async Task GetAllAsync_ShouldSortByNameDescending()
        {
            // Arrange
            var employee2 = new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Alice Smith",
                Email = "alice@example.com",
                DateOfBirth = DateTime.Now.AddYears(-30),
                DepartmentId = _context.Departments.First().Id
            };
            _context.Employees.Add(employee2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync(1, 10, "Name", "desc");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            var employees = result.Data.ToList();
            Assert.Equal("John Doe", employees[0].Name);
            Assert.Equal("Alice Smith", employees[1].Name);
        }

        [Fact]
        public async Task SearchAsync_ShouldReturnMatchingEmployees()
        {
            // Act
            var result = await _repository.SearchAsync("john", 1, 5, "Name", "asc");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Data);
            Assert.Contains("john", result.Data.First().Name.ToLower());
        }

        [Fact]
        public async Task SearchAsync_ShouldReturnEmptyWhenNoMatches()
        {
            // Act
            var result = await _repository.SearchAsync("nonexistent", 1, 5, "Name", "asc");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.TotalCount);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task SearchAsync_ShouldSearchByEmail()
        {
            // Act
            var result = await _repository.SearchAsync("john@example.com", 1, 5, "Name", "asc");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Data);
            Assert.Equal("john@example.com", result.Data.First().Email);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnEmployee_WhenExists()
        {
            // Arrange
            var existingEmployee = _context.Employees.First();

            // Act
            var result = await _repository.GetByIdAsync(existingEmployee.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingEmployee.Id, result.Id);
            Assert.Equal(existingEmployee.Name, result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenEmployeeDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.GetByIdAsync(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_ShouldAddEmployee()
        {
            // Arrange
            var newEmployee = new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Jane Smith",
                Email = "jane@example.com",
                DateOfBirth = DateTime.Now.AddYears(-28),
                DepartmentId = _context.Departments.First().Id
            };

            // Act
            var result = await _repository.AddAsync(newEmployee);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newEmployee.Id, result.Id);
            Assert.Equal(newEmployee.Name, result.Name);

            // Verify it was saved to database
            var savedEmployee = await _context.Employees.FindAsync(newEmployee.Id);
            Assert.NotNull(savedEmployee);
            Assert.Equal(newEmployee.Name, savedEmployee.Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateEmployee()
        {
            // Arrange
            var existingEmployee = _context.Employees.First();
            var originalName = existingEmployee.Name;
            existingEmployee.Name = "Updated Name";

            // Act
            var result = await _repository.UpdateAsync(existingEmployee);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Name", result.Name);

            // Verify it was updated in database
            var updatedEmployee = await _context.Employees.FindAsync(existingEmployee.Id);
            Assert.NotNull(updatedEmployee);
            Assert.Equal("Updated Name", updatedEmployee.Name);
            Assert.NotEqual(originalName, updatedEmployee.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldSoftDeleteEmployee()
        {
            // Arrange
            var existingEmployee = _context.Employees.First();
            Assert.False(existingEmployee.IsDeleted);

            // Act
            var result = await _repository.DeleteAsync(existingEmployee.Id);

            // Assert
            Assert.True(result);

            // Verify it was soft deleted
            var deletedEmployee = await _context.Employees.FindAsync(existingEmployee.Id);
            Assert.NotNull(deletedEmployee);
            Assert.True(deletedEmployee.IsDeleted);
            Assert.NotNull(deletedEmployee.DeletedAt);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenEmployeeDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.DeleteAsync(nonExistentId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenEmployeeExists()
        {
            // Arrange
            var existingEmployee = _context.Employees.First();

            // Act
            var result = await _repository.ExistsAsync(existingEmployee.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnFalse_WhenEmployeeDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.ExistsAsync(nonExistentId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task EmailExistsAsync_ShouldReturnTrue_WhenEmailExists()
        {
            // Arrange
            var existingEmail = "john@example.com";

            // Act
            var result = await _repository.EmailExistsAsync(existingEmail);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task EmailExistsAsync_ShouldReturnFalse_WhenEmailDoesNotExist()
        {
            // Arrange
            var nonExistentEmail = "nonexistent@example.com";

            // Act
            var result = await _repository.EmailExistsAsync(nonExistentEmail);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task EmailExistsAsync_ShouldExcludeEmployee_WhenExcludeIdProvided()
        {
            // Arrange
            var existingEmployee = _context.Employees.First();
            var existingEmail = existingEmployee.Email;

            // Act
            var result = await _repository.EmailExistsAsync(existingEmail, existingEmployee.Id);

            // Assert
            Assert.False(result); // Should not find the email when excluding the current employee
        }

        [Fact]
        public async Task GetDeletedAsync_ShouldReturnOnlyDeletedEmployees()
        {
            // Arrange
            var existingEmployee = _context.Employees.First();
            await _repository.DeleteAsync(existingEmployee.Id);

            // Act
            var result = await _repository.GetDeletedAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.True(result.First().IsDeleted);
        }

        [Fact]
        public async Task RestoreAsync_ShouldRestoreEmployee()
        {
            // Arrange
            var existingEmployee = _context.Employees.First();
            await _repository.DeleteAsync(existingEmployee.Id);
            Assert.True(existingEmployee.IsDeleted);

            // Act
            var result = await _repository.RestoreAsync(existingEmployee.Id);

            // Assert
            Assert.True(result);

            // Verify it was restored
            var restoredEmployee = await _context.Employees.FindAsync(existingEmployee.Id);
            Assert.NotNull(restoredEmployee);
            Assert.False(restoredEmployee.IsDeleted);
            Assert.Null(restoredEmployee.DeletedAt);
        }

        [Fact]
        public async Task RestoreAsync_ShouldReturnFalse_WhenEmployeeDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.RestoreAsync(nonExistentId);

            // Assert
            Assert.False(result);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
} 