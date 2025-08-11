using Microsoft.AspNetCore.Mvc;
using Moq;
using employee_management.Server.Controllers;
using employee_management.Server.Services;
using employee_management.Server.Models.DTOs;
using employee_management.Server.Models.Responses;
using employee_management.Server.Models.Entities;

namespace employee_management.Tests.Controllers
{
    public class EmployeesControllerTests
    {
        private readonly Mock<IEmployeeService> _mockService;
        private readonly EmployeesController _controller;

        public EmployeesControllerTests()
        {
            _mockService = new Mock<IEmployeeService>();
            _controller = new EmployeesController(_mockService.Object);
        }

        [Fact]
        public async Task GetEmployees_ShouldReturnOkResult_WhenServiceReturnsSuccess()
        {
            // Arrange
            var paginationRequest = new PaginationRequest { PageNumber = 1, PageSize = 10 };
            var employees = new List<EmployeeDto>
            {
                new EmployeeDto { Id = Guid.NewGuid(), Name = "John Doe", Email = "john@example.com" },
                new EmployeeDto { Id = Guid.NewGuid(), Name = "Jane Smith", Email = "jane@example.com" }
            };

            var paginatedResult = new PaginatedResult<EmployeeDto>(employees, 2, 1, 10);
            var serviceResponse = ApiResponse<PaginatedResult<EmployeeDto>>.Success(paginatedResult);

            _mockService.Setup(s => s.GetAllEmployeesAsync(1, 10, "Name", "asc"))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.GetEmployees(paginationRequest);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, statusCodeResult.StatusCode);
            
            var response = Assert.IsType<ApiResponse<PaginatedResult<EmployeeDto>>>(statusCodeResult.Value);
            Assert.True(response.IsSuccess);
            Assert.Equal(2, response.Data.TotalCount);
        }

        [Fact]
        public async Task GetEmployee_ShouldReturnOkResult_WhenEmployeeExists()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var employee = new EmployeeDto { Id = employeeId, Name = "John Doe", Email = "john@example.com" };
            var serviceResponse = ApiResponse<EmployeeDto>.Success(employee);

            _mockService.Setup(s => s.GetEmployeeByIdAsync(employeeId))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.GetEmployee(employeeId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, statusCodeResult.StatusCode);
            
