using Microsoft.AspNetCore.Mvc;
using Moq;
using employee_management.Server.Controllers;
using employee_management.Server.Services;
using employee_management.Server.Models.DTOs;
using employee_management.Server.Models.Responses;

namespace employee_management.Tests.Controllers
{
    public class DepartmentsControllerTests
    {
        private readonly Mock<IDepartmentService> _mockService;
        private readonly DepartmentsController _controller;

        public DepartmentsControllerTests()
        {
            _mockService = new Mock<IDepartmentService>();
            _controller = new DepartmentsController(_mockService.Object);
        }

        [Fact]
        public async Task GetDepartments_ShouldReturnOkResult_WhenServiceReturnsSuccess()
        {
            // Arrange
            var paginationRequest = new PaginationRequest { PageNumber = 1, PageSize = 10 };
            var departments = new List<DepartmentDto>
            {
                new DepartmentDto { Id = Guid.NewGuid(), Name = "Engineering", Description = "Software Engineering" },
                new DepartmentDto { Id = Guid.NewGuid(), Name = "Marketing", Description = "Marketing Department" }
            };

            var paginatedResult = new PaginatedResult<DepartmentDto>(departments, 2, 1, 10);
            var serviceResponse = ApiResponse<PaginatedResult<DepartmentDto>>.Success(paginatedResult);

            _mockService.Setup(s => s.GetAllDepartmentsAsync(paginationRequest))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.GetDepartments(paginationRequest);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, statusCodeResult.StatusCode);
            
            Assert.NotNull(statusCodeResult.Value);
            var response = Assert.IsType<ApiResponse<PaginatedResult<DepartmentDto>>>(statusCodeResult.Value);
            Assert.True(response.IsSuccess);
            Assert.Equal(2, response.Data.TotalCount);
        }

        [Fact]
        public async Task GetDepartment_ShouldReturnOkResult_WhenDepartmentExists()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var department = new DepartmentDto { Id = departmentId, Name = "Engineering", Description = "Software Engineering" };
            var serviceResponse = ApiResponse<DepartmentDto>.Success(department);

            _mockService.Setup(s => s.GetDepartmentByIdAsync(departmentId))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.GetDepartment(departmentId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, statusCodeResult.StatusCode);
            
