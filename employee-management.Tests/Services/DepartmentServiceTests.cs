using Moq;
using Microsoft.Extensions.Logging;
using employee_management.Server.Services;
using employee_management.Server.Data.Repositories;
using employee_management.Server.Models.Entities;
using employee_management.Server.Models.DTOs;
using employee_management.Server.Models.Responses;
using AutoMapper;

namespace employee_management.Tests.Services
{
    public class DepartmentServiceTests
    {
        private readonly Mock<IDepartmentRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<DepartmentService>> _mockLogger;
        private readonly DepartmentService _service;

        public DepartmentServiceTests()
        {
            _mockRepository = new Mock<IDepartmentRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<DepartmentService>>();
            _service = new DepartmentService(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllDepartmentsAsync_ShouldReturnSuccessResponse_WhenRepositoryReturnsData()
        {
            // Arrange
            var departments = new List<Department>
            {
                new Department { Id = Guid.NewGuid(), Name = "Engineering", Description = "Software Engineering" },
                new Department { Id = Guid.NewGuid(), Name = "Marketing", Description = "Marketing Department" }
            };

            var departmentDtos = new List<DepartmentDto>
            {
                new DepartmentDto { Id = departments[0].Id, Name = "Engineering", Description = "Software Engineering" },
                new DepartmentDto { Id = departments[1].Id, Name = "Marketing", Description = "Marketing Department" }
            };

            var paginatedResult = new PaginatedResult<Department>(departments, 2, 1, 10);
            var expectedPaginatedResult = new PaginatedResult<DepartmentDto>(departmentDtos, 2, 1, 10);

            var request = new PaginationRequest { PageNumber = 1, PageSize = 10, SortBy = "Name", SortOrder = "asc" };

            _mockRepository.Setup(r => r.GetAllAsync(request))
                .ReturnsAsync(paginatedResult);
            _mockMapper.Setup(m => m.Map<IEnumerable<DepartmentDto>>(departments))
                .Returns(departmentDtos);

            // Act
            var result = await _service.GetAllDepartmentsAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.TotalCount);
            Assert.Equal(2, result.Data.Data.Count());

            _mockRepository.Verify(r => r.GetAllAsync(request), Times.Once);
            _mockMapper.Verify(m => m.Map<IEnumerable<DepartmentDto>>(departments), Times.Once);
        }

        [Fact]
        public async Task GetAllDepartmentsAsync_ShouldReturnErrorResponse_WhenExceptionIsThrown()
        {
            // Arrange
            var request = new PaginationRequest { PageNumber = 1, PageSize = 10 };
            var exception = new Exception("Database error");

            _mockRepository.Setup(r => r.GetAllAsync(request))
                .ThrowsAsync(exception);

            // Act
            var result = await _service.GetAllDepartmentsAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(500, result.StatusCode);
            Assert.Contains("Unable to retrieve departments at this time", result.Message);

            // Verify logging
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to retrieve departments")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetDepartmentByIdAsync_ShouldReturnSuccessResponse_WhenDepartmentExists()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var department = new Department { Id = departmentId, Name = "Engineering", Description = "Software Engineering" };
            var departmentDto = new DepartmentDto { Id = departmentId, Name = "Engineering", Description = "Software Engineering" };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId))
                .ReturnsAsync(department);
            _mockMapper.Setup(m => m.Map<DepartmentDto>(department))
                .Returns(departmentDto);

            // Act
            var result = await _service.GetDepartmentByIdAsync(departmentId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Equal(departmentId, result.Data.Id);
            Assert.Equal("Engineering", result.Data.Name);

            _mockRepository.Verify(r => r.GetByIdAsync(departmentId), Times.Once);
            _mockMapper.Verify(m => m.Map<DepartmentDto>(department), Times.Once);
        }

        [Fact]
        public async Task GetDepartmentByIdAsync_ShouldReturnNotFoundResponse_WhenDepartmentDoesNotExist()
        {
            // Arrange
            var departmentId = Guid.NewGuid();

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId))
                .ReturnsAsync((Department)null);

            // Act
            var result = await _service.GetDepartmentByIdAsync(departmentId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(404, result.StatusCode);
            Assert.Contains("not found", result.Message.ToLower());

            _mockRepository.Verify(r => r.GetByIdAsync(departmentId), Times.Once);
            _mockMapper.Verify(m => m.Map<DepartmentDto>(It.IsAny<Department>()), Times.Never);
        }

        [Fact]
        public async Task SearchDepartmentsAsync_ShouldReturnSuccessResponse_WhenSearchFindsResults()
        {
            // Arrange
            var searchRequest = new SearchRequest
            {
                SearchTerm = "engineering",
                PageNumber = 1,
                PageSize = 10,
                SortBy = "Name",
                SortOrder = "asc"
            };

            var departments = new List<Department>
            {
                new Department { Id = Guid.NewGuid(), Name = "Engineering", Description = "Software Engineering" }
            };

            var departmentDtos = new List<DepartmentDto>
            {
                new DepartmentDto { Id = departments[0].Id, Name = "Engineering", Description = "Software Engineering" }
            };

            var paginatedResult = new PaginatedResult<Department>(departments, 1, 1, 10);

            _mockRepository.Setup(r => r.SearchAsync(searchRequest))
                .ReturnsAsync(paginatedResult);
            _mockMapper.Setup(m => m.Map<IEnumerable<DepartmentDto>>(departments))
                .Returns(departmentDtos);

            // Act
            var result = await _service.SearchDepartmentsAsync(searchRequest);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data.Data);