            var response = Assert.IsType<ApiResponse<EmployeeDto>>(statusCodeResult.Value);
            Assert.True(response.IsSuccess);
            Assert.Equal(employeeId, response.Data.Id);
        }

        [Fact]
        public async Task GetEmployee_ShouldReturnNotFound_WhenEmployeeDoesNotExist()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var serviceResponse = ApiResponse<EmployeeDto>.NotFound("Employee not found");

            _mockService.Setup(s => s.GetEmployeeByIdAsync(employeeId))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.GetEmployee(employeeId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(404, statusCodeResult.StatusCode);
            
            var response = Assert.IsType<ApiResponse<EmployeeDto>>(statusCodeResult.Value);
            Assert.False(response.IsSuccess);
        }

        [Fact]
        public async Task SearchEmployees_ShouldReturnOkResult_WhenSearchIsValid()
        {
            // Arrange
            var searchTerm = "john";
            var paginationRequest = new PaginationRequest { PageNumber = 1, PageSize = 10 };
            var employees = new List<EmployeeDto>
            {
                new EmployeeDto { Id = Guid.NewGuid(), Name = "John Doe", Email = "john@example.com" }
            };

            var paginatedResult = new PaginatedResult<EmployeeDto>(employees, 1, 1, 10);
            var serviceResponse = ApiResponse<PaginatedResult<EmployeeDto>>.Success(paginatedResult);

            _mockService.Setup(s => s.SearchEmployeesAsync(searchTerm, 1, 10, "Name", "asc"))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.SearchEmployees(searchTerm, paginationRequest);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, statusCodeResult.StatusCode);
            
            var response = Assert.IsType<ApiResponse<PaginatedResult<EmployeeDto>>>(statusCodeResult.Value);
            Assert.True(response.IsSuccess);
            Assert.Single(response.Data.Data);
        }

        [Fact]
        public async Task CreateEmployee_ShouldReturnCreatedResult_WhenEmployeeIsValid()
        {
            // Arrange
            var createDto = new CreateEmployeeDto
            {
                Name = "John Doe",
                Email = "john@example.com",
                DateOfBirth = DateTime.Now.AddYears(-25),
                DepartmentId = Guid.NewGuid()
            };

            var createdEmployee = new EmployeeDto
            {
                Id = Guid.NewGuid(),
                Name = createDto.Name,
                Email = createDto.Email,
                DateOfBirth = createDto.DateOfBirth,
                DepartmentId = createDto.DepartmentId
            };

            var serviceResponse = ApiResponse<EmployeeDto>.Success(createdEmployee);

            _mockService.Setup(s => s.CreateEmployeeAsync(createDto))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.CreateEmployee(createDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, statusCodeResult.StatusCode);
            
            var response = Assert.IsType<ApiResponse<EmployeeDto>>(statusCodeResult.Value);
            Assert.True(response.IsSuccess);
            Assert.Equal(createDto.Name, response.Data.Name);
        }

        [Fact]
        public async Task CreateEmployee_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var createDto = new CreateEmployeeDto
            {
                Name = "", // Invalid - empty name
                Email = "invalid-email", // Invalid email format
                DateOfBirth = DateTime.Now.AddYears(-25),
                DepartmentId = Guid.NewGuid()
            };

            // Simulate ModelState errors
            _controller.ModelState.AddModelError("Name", "Name is required");
            _controller.ModelState.AddModelError("Email", "Invalid email format");

            // Act
            var result = await _controller.CreateEmployee(createDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(400, statusCodeResult.StatusCode);
            
            var response = Assert.IsType<ApiResponse<EmployeeDto>>(statusCodeResult.Value);
            Assert.False(response.IsSuccess);
            Assert.Equal(400, response.StatusCode);
            Assert.Contains("Name is required", response.Errors);
            Assert.Contains("Invalid email format", response.Errors);
        }

        [Fact]
        public async Task UpdateEmployee_ShouldReturnOkResult_WhenUpdateIsValid()
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

            var updatedEmployee = new EmployeeDto
            {
                Id = employeeId,
                Name = updateDto.Name,
                Email = updateDto.Email,
                DateOfBirth = updateDto.DateOfBirth,
                DepartmentId = updateDto.DepartmentId
            };

            var serviceResponse = ApiResponse<EmployeeDto>.Success(updatedEmployee);

            _mockService.Setup(s => s.UpdateEmployeeAsync(employeeId, updateDto))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.UpdateEmployee(employeeId, updateDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, statusCodeResult.StatusCode);
            
            var response = Assert.IsType<ApiResponse<EmployeeDto>>(statusCodeResult.Value);
            Assert.True(response.IsSuccess);
            Assert.Equal(updateDto.Name, response.Data.Name);
        }

        [Fact]
        public async Task UpdateEmployee_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var updateDto = new UpdateEmployeeDto
            {
                Name = "", // Invalid - empty name
                Email = "invalid-email", // Invalid email format
                DateOfBirth = DateTime.Now.AddYears(-25),
                DepartmentId = Guid.NewGuid()
            };

            // Simulate ModelState errors
            _controller.ModelState.AddModelError("Name", "Name is required");
            _controller.ModelState.AddModelError("Email", "Invalid email format");

            // Act
            var result = await _controller.UpdateEmployee(employeeId, updateDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(400, statusCodeResult.StatusCode);
            
            var response = Assert.IsType<ApiResponse<EmployeeDto>>(statusCodeResult.Value);
            Assert.False(response.IsSuccess);
            Assert.Equal(400, response.StatusCode);
            Assert.Contains("Name is required", response.Errors);
            Assert.Contains("Invalid email format", response.Errors);
        }

        [Fact]
        public async Task DeleteEmployee_ShouldReturnOkResult_WhenDeleteIsSuccessful()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var serviceResponse = ApiResponse<bool>.Success(true);

            _mockService.Setup(s => s.DeleteEmployeeAsync(employeeId))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.DeleteEmployee(employeeId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, statusCodeResult.StatusCode);
            
            var response = Assert.IsType<ApiResponse<bool>>(statusCodeResult.Value);
            Assert.True(response.IsSuccess);
            Assert.True(response.Data);
        }

        [Fact]
        public async Task GetDeletedEmployees_ShouldReturnOkResult_WhenServiceReturnsData()
        {
            // Arrange
            var deletedEmployees = new List<EmployeeDto>
            {
                new EmployeeDto { Id = Guid.NewGuid(), Name = "Deleted John", Email = "deleted@example.com" }
            };

            var serviceResponse = ApiResponse<IEnumerable<EmployeeDto>>.Success(deletedEmployees);

            _mockService.Setup(s => s.GetDeletedEmployeesAsync())
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.GetDeletedEmployees();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, statusCodeResult.StatusCode);
            
            var response = Assert.IsType<ApiResponse<IEnumerable<EmployeeDto>>>(statusCodeResult.Value);
            Assert.True(response.IsSuccess);
            Assert.Single(response.Data);
        }

        [Fact]
        public async Task RestoreEmployee_ShouldReturnOkResult_WhenRestoreIsSuccessful()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var serviceResponse = ApiResponse<bool>.Success(true);

            _mockService.Setup(s => s.RestoreEmployeeAsync(employeeId))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.RestoreEmployee(employeeId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, statusCodeResult.StatusCode);
            
            var response = Assert.IsType<ApiResponse<bool>>(statusCodeResult.Value);
            Assert.True(response.IsSuccess);
            Assert.True(response.Data);
        }
    }
} 