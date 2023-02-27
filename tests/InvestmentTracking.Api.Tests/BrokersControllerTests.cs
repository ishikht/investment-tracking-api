using InvestmentTracking.Api.Controllers;
using InvestmentTracking.Core.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using InvestmentTracking.Core.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace InvestmentTracking.Api.Tests
{
    public class BrokersControllerTests
    {
        private readonly Mock<IBrokerService> _brokerServiceMock;
        private readonly Mock<ILogger<BrokersController>> _loggerMock;
        private readonly BrokersController _controller;

        public BrokersControllerTests()
        {
            _brokerServiceMock = new Mock<IBrokerService>();
            _loggerMock = new Mock<ILogger<BrokersController>>();
            _controller = new BrokersController(_brokerServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateBroker_ReturnsCreatedBroker()
        {
            // Arrange
            var brokerDto = new BrokerDto
            {
                Id = Guid.NewGuid(),
                Name = "Test Broker"
            };

            _brokerServiceMock.Setup(x => x.AddBrokerAsync(It.IsAny<BrokerDto>()))
                .ReturnsAsync(brokerDto);

            // Act
            var result = await _controller.CreateBroker(brokerDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<BrokerDto>(createdAtActionResult.Value);
            Assert.Equal(brokerDto.Id, returnValue.Id);
            Assert.Equal(brokerDto.Name, returnValue.Name);
            Assert.Equal(StatusCodes.Status201Created, createdAtActionResult.StatusCode);

            _brokerServiceMock.Verify(x => x.AddBrokerAsync(It.IsAny<BrokerDto>()), Times.Once);
        }

        [Fact]
        public async Task CreateBroker_ReturnsBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            var brokerServiceMock = new Mock<IBrokerService>();
            var loggerMock = new Mock<ILogger<BrokersController>>();
            brokerServiceMock.Setup(x => x.AddBrokerAsync(It.IsAny<BrokerDto>())).ThrowsAsync(new Exception());
            var controller = new BrokersController(brokerServiceMock.Object, loggerMock.Object);
            var brokerDto = new BrokerDto { Id = Guid.NewGuid(), Name = "Test Broker" };

            // Act
            var result = await controller.CreateBroker(brokerDto);

            // Assert
            var actionResult = Assert.IsType<BadRequestResult>(result);
            brokerServiceMock.Verify(x => x.AddBrokerAsync(brokerDto), Times.Once);
        }

        [Fact]
        public async Task GetBrokers_ReturnsListOfBrokers()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<BrokersController>>();
            var mockBrokerService = new Mock<IBrokerService>();
            var brokers = new List<BrokerDto> { new BrokerDto { Id = Guid.NewGuid(), Name = "Broker 1" }, new BrokerDto { Id = Guid.NewGuid(), Name = "Broker 2" } };
            mockBrokerService.Setup(service => service.GetAllBrokersAsync()).Returns(brokers.ToAsyncEnumerable());
            var controller = new BrokersController(mockBrokerService.Object, mockLogger.Object);

            // Act
            var response = await controller.GetBrokers();

            // Assert
            var result = Assert.IsType<OkObjectResult>(response.Result);
            var resultValue = Assert.IsAssignableFrom<IEnumerable<BrokerDto>>(result.Value);
            Assert.Equal(2, resultValue.Count());
        }

        [Fact]
        public async Task GetBrokers_ReturnsBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            var mockBrokerService = new Mock<IBrokerService>();
            mockBrokerService.Setup(service => service.GetAllBrokersAsync()).Throws(new Exception());
            var controller = new BrokersController(mockBrokerService.Object, NullLogger<BrokersController>.Instance);

            // Act
            var result = await controller.GetBrokers();

            // Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public async Task GetBroker_ReturnsBroker_WithValidId()
        {
            // Arrange
            var expectedBroker = new BrokerDto
            {
                Id = Guid.NewGuid(),
                Name = "Test Broker"
            };

            var mockBrokerService = new Mock<IBrokerService>();
            mockBrokerService.Setup(service => service.GetBrokerByIdAsync(expectedBroker.Id)).ReturnsAsync(expectedBroker);

            var controller = new BrokersController(mockBrokerService.Object, NullLogger<BrokersController>.Instance);

            // Act
            var result = await controller.GetBroker(expectedBroker.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualBroker = Assert.IsType<BrokerDto>(okResult.Value);
            Assert.Equal(expectedBroker.Id, actualBroker.Id);
            Assert.Equal(expectedBroker.Name, actualBroker.Name);
        }

        [Fact]
        public async Task GetBroker_ReturnsNotFound_WithInvalidId()
        {
            // Arrange
            var mockBrokerService = new Mock<IBrokerService>();
            var controller = new BrokersController(mockBrokerService.Object, NullLogger<BrokersController>.Instance);

            // Act
            var result = await controller.GetBroker(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetBroker_ReturnsBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            var mockBrokerService = new Mock<IBrokerService>();
            mockBrokerService.Setup(service => service.GetBrokerByIdAsync(It.IsAny<Guid>())).Throws(new Exception());
            var controller = new BrokersController(mockBrokerService.Object, NullLogger<BrokersController>.Instance);

            // Act
            var result = await controller.GetBroker(Guid.NewGuid());

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UpdateBroker_ReturnsOk_WithValidId()
        {
            // Arrange
            var brokerId = Guid.NewGuid();
            var broker = new BrokerUpdateDto { Name = "Test Broker" };
            var mockBrokerService = new Mock<IBrokerService>();
            mockBrokerService.Setup(service => service.UpdateBrokerAsync(It.IsAny<Guid>(),It.IsAny<BrokerUpdateDto>())).Returns(Task.CompletedTask);
            var controller = new BrokersController(mockBrokerService.Object, NullLogger<BrokersController>.Instance);

            // Act
            var result = await controller.UpdateBroker(brokerId, broker);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task UpdateBroker_ReturnsBadRequest_WithInvalidId()
        {
            // Arrange
            var mockBrokerService = new Mock<IBrokerService>();
            mockBrokerService.Setup(service => service.UpdateBrokerAsync(It.IsAny<Guid>(),It.IsAny<BrokerUpdateDto>())).Throws<KeyNotFoundException>();
            var controller = new BrokersController(mockBrokerService.Object, NullLogger<BrokersController>.Instance);
            var broker = new BrokerUpdateDto { Name = "Test Broker" };

            // Act
            var result = await controller.UpdateBroker(Guid.Empty, broker);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteBroker_ReturnsNoContent_WithValidId()
        {
            // Arrange
            var mockBrokerService = new Mock<IBrokerService>();
            var brokerId = Guid.NewGuid();
            mockBrokerService.Setup(service => service.DeleteBrokerAsync(brokerId)).Returns(Task.CompletedTask);
            var controller = new BrokersController(mockBrokerService.Object, NullLogger<BrokersController>.Instance);

            // Act
            var result = await controller.DeleteBroker(brokerId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteBroker_ReturnsNotFound_WithInvalidId()
        {
            // Arrange
            var brokerId = Guid.NewGuid();
            var mockBrokerService = new Mock<IBrokerService>();
            mockBrokerService.Setup(service => service.DeleteBrokerAsync(brokerId)).Throws(new KeyNotFoundException());
            var controller = new BrokersController(mockBrokerService.Object, NullLogger<BrokersController>.Instance);

            // Act
            var result = await controller.DeleteBroker(brokerId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteBroker_ReturnsNotFound_WhenServiceThrowsException()
        {
            // Arrange
            var brokerId = Guid.NewGuid();
            var mockBrokerService = new Mock<IBrokerService>();
            mockBrokerService.Setup(service => service.DeleteBrokerAsync(brokerId)).Throws(new Exception());
            var controller = new BrokersController(mockBrokerService.Object, NullLogger<BrokersController>.Instance);

            // Act
            var result = await controller.DeleteBroker(brokerId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }



    }
}
