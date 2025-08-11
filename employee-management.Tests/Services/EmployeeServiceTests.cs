using Moq;
using employee_management.Server.Services;
using employee_management.Server.Data.Repositories;
using employee_management.Server.Models.Entities;
using employee_management.Server.Models.DTOs;
using employee_management.Server.Models.Responses;
using AutoMapper;

namespace employee_management.Tests.Services
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IEmployeeRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly EmployeeService _service;

        public EmployeeServiceTests()
        {
            _mockRepository = new Mock<IEmployeeRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new EmployeeService(_mockRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllEmployeesAsync_ShouldReturnSuccessResponse_WhenRepositoryReturnsData()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { Id = Guid.NewGuid(), Name = "John Doe", Email = "john@example.com" },
                new Employee { Id = Guid.NewGuid(), Name = "Jane Smith", Email = "jane@example.com" }
            };

            var employeeDtos = new List<EmployeeDto>
            {
                new EmployeeDto { Id = employees[0].Id, Name = "John Doe", Email = "john@example.com" },
                new EmployeeDto { Id = employees[1].Id, Name = "Jane Smith", Email = "jane@example.com" }
            };

            var paginatedResult = new PaginatedResult<Employee>(employees, 2, 1, 10);
            var expectedPaginatedResult = new PaginatedResult<EmployeeDto>(employeeDtos, 2, 1, 10);

            _mockRepository.Setup(r => r.GetAllAsync(1, 10, "Name", "asc"))
                .ReturnsAsync(paginatedResult);
            _mockMapper.Setup(m => m.Map<IEnumerable<EmployeeDto>>(employees))
                .Returns(employeeDtos);

            // Act
            var result = await _service.GetAllEmployeesAsync(1, 10, "Name", "asc");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(2, result.Data.TotalCount);
            Assert.Equal(2, result.Data.Data.Count());
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_ShouldReturnSuccessResponse_WhenEmployeeExists()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var employee = new Employee { Id = employeeId, Name = "John Doe", Email = "john@example.com" };
            var employeeDto = new EmployeeDto { Id = employeeId, Name = "John Doe", Email = "john@example.com" };

            _mockRepository.Setup(r => r.GetByIdAsync(employeeId))
                .ReturnsAsync(employee);
            _mockMapper.Setup(m => m.Map<EmployeeDto>(employee))
                .Returns(employeeDto);

            // Act
            var result = await _service.GetEmployeeByIdAsync(employeeId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(employeeId, result.Data.Id);
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_ShouldReturnNotFoundResponse_WhenEmployeeDoesNotExist()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(employeeId))
                .ReturnsAsync((Employee?)null);

            // Act
            var result = await _service.GetEmployeeByIdAsync(employeeId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(404, result.StatusCode);
            Assert.Contains("not found", result.Message.ToLower());
        }

        [Fact]
        public async Task CreateEmployeeAsync_ShouldReturnSuccessResponse_WhenEmployeeIsValid()
        {
            // Arrange
            var createDto = new CreateEmployeeDto
            {
                Name = "John Doe",
                Email = "john@example.com",
                DateOfBirth = DateTime.Now.AddYears(-25),
                DepartmentId = Guid.NewGuid()
            };

            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                Name = createDto.Name,
                Email = createDto.Email,
                DateOfBirth = createDto.DateOfBirth,
                DepartmentId = createDto.DepartmentId
            };

            var employeeDto = new EmployeeDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                DateOfBirth = employee.DateOfBirth,
                DepartmentId = employee.DepartmentId
            };

            _mockRepository.Setup(r => r.EmailExistsAsync(createDto.Email, null))
                .ReturnsAsync(false);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Employee>()))
                .ReturnsAsync(employee);
            _mockMapper.Setup(m => m.Map<Employee>(createDto))
                .Returns(employee);
            _mockMapper.Setup(m => m.Map<EmployeeDto>(employee))
                .Returns(employeeDto);

            // Act
            var result = await _service.CreateEmployeeAsync(createDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(employee.Id, result.Data.Id);
        }

        [Fact]
        public async Task CreateEmployeeAsync_ShouldReturnConflictResponse_WhenEmailAlreadyExists()
        {
            // Arrange
            var createDto = new CreateEmployeeDto
            {
                Name = "John Doe",
                Email = "existing@example.com",
                DateOfBirth = DateTime.Now.AddYears(-25),
                DepartmentId = Guid.NewGuid()
            };

            _mockRepository.Setup(r => r.EmailExistsAsync(createDto.Email, null))
                .ReturnsAsync(true);

            // Act
            var result = await _service.CreateEmployeeAsync(createDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(409, result.StatusCode);
            Assert.Contains("email", result.Message.ToLower());
        }

        [Fact]
        public async Task UpdateEmployeeAsync_ShouldReturnSuccessResponse_WhenEmployeeExistsAndIsValid()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var updateDto = new UpdateEmployeeDto
            {
                Name = "John Updated",
                Email = "john.updated@example.com",
                DateOfBirth = DateTime.Now.AddYears(-25),
                DepartmentId = Guid.NewGuid()
            };

            var existingEmployee = new Employee
            {
                Id = employeeId,
                Name = "John Doe",
                Email = "john@example.com",
                DateOfBirth = DateTime.Now.AddYears(-25),
                DepartmentId = Guid.NewGuid()
            };

            var updatedEmployee = new Employee
            {
                Id = employeeId,
                Name = updateDto.Name,
                Email = updateDto.Email,
                DateOfBirth = updateDto.DateOfBirth,
                DepartmentId = updateDto.DepartmentId
            };

            var employeeDto = new EmployeeDto
            {
                Id = employeeId,
                Name = updateDto.Name,
                Email = updateDto.Email,
                DateOfBirth = updateDto.DateOfBirth,
                DepartmentId = updateDto.DepartmentId
            };

            _mockRepository.Setup(r => r.GetByIdAsync(employeeId))
                .ReturnsAsync(existingEmployee);
            _mockRepository.Setup(r => r.EmailExistsAsync(updateDto.Email, employeeId))
                .ReturnsAsync(false);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Employee>()))
                .ReturnsAsync(updatedEmployee);
            _mockMapper.Setup(m => m.Map<Employee>(updateDto))
                .Returns(updatedEmployee);
            _mockMapper.Setup(m => m.Map<EmployeeDto>(updatedEmployee))
                .Returns(employeeDto);

            // Act
            var result = await _service.UpdateEmployeeAsync(employeeId, updateDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(updateDto.Name, result.Data.Name);
        }

        [Fact]
        public async Task UpdateEmployeeAsync_ShouldReturnNotFoundResponse_WhenEmployeeDoesNotExist()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var updateDto = new UpdateEmployeeDto
            {
                Name = "John Updated",
                Email = "john.updated@example.com",
                DateOfBirth = DateTime.Now.AddYears(-25),
                DepartmentId = Guid.NewGuid()
            };

            _mockRepository.Setup(r => r.GetByIdAsync(employeeId))
                .ReturnsAsync((Employee?)null);

            // Act
            var result = await _service.UpdateEmployeeAsync(employeeId, updateDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(404, result.StatusCode);
            Assert.Contains("not found", result.Message.ToLower());
        }

        [Fact]
        public async Task DeleteEmployeeAsync_ShouldReturnSuccessResponse_WhenEmployeeExists()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            _mockRepository.Setup(r => r.ExistsAsync(employeeId))
                .ReturnsAsync(true);
            _mockRepository.Setup(r => r.DeleteAsync(employeeId))
                .ReturnsAsync(true);

            // Act
            var result = await _service.DeleteEmployeeAsync(employeeId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task DeleteEmployeeAsync_ShouldReturnNotFoundResponse_WhenEmployeeDoesNotExist()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            _mockRepository.Setup(r => r.ExistsAsync(employeeId))
                .ReturnsAsync(false);

            // Act
            var result = await _service.DeleteEmployeeAsync(employeeId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(404, result.StatusCode);
            Assert.Contains("not found", result.Message.ToLower());
        }

        [Fact]
        public async Task SearchEmployeesAsync_ShouldReturnSuccessResponse_WhenSearchTermIsValid()
        {
            // Arrange
            var searchTerm = "john";
            var employees = new List<Employee>
            {
                new Employee { Id = Guid.NewGuid(), Name = "John Doe", Email = "john@example.com" }
            };

            var employeeDtos = new List<EmployeeDto>
            {
                new EmployeeDto { Id = employees[0].Id, Name = "John Doe", Email = "john@example.com" }
            };

            var paginatedResult = new PaginatedResult<Employee>(employees, 1, 1, 10);
            var expectedPaginatedResult = new PaginatedResult<EmployeeDto>(employeeDtos, 1, 1, 10);

            _mockRepository.Setup(r => r.SearchAsync(searchTerm, 1, 10, "Name", "asc"))
                .ReturnsAsync(paginatedResult);
            _mockMapper.Setup(m => m.Map<IEnumerable<EmployeeDto>>(employees))
                .Returns(employeeDtos);

            // Act
            var result = await _service.SearchEmployeesAsync(searchTerm, 1, 10, "Name", "asc");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(1, result.Data.TotalCount);
            Assert.Single(result.Data.Data);
        }

        [Fact]
        public async Task SearchEmployeesAsync_ShouldReturnBadRequestResponse_WhenSearchTermIsEmpty()
        {
            // Arrange
            var searchTerm = "";

            // Act
            var result = await _service.SearchEmployeesAsync(searchTerm);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Validation failed", result.Message);
            Assert.Contains("Search term cannot be empty", result.Errors);
        }

        [Fact]
        public async Task GetDeletedEmployeesAsync_ShouldReturnSuccessResponse_WhenRepositoryReturnsData()
        {
            // Arrange
            var deletedEmployees = new List<Employee>
            {
                new Employee { Id = Guid.NewGuid(), Name = "Deleted John", Email = "deleted@example.com", IsDeleted = true }
            };

            var employeeDtos = new List<EmployeeDto>
            {
                new EmployeeDto { Id = deletedEmployees[0].Id, Name = "Deleted John", Email = "deleted@example.com" }
            };

            _mockRepository.Setup(r => r.GetDeletedAsync())
                .ReturnsAsync(deletedEmployees);
            _mockMapper.Setup(m => m.Map<IEnumerable<EmployeeDto>>(deletedEmployees))
                .Returns(employeeDtos);

            // Act
            var result = await _service.GetDeletedEmployeesAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.Single(result.Data);
        }

        [Fact]
        public async Task RestoreEmployeeAsync_ShouldReturnSuccessResponse_WhenEmployeeExists()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            _mockRepository.Setup(r => r.ExistsAsync(employeeId))
                .ReturnsAsync(true);
            _mockRepository.Setup(r => r.RestoreAsync(employeeId))
                .ReturnsAsync(true);

            // Act
            var result = await _service.RestoreEmployeeAsync(employeeId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task RestoreEmployeeAsync_ShouldReturnNotFoundResponse_WhenEmployeeDoesNotExist()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            _mockRepository.Setup(r => r.ExistsAsync(employeeId))
                .ReturnsAsync(false);

            // Act
            var result = await _service.RestoreEmployeeAsync(employeeId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(404, result.StatusCode);
            Assert.Contains("not found", result.Message.ToLower());
        }
    }
} 