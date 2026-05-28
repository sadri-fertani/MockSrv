using AutoMapper;
using Microsoft.Extensions.Logging;
using MockSrv.Application.Mapper;
using MockSrv.Application.Services;
using MockSrv.Web.FuncTests.DbContextes;
using Moq;

namespace MockSrv.Web.FuncTests;

public class UnitTestMockService
{
    [Fact]
    public async Task RecupererAsync_ReturnNull_Async()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<MockRequestResponseService>>();
        var mockLoggerFactory = new Mock<ILoggerFactory>();
        mockLoggerFactory
            .Setup(f => f.CreateLogger(It.IsAny<string>()))
            .Returns(mockLogger.Object);

        var mapperConfiguration = new MapperConfiguration
            (
                cfg => { cfg.AddProfile<ApplicationProfile>(); },
                mockLoggerFactory.Object
            );        
        var mapper = mapperConfiguration.CreateMapper();
        var dbContext = DbContexteUtilitaire.Get();

        var service = new MockRequestResponseService
            (
            mockLogger.Object,
            mapper,
            dbContext
            );

        // Act
        var result = await service.GetAsync(0);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task RecupererAsync_ReturnMockRequest_Async()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<MockRequestResponseService>>();
        var mockLoggerFactory = new Mock<ILoggerFactory>();
        mockLoggerFactory
            .Setup(f => f.CreateLogger(It.IsAny<string>()))
            .Returns(mockLogger.Object);

        var mapperConfiguration = new MapperConfiguration
            (
                cfg => { cfg.AddProfile<ApplicationProfile>(); },
                mockLoggerFactory.Object
            );
        var mapper = mapperConfiguration.CreateMapper();
        var dbContext = DbContexteUtilitaire.Get();

        var service = new MockRequestResponseService
            (
            mockLogger.Object,
            mapper,
            dbContext
            );

        // Act
        var result = await service.GetAsync(1);

        // Assert
        Assert.Equal("/a/test", result.RequestPath);
        Assert.Equal("Get", result.RequestMethod);
        Assert.Equal("Default", result.ResponseBody);
        Assert.Equal(200, result.ResponseStatusCode);
        Assert.Equal("text/enriched", result.ResponseContentType);
        Assert.Equal("a", result.ApiName);
        Assert.Equal("test", result.Route);
    }
}