            Assert.NotNull(statusCodeResult.Value);
            var response = Assert.IsType<ApiResponse<DepartmentDto>>(statusCodeResult.Value);
            Assert.True(response.IsSuccess);
            Assert.Equal(departmentId, response.Data.Id);
        }

        [Fact]
        public async Task GetDepartment_ShouldReturnNotFound_WhenDepartmentDoesNotExist()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var serviceResponse = ApiResponse<DepartmentDto>.NotFound("Department not found");

            _mockService.Setup(s => s.GetDepartmentByIdAsync(departmentId))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.GetDepartment(departmentId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(404, statusCodeResult.StatusCode);
            
            Assert.NotNull(statusCodeResult.Value);
            var response = Assert.IsType<ApiResponse<DepartmentDto>>(statusCodeResult.Value);
            Assert.False(response.IsSuccess);
        }

        [Fact]
        public async Task SearchDepartments_ShouldReturnOkResult_WhenSearchIsValid()
        {
            // Arrange
            var searchTerm = "engineering";
            var departments = new List<DepartmentDto>
            {
                new DepartmentDto { Id = Guid.NewGuid(), Name = "Engineering", Description = "Software Engineering" }
            };

            var paginatedResult = new PaginatedResult<DepartmentDto>(departments, 1, 1, 10);
            var serviceResponse = ApiResponse<PaginatedResult<DepartmentDto>>.Success(paginatedResult);

            var searchRequest = new SearchRequest 
            { 
                SearchTerm = searchTerm, 
                PageNumber = 1, 
                PageSize = 10, 
                SortBy = "Name", 
                SortOrder = "asc" 
            };
            _mockService.Setup(s => s.SearchDepartmentsAsync(searchRequest))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.SearchDepartments(searchRequest);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, statusCodeResult.StatusCode);
            
            var response = Assert.IsType<ApiResponse<PaginatedResult<DepartmentDto>>>(statusCodeResult.Value);
            Assert.True(response.IsSuccess);
            Assert.Single(response.Data.Data);
        }

        [Fact]
        public async Task CreateDepartment_ShouldReturnCreatedResult_WhenDepartmentIsValid()
        {
            // Arrange
            var createDto = new CreateDepartmentDto
            {
                Name = "Human Resources",
                Description = "HR Department"
            };

            var createdDepartment = new DepartmentDto
            {
                Id = Guid.NewGuid(),
                Name = createDto.Name,
                Description = createDto.Description
            };

            var serviceResponse = ApiResponse<DepartmentDto>.Created(createdDepartment);

            _mockService.Setup(s => s.CreateDepartmentAsync(createDto))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.CreateDepartment(createDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(201, statusCodeResult.StatusCode);
            
            Assert.NotNull(statusCodeResult.Value);
            var response = Assert.IsType<ApiResponse<DepartmentDto>>(statusCodeResult.Value);
            Assert.True(response.IsSuccess);
            Assert.Equal(createDto.Name, response.Data.Name);
        }

        [Fact]
        public async Task CreateDepartment_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var createDto = new CreateDepartmentDto
            {
                Name = "", // Invalid - empty name
                Description = "Some description"
            };

            // Simulate ModelState errors
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.CreateDepartment(createDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(400, statusCodeResult.StatusCode);
            
            Assert.NotNull(statusCodeResult.Value);
            var response = Assert.IsType<ApiResponse<DepartmentDto>>(statusCodeResult.Value);
            Assert.False(response.IsSuccess);
            Assert.Equal(400, response.StatusCode);
            Assert.Contains("Name is required", response.Errors);
        }

        [Fact]
        public async Task CreateDepartment_ShouldReturnConflict_WhenDepartmentNameAlreadyExists()
        {
            // Arrange
            var createDto = new CreateDepartmentDto
            {
                Name = "Engineering",
                Description = "Duplicate department"
            };

            var serviceResponse = ApiResponse<DepartmentDto>.Conflict("Department with this name already exists");

            _mockService.Setup(s => s.CreateDepartmentAsync(createDto))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.CreateDepartment(createDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(409, statusCodeResult.StatusCode);
            
            Assert.NotNull(statusCodeResult.Value);
            var response = Assert.IsType<ApiResponse<DepartmentDto>>(statusCodeResult.Value);
            Assert.False(response.IsSuccess);
            Assert.Equal(409, response.StatusCode);
        }

        [Fact]
        public async Task UpdateDepartment_ShouldReturnOkResult_WhenUpdateIsValid()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var updateDto = new UpdateDepartmentDto
            {
                Name = "Engineering Updated",
                Description = "Updated Engineering Department"
            };

            var updatedDepartment = new DepartmentDto
            {
                Id = departmentId,
                Name = updateDto.Name,
                Description = updateDto.Description
            };

            var serviceResponse = ApiResponse<DepartmentDto>.Success(updatedDepartment);

            _mockService.Setup(s => s.UpdateDepartmentAsync(departmentId, updateDto))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.UpdateDepartment(departmentId, updateDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, statusCodeResult.StatusCode);
            
            Assert.NotNull(statusCodeResult.Value);
            var response = Assert.IsType<ApiResponse<DepartmentDto>>(statusCodeResult.Value);
            Assert.True(response.IsSuccess);
            Assert.Equal(updateDto.Name, response.Data.Name);
        }

        [Fact]
        public async Task UpdateDepartment_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var updateDto = new UpdateDepartmentDto
            {
                Name = "", // Invalid - empty name
                Description = "Some description"
            };

            // Simulate ModelState errors
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.UpdateDepartment(departmentId, updateDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(400, statusCodeResult.StatusCode);
            
            var response = Assert.IsType<ApiResponse<DepartmentDto>>(statusCodeResult.Value);
            Assert.False(response.IsSuccess);
            Assert.Equal(400, response.StatusCode);
            Assert.Contains("Name is required", response.Errors);
        }

        [Fact]
        public async Task UpdateDepartment_ShouldReturnNotFound_WhenDepartmentDoesNotExist()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var updateDto = new UpdateDepartmentDto
            {
                Name = "Updated Name",
                Description = "Updated Description"
            };

            var serviceResponse = ApiResponse<DepartmentDto>.NotFound("Department not found");

            _mockService.Setup(s => s.UpdateDepartmentAsync(departmentId, updateDto))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.UpdateDepartment(departmentId, updateDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(404, statusCodeResult.StatusCode);
            
            var response = Assert.IsType<ApiResponse<DepartmentDto>>(statusCodeResult.Value);
            Assert.False(response.IsSuccess);
        }

        [Fact]
        public async Task DeleteDepartment_ShouldReturnOkResult_WhenDeleteIsSuccessful()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var serviceResponse = ApiResponse<bool>.Success(true);

            _mockService.Setup(s => s.DeleteDepartmentAsync(departmentId))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.DeleteDepartment(departmentId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, statusCodeResult.StatusCode);
            
            Assert.NotNull(statusCodeResult.Value);
            var response = Assert.IsType<ApiResponse<bool>>(statusCodeResult.Value);
            Assert.True(response.IsSuccess);
            Assert.True(response.Data);
        }

        [Fact]
        public async Task DeleteDepartment_ShouldReturnConflict_WhenDepartmentHasEmployees()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var serviceResponse = ApiResponse<bool>.Conflict("Cannot delete department with existing employees");

            _mockService.Setup(s => s.DeleteDepartmentAsync(departmentId))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.DeleteDepartment(departmentId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(409, statusCodeResult.StatusCode);
            
            var response = Assert.IsType<ApiResponse<bool>>(statusCodeResult.Value);
            Assert.False(response.IsSuccess);
            Assert.Equal(409, response.StatusCode);
        }

        [Fact]
        public async Task GetDeletedDepartments_ShouldReturnOkResult_WhenServiceReturnsData()
        {
            // Arrange
            var deletedDepartments = new List<DepartmentDto>
            {
                new DepartmentDto { Id = Guid.NewGuid(), Name = "Deleted Department", Description = "This was deleted" }
            };

            var serviceResponse = ApiResponse<IEnumerable<DepartmentDto>>.Success(deletedDepartments);

            _mockService.Setup(s => s.GetDeletedDepartmentsAsync())
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.GetDeletedDepartments();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, statusCodeResult.StatusCode);
            
            Assert.NotNull(statusCodeResult.Value);
            var response = Assert.IsType<ApiResponse<IEnumerable<DepartmentDto>>>(statusCodeResult.Value);
            Assert.True(response.IsSuccess);
            Assert.Single(response.Data);
        }

        [Fact]
        public async Task RestoreDepartment_ShouldReturnOkResult_WhenRestoreIsSuccessful()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var serviceResponse = ApiResponse<bool>.Success(true);

            _mockService.Setup(s => s.RestoreDepartmentAsync(departmentId))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.RestoreDepartment(departmentId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, statusCodeResult.StatusCode);
            
            Assert.NotNull(statusCodeResult.Value);
            var response = Assert.IsType<ApiResponse<bool>>(statusCodeResult.Value);
            Assert.True(response.IsSuccess);
            Assert.True(response.Data);
        }

        [Fact]
        public async Task RestoreDepartment_ShouldReturnNotFound_WhenDepartmentNotFoundOrNotDeleted()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var serviceResponse = ApiResponse<bool>.NotFound("Department not found or not deleted");

            _mockService.Setup(s => s.RestoreDepartmentAsync(departmentId))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.RestoreDepartment(departmentId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(404, statusCodeResult.StatusCode);
            
            var response = Assert.IsType<ApiResponse<bool>>(statusCodeResult.Value);
            Assert.False(response.IsSuccess);
        }
    }
}
