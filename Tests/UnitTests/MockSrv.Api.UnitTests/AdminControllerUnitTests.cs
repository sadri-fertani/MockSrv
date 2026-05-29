using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockSrv.Api.Controllers;
using MockSrv.Application.DTOs;
using MockSrv.Application.Interfaces.Services;
using MockSrv.Common.Globals;
using MockSrv.Common.Logging;
using Moq;

namespace MockSrv.Api.UnitTests;

public class AdminControllerUnitTests
{
    #region GetAll
    [Fact]
    public async Task Test_GetAll_Return_List_Async()
    {
        // Arrange
        var mockLogger = new Mock<ISanitizedLogger<AdminController>>();
        var mockRequestService = new Mock<IMockRequestResponseService>();

        mockRequestService
            .Setup(r => r.GetAsync())
            .ReturnsAsync
            (
                [
                    new MockRequestResponseDto(){ },
                    new MockRequestResponseDto(){ }
                ]
            );


        var controller = new AdminController(
            mockLogger.Object,
            mockRequestService.Object);

        // Act
        var result = await controller.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Value!.Count);
    }

    [Fact]
    public async Task Test_GetAll_Return_NotFound_Async()
    {
        // Arrange
        var mockLogger = new Mock<ISanitizedLogger<AdminController>>();
        var mockRequestService = new Mock<IMockRequestResponseService>();

        mockRequestService
            .Setup(r => r.GetAsync())
            .ReturnsAsync
            (
                value: null!
            );

        var controller = new AdminController(
            mockLogger.Object,
            mockRequestService.Object);

        // Act
        var result = await controller.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Test_GetAll_Return_ThrowException_Async()
    {
        // Arrange
        var mockLogger = new Mock<ISanitizedLogger<AdminController>>();
        var mockRequestService = new Mock<IMockRequestResponseService>();

        mockRequestService
            .Setup(r => r.GetAsync())
            .Throws(new Exception("..."));


        var controller = new AdminController(
            mockLogger.Object,
            mockRequestService.Object);

        // Act
        var result = await controller.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
    }
    #endregion

    #region GetOne
    [Fact]
    public async Task Test_GetOne_Return_OK_Async()
    {
        // Arrange
        var mockLogger = new Mock<ISanitizedLogger<AdminController>>();
        var mockRequestService = new Mock<IMockRequestResponseService>();

        var expected = new MockRequestResponseDto
        {
            Id = 1,
            ApiName = "Test",
            RequestBody = "",
            RequestMethod = "Get",
            RequestPath = "/",
            ResponseBody = "",
            ResponseContentType = "application/json",
            ResponseStatusCode = 200,
            RequestQueryString = "",
            Route = ""
        };

        mockRequestService
            .Setup(r => r.GetAsync(It.IsAny<int>()))
            .ReturnsAsync
            (
                expected
            );


        var controller = new AdminController(
            mockLogger.Object,
            mockRequestService.Object);

        // Act
        var result = await controller.GetOneAsync(expected.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected.Id, result.Value!.Id);
    }

    [Fact]
    public async Task Test_GetOne_Return_NotFound_Async()
    {
        // Arrange
        var mockLogger = new Mock<ISanitizedLogger<AdminController>>();
        var mockRequestService = new Mock<IMockRequestResponseService>();

        mockRequestService
            .Setup(r => r.GetAsync(It.IsAny<int>()))
            .ReturnsAsync
            (
                value: null!
            );

        var controller = new AdminController(
            mockLogger.Object,
            mockRequestService.Object);

        // Act
        var result = await controller.GetOneAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Test_GetOne_Return_ThrowException_Async()
    {
        // Arrange
        var mockLogger = new Mock<ISanitizedLogger<AdminController>>();
        var mockRequestService = new Mock<IMockRequestResponseService>();

        mockRequestService
            .Setup(r => r.GetAsync(It.IsAny<int>()))
            .Throws(new Exception("..."));


        var controller = new AdminController(
            mockLogger.Object,
            mockRequestService.Object);

        // Act
        var result = await controller.GetOneAsync(1);

        // Assert
        Assert.NotNull(result);
        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
    }
    #endregion

    #region Post
    [Fact]
    public async Task Test_Post_Return_New_Async()
    {
        // Arrange
        var mockLogger = new Mock<ISanitizedLogger<AdminController>>();
        var mockRequestService = new Mock<IMockRequestResponseService>();

        var expectedIn = new MockRequestResponseDto
        {
            Id = 0,
            ApiName = "Test",
            RequestBody = "",
            RequestMethod = "Get",
            RequestPath = "/",
            ResponseBody = "",
            ResponseContentType = "application/json",
            ResponseStatusCode = 200,
            RequestQueryString = "",
            Route = ""
        };

        var expectedOut = new MockRequestResponseDto
        {
            Id = 1,
            ApiName = "Test",
            RequestBody = "",
            RequestMethod = "Get",
            RequestPath = "/",
            ResponseBody = "",
            ResponseContentType = "application/json",
            ResponseStatusCode = 200,
            RequestQueryString = "",
            Route = ""
        };

        mockRequestService
            .Setup(r => r.AddAsync(It.IsAny<MockRequestResponseDto>())).ReturnsAsync(expectedOut);

        var controller = new AdminController(
            mockLogger.Object,
            mockRequestService.Object);

        // Act
        var result = await controller.PostAsync(expectedIn);

        Assert.NotNull(result);

        Assert.NotEqual(0, ((result.Result! as CreatedAtActionResult)!.Value as MockRequestResponseDto)!.Id);
    }

    [Fact]
    public async Task Test_Post_ThrowException_Async()
    {
        // Arrange
        var mockLogger = new Mock<ISanitizedLogger<AdminController>>();
        var mockRequestService = new Mock<IMockRequestResponseService>();

        var expected = new MockRequestResponseDto
        {
            Id = 1,
            ApiName = "Test",
            RequestBody = "",
            RequestMethod = "Get",
            RequestPath = "/",
            ResponseBody = "",
            ResponseContentType = "application/json",
            ResponseStatusCode = 200,
            RequestQueryString = "",
            Route = ""
        };

        mockRequestService
            .Setup(r => r.AddAsync(It.IsAny<MockRequestResponseDto>())).Throws(new Exception("..."));

        var controller = new AdminController(
            mockLogger.Object,
            mockRequestService.Object);

        // Act
        var result = await controller.PostAsync(expected);

        // Assert
        Assert.NotNull(result);
        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);

    }

    [Fact]
    public async Task Test_Post_ThrowSqliteExceptionAsInnerException_Async()
    {
        // Arrange
        var mockLogger = new Mock<ISanitizedLogger<AdminController>>();
        var mockRequestService = new Mock<IMockRequestResponseService>();

        var expected = new MockRequestResponseDto
        {
            Id = 1,
            ApiName = "Test",
            RequestBody = "",
            RequestMethod = "Get",
            RequestPath = "/",
            ResponseBody = "",
            ResponseContentType = "application/json",
            ResponseStatusCode = 200,
            RequestQueryString = "",
            Route = ""
        };

        var expectedException = new DbUpdateException
            (
                "Duplicate key",
                new SqliteException("...", ErrorCodes.CODE_DUPLICATE_KEY_SQLLITE)
            );

        mockRequestService
            .Setup(r => r.AddAsync(It.IsAny<MockRequestResponseDto>())).Throws(expectedException);

        var controller = new AdminController(
            mockLogger.Object,
            mockRequestService.Object);

        // Act
        var result = await controller.PostAsync(expected);

        // Assert
        Assert.NotNull(result);
        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
    }
    #endregion

    #region Put
    [Fact]
    public async Task Test_Put_Return_Notfound_Async()
    {
        // Arrange
        var mockLogger = new Mock<ISanitizedLogger<AdminController>>();
        var mockRequestService = new Mock<IMockRequestResponseService>();

        mockRequestService
            .Setup(r => r.GetAsync(It.IsAny<int>())).ReturnsAsync((MockRequestResponseDto)null!);

        var controller = new AdminController(
            mockLogger.Object,
            mockRequestService.Object);

        // Act
        var result = await controller.PutAsync(new MockRequestResponseDto { });

        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status404NotFound, ((ObjectResult)result!.Result!).StatusCode);
    }

    [Fact]
    public async Task Test_Put_ThrowException_Async()
    {
        // Arrange
        var mockLogger = new Mock<ISanitizedLogger<AdminController>>();
        var mockRequestService = new Mock<IMockRequestResponseService>();

        var expected = new MockRequestResponseDto
        {
            Id = 1,
            ApiName = "Test",
            RequestBody = "",
            RequestMethod = "Get",
            RequestPath = "/",
            ResponseBody = "",
            ResponseContentType = "application/json",
            ResponseStatusCode = 200,
            RequestQueryString = "",
            Route = ""
        };

        mockRequestService
            .Setup(r => r.GetAsync(It.IsAny<int>())).ReturnsAsync(expected);
        mockRequestService
            .Setup(r => r.UpdateAsync(It.IsAny<MockRequestResponseDto>())).Throws(new Exception("..."));

        var controller = new AdminController(
            mockLogger.Object,
            mockRequestService.Object);

        // Act
        var result = await controller.PutAsync(expected);

        // Assert
        Assert.NotNull(result);
        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);

    }
    #endregion

    #region Delete
    [Fact]
    public async Task Test_Delete_Return_NoContent_Async()
    {
        // Arrange
        var mockLogger = new Mock<ISanitizedLogger<AdminController>>();
        var mockRequestService = new Mock<IMockRequestResponseService>();

        mockRequestService
            .Setup(r => r.DeleteAsync(It.IsAny<int>()))
            .Verifiable();

        var controller = new AdminController(
            mockLogger.Object,
            mockRequestService.Object);

        // Act
        var result = await controller.DeleteAsync(1001);

        // Assert
        Assert.Equal(StatusCodes.Status204NoContent, ((StatusCodeResult)result).StatusCode);
    }

    [Fact]
    public async Task Test_Delete_Return_ThrowException_Async()
    {
        // Arrange
        var mockLogger = new Mock<ISanitizedLogger<AdminController>>();
        var mockRequestService = new Mock<IMockRequestResponseService>();

        mockRequestService
            .Setup(r => r.GetAsync(It.IsAny<int>()))
            .Throws(new Exception("..."));

        var controller = new AdminController(
            mockLogger.Object,
            mockRequestService.Object);

        // Act
        var result = await controller.DeleteAsync(1001);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, ((ObjectResult)result).StatusCode);
    }
    #endregion
}