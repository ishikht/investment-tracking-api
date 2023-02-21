using AutoMapper;
using InvestmentTracking.Core.Data;
using InvestmentTracking.Core.Dtos;
using InvestmentTracking.Core.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace InvestmentTracking.Services.Tests;

[Collection("AutoMapper")]
public class BrokerServiceTests
{
    private readonly Mock<ILogger<BrokerService>> _loggerMock;
    private readonly IMapper _mapper;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public BrokerServiceTests(AutoMapperFixture fixture)
    {
        _mapper = fixture.Mapper;

        _unitOfWorkMock = new Mock<IUnitOfWork>();
        var brokerRepositoryMock = new Mock<IBrokerRepository>();
        _unitOfWorkMock.Setup(uow => uow.BrokerRepository).Returns(brokerRepositoryMock.Object);

        _loggerMock = new Mock<ILogger<BrokerService>>();
    }

    [Fact]
    public async Task AddBrokerAsync_Should_AddNewBrokerToDatabase()
    {
        // Arrange
        var brokerDto = new BrokerDto
        {
            Id = Guid.NewGuid(),
            Name = "Test Broker"
        };

        Broker capturedBroker = null;

        var brokerRepositoryMock = new Mock<IBrokerRepository>();
        brokerRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Broker>()))
            .Returns((Broker b) =>
            {
                capturedBroker = b;
                capturedBroker.Id = b.Id;
                return Task.FromResult(capturedBroker);
            });

        _unitOfWorkMock.Setup(x => x.BrokerRepository).Returns(brokerRepositoryMock.Object);

        var brokerService = new BrokerService(_unitOfWorkMock.Object, _mapper, _loggerMock.Object);

        // Act
        var result = await brokerService.AddBrokerAsync(brokerDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(brokerDto.Id, result.Id);
        Assert.Equal(brokerDto.Name, result.Name);

        _unitOfWorkMock.Verify(x => x.BrokerRepository.AddAsync(It.IsAny<Broker>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        Assert.NotNull(capturedBroker);
        Assert.Equal(brokerDto.Name, capturedBroker.Name);
    }

    [Fact]
    public async Task GetAllBrokersAsync_ReturnsAllBrokers()
    {
        // Arrange
        var brokers = new List<Broker>
        {
            new() {Id = Guid.NewGuid(), Name = "Broker 1"},
            new() {Id = Guid.NewGuid(), Name = "Broker 2"}
        };

        var brokerRepositoryMock = new Mock<IBrokerRepository>();
        brokerRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(brokers);
        _unitOfWorkMock.Setup(x => x.BrokerRepository).Returns(brokerRepositoryMock.Object);
        var service = new BrokerService(_unitOfWorkMock.Object, _mapper, _loggerMock.Object);

        // Act
        var result = await service.GetAllBrokersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Collection(result,
            item => Assert.Equal(brokers[0].Id, item.Id),
            item => Assert.Equal(brokers[1].Id, item.Id)
        );
    }

    [Fact]
    public async Task GetBrokerByIdAsync_ReturnsBroker_IfExists()
    {
        // Arrange
        var brokerId = Guid.NewGuid();
        var broker = new Broker {Id = brokerId, Name = "Test Broker"};

        var brokerRepositoryMock = new Mock<IBrokerRepository>();
        brokerRepositoryMock.Setup(x => x.GetAsync(brokerId)).ReturnsAsync(broker);
        _unitOfWorkMock.Setup(x => x.BrokerRepository).Returns(brokerRepositoryMock.Object);
        var service = new BrokerService(_unitOfWorkMock.Object, _mapper, _loggerMock.Object);

        // Act
        var result = await service.GetBrokerByIdAsync(brokerId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(brokerId, result.Id);
        Assert.Equal(broker.Name, result.Name);
    }

    [Fact]
    public async Task UpdateBrokerAsync_Should_UpdateBrokerInDatabase()
    {
        // Arrange
        var brokerId = Guid.NewGuid();
        var brokerDto = new BrokerDto { Id = brokerId, Name = "Test Broker" };

        var broker = new Broker { Id = brokerId, Name = "Old Name" };
        var brokerRepositoryMock = new Mock<IBrokerRepository>();
        brokerRepositoryMock.Setup(x => x.GetAsync(brokerId)).ReturnsAsync(broker);
        _unitOfWorkMock.Setup(x => x.BrokerRepository).Returns(brokerRepositoryMock.Object);
        var service = new BrokerService(_unitOfWorkMock.Object, _mapper, _loggerMock.Object);

        // Act
        await service.UpdateBrokerAsync(brokerDto);

        // Assert
        brokerRepositoryMock.Verify(x => x.UpdateAsync(broker), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        Assert.Equal(brokerDto.Name, broker.Name);
    }

    [Fact]
    public async Task DeleteBrokerAsync_Should_DeleteBrokerFromDatabase()
    {
        // Arrange
        var brokerId = Guid.NewGuid();
        var broker = new Broker { Id = brokerId, Name = "Test Broker" };

        var brokerRepositoryMock = new Mock<IBrokerRepository>();
        brokerRepositoryMock.Setup(x => x.GetAsync(brokerId)).ReturnsAsync(broker);
        _unitOfWorkMock.Setup(x => x.BrokerRepository).Returns(brokerRepositoryMock.Object);
        var service = new BrokerService(_unitOfWorkMock.Object, _mapper, _loggerMock.Object);

        // Act
        await service.DeleteBrokerAsync(brokerId);

        // Assert
        brokerRepositoryMock.Verify(x => x.DeleteAsync(broker), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }


}