            _mockRepository.Verify(r => r.SearchAsync(searchRequest), Times.Once);
        }

        [Fact]
        public async Task CreateDepartmentAsync_ShouldReturnCreatedResponse_WhenDepartmentIsValidAndUnique()
        {
            // Arrange
            var createDto = new CreateDepartmentDto
            {
                Name = "Human Resources",
                Description = "HR Department"
            };

            var department = new Department
            {
                Id = Guid.NewGuid(),
                Name = createDto.Name,
                Description = createDto.Description
            };

            var departmentDto = new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description
            };

            _mockRepository.Setup(r => r.NameExistsAsync(createDto.Name, null))
                .ReturnsAsync(false);
            _mockMapper.Setup(m => m.Map<Department>(createDto))
                .Returns(department);
            _mockRepository.Setup(r => r.CreateAsync(department))
                .ReturnsAsync(department);
            _mockMapper.Setup(m => m.Map<DepartmentDto>(department))
                .Returns(departmentDto);

            // Act
            var result = await _service.CreateDepartmentAsync(createDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(201, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Equal(createDto.Name, result.Data.Name);
            Assert.Equal(createDto.Description, result.Data.Description);

            _mockRepository.Verify(r => r.NameExistsAsync(createDto.Name, null), Times.Once);
            _mockRepository.Verify(r => r.CreateAsync(department), Times.Once);
        }

        [Fact]
        public async Task CreateDepartmentAsync_ShouldReturnConflictResponse_WhenDepartmentNameAlreadyExists()
        {
            // Arrange
            var createDto = new CreateDepartmentDto
            {
                Name = "Engineering",
                Description = "Duplicate Engineering Department"
            };

            _mockRepository.Setup(r => r.NameExistsAsync(createDto.Name, null))
                .ReturnsAsync(true);

            // Act
            var result = await _service.CreateDepartmentAsync(createDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("already exists", result.Errors.First());

            _mockRepository.Verify(r => r.NameExistsAsync(createDto.Name, null), Times.Once);
            _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Department>()), Times.Never);
        }

        [Fact]
        public async Task CreateDepartmentAsync_ShouldReturnErrorResponse_WhenExceptionIsThrown()
        {
            // Arrange
            var createDto = new CreateDepartmentDto
            {
                Name = "Test Department",
                Description = "Test Description"
            };

            var exception = new Exception("Database error");

            _mockRepository.Setup(r => r.NameExistsAsync(createDto.Name, null))
                .ReturnsAsync(false);
            _mockMapper.Setup(m => m.Map<Department>(createDto))
                .Returns(new Department());
            _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Department>()))
                .ThrowsAsync(exception);

            // Act
            var result = await _service.CreateDepartmentAsync(createDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(500, result.StatusCode);
            Assert.Contains("Unable to create department at this time", result.Message);

            // Verify logging
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to create department")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateDepartmentAsync_ShouldReturnSuccessResponse_WhenUpdateIsValid()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var updateDto = new UpdateDepartmentDto
            {
                Name = "Engineering Updated",
                Description = "Updated Engineering Department"
            };

            var existingDepartment = new Department
            {
                Id = departmentId,
                Name = "Engineering",
                Description = "Software Engineering"
            };

            var updatedDepartment = new Department
            {
                Id = departmentId,
                Name = updateDto.Name,
                Description = updateDto.Description
            };

            var departmentDto = new DepartmentDto
            {
                Id = departmentId,
                Name = updateDto.Name,
                Description = updateDto.Description
            };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId))
                .ReturnsAsync(existingDepartment);
            _mockRepository.Setup(r => r.NameExistsAsync(updateDto.Name, departmentId))
                .ReturnsAsync(false);
            _mockMapper.Setup(m => m.Map(updateDto, existingDepartment))
                .Returns(updatedDepartment);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Department>()))
                .ReturnsAsync(updatedDepartment);
            _mockMapper.Setup(m => m.Map<DepartmentDto>(It.IsAny<Department>()))
                .Returns(departmentDto);

            // Act
            var result = await _service.UpdateDepartmentAsync(departmentId, updateDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Equal(updateDto.Name, result.Data.Name);

            _mockRepository.Verify(r => r.GetByIdAsync(departmentId), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Department>()), Times.Once);
        }

        [Fact]
        public async Task UpdateDepartmentAsync_ShouldReturnNotFoundResponse_WhenDepartmentDoesNotExist()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var updateDto = new UpdateDepartmentDto
            {
                Name = "Updated Name",
                Description = "Updated Description"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId))
                .ReturnsAsync((Department)null);

            // Act
            var result = await _service.UpdateDepartmentAsync(departmentId, updateDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(404, result.StatusCode);
            Assert.Contains("not found", result.Message.ToLower());

            _mockRepository.Verify(r => r.GetByIdAsync(departmentId), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Department>()), Times.Never);
        }

        [Fact]
        public async Task UpdateDepartmentAsync_ShouldReturnConflictResponse_WhenNameAlreadyExists()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var updateDto = new UpdateDepartmentDto
            {
                Name = "Marketing", // This name already exists
                Description = "Updated Description"
            };

            var existingDepartment = new Department
            {
                Id = departmentId,
                Name = "Engineering",
                Description = "Software Engineering"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId))
                .ReturnsAsync(existingDepartment);
            _mockRepository.Setup(r => r.NameExistsAsync(updateDto.Name, departmentId))
                .ReturnsAsync(true);

            // Act
            var result = await _service.UpdateDepartmentAsync(departmentId, updateDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("already exists", result.Errors.First());

            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Department>()), Times.Never);
        }

        [Fact]
        public async Task DeleteDepartmentAsync_ShouldReturnSuccessResponse_WhenDeleteIsSuccessful()
        {
            // Arrange
            var departmentId = Guid.NewGuid();

            _mockRepository.Setup(r => r.DeleteAsync(departmentId))
                .ReturnsAsync(true);

            // Act
            var result = await _service.DeleteDepartmentAsync(departmentId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.True(result.Data);

            _mockRepository.Verify(r => r.DeleteAsync(departmentId), Times.Once);
        }

        [Fact]
        public async Task DeleteDepartmentAsync_ShouldReturnConflictResponse_WhenDepartmentHasEmployees()
        {
            // Arrange
            var departmentId = Guid.NewGuid();

            _mockRepository.Setup(r => r.DeleteAsync(departmentId))
                .ThrowsAsync(new InvalidOperationException("Cannot delete department with active employees"));

            // Act
            var result = await _service.DeleteDepartmentAsync(departmentId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("employees", result.Errors.First().ToLower());

            _mockRepository.Verify(r => r.DeleteAsync(departmentId), Times.Once);
        }

        [Fact]
        public async Task DeleteDepartmentAsync_ShouldReturnNotFoundResponse_WhenDepartmentDoesNotExist()
        {
            // Arrange
            var departmentId = Guid.NewGuid();

            _mockRepository.Setup(r => r.DeleteAsync(departmentId))
                .ReturnsAsync(false);

            // Act
            var result = await _service.DeleteDepartmentAsync(departmentId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(404, result.StatusCode);
            Assert.Contains("not found", result.Message.ToLower());

            _mockRepository.Verify(r => r.DeleteAsync(departmentId), Times.Once);
        }

        [Fact]
        public async Task GetDeletedDepartmentsAsync_ShouldReturnSuccessResponse_WhenDeletedDepartmentsExist()
        {
            // Arrange
            var deletedDepartments = new List<Department>
            {
                new Department { Id = Guid.NewGuid(), Name = "Deleted Dept", Description = "Was deleted", IsDeleted = true }
            };

            var departmentDtos = new List<DepartmentDto>
            {
                new DepartmentDto { Id = deletedDepartments[0].Id, Name = "Deleted Dept", Description = "Was deleted" }
            };

            _mockRepository.Setup(r => r.GetDeletedAsync())
                .ReturnsAsync(deletedDepartments);
            _mockMapper.Setup(m => m.Map<IEnumerable<DepartmentDto>>(deletedDepartments))
                .Returns(departmentDtos);

            // Act
            var result = await _service.GetDeletedDepartmentsAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data);

            _mockRepository.Verify(r => r.GetDeletedAsync(), Times.Once);
        }

        [Fact]
        public async Task RestoreDepartmentAsync_ShouldReturnSuccessResponse_WhenRestoreIsSuccessful()
        {
            // Arrange
            var departmentId = Guid.NewGuid();

            _mockRepository.Setup(r => r.RestoreAsync(departmentId))
                .ReturnsAsync(true);

            // Act
            var result = await _service.RestoreDepartmentAsync(departmentId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.True(result.Data);

            _mockRepository.Verify(r => r.RestoreAsync(departmentId), Times.Once);
        }

        [Fact]
        public async Task RestoreDepartmentAsync_ShouldReturnNotFoundResponse_WhenDepartmentNotFoundOrNotDeleted()
        {
            // Arrange
            var departmentId = Guid.NewGuid();

            _mockRepository.Setup(r => r.RestoreAsync(departmentId))
                .ReturnsAsync(false);

            // Act
            var result = await _service.RestoreDepartmentAsync(departmentId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(404, result.StatusCode);
            Assert.Contains("not found", result.Message.ToLower());

            _mockRepository.Verify(r => r.RestoreAsync(departmentId), Times.Once);
        }
    }
}
