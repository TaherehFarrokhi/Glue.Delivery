using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Glue.Delivery.Core.Domain;
using Glue.Delivery.Core.Dto;
using Glue.Delivery.Core.Handlers;
using Glue.Delivery.Core.Stores;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;
using Xunit;

namespace Glue.Delivery.Core.UnitTests.Handlers
{
    public class GetDeliveryRequestHandlerTests
    {
        private readonly Fixture _fixture = new();

        [Fact]
        public async Task Should_Handle_ReturnTheDelivery_WhenTheDeliveryIdIsExist()
        {
            // Arrange
            var deliveries = _fixture.CreateMany<OrderDelivery>().ToList();
            var dbContext = new Mock<DeliveryDbContext>();
            dbContext.Setup(x => x.Deliveries).ReturnsDbSet(deliveries);
            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<OrderDeliveryDto>(deliveries.First())).Returns(new OrderDeliveryDto());
            var logger = new Mock<ILogger<NewDeliveryRequestHandler>>();
            var sut = new GetDeliveryRequestHandler(dbContext.Object, mapper.Object, logger.Object);

            // Act
            var actual = await sut.Handle(new GetDeliveryRequest(deliveries.First().DeliveryId),
                CancellationToken.None);

            // Assert
            actual.Succeed.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Handle_ReturnErrorWithResourceNotFoundReason_WhenTheDeliveryIdIsNotExist()
        {
            // Arrange
            var deliveries = _fixture.CreateMany<OrderDelivery>();
            var dbContext = new Mock<DeliveryDbContext>();
            dbContext.Setup(x => x.Deliveries).ReturnsDbSet(deliveries);
            var mapper = new Mock<IMapper>();
            var logger = new Mock<ILogger<NewDeliveryRequestHandler>>();
            var sut = new GetDeliveryRequestHandler(dbContext.Object, mapper.Object, logger.Object);

            // Act
            var actual = await sut.Handle(new GetDeliveryRequest(new Guid()), CancellationToken.None);

            // Assert
            actual.Failed.Should().BeTrue();
            actual.ErrorReason.Should().Be(OperationErrorReason.ResourceNotFound);
        }

        [Fact]
        public async Task Should_Handle_ReturnErrorWithGenericErrorReason_WhenExceptionHappened()
        {
            // Arrange
            var dbContext = new Mock<DeliveryDbContext>();
            dbContext.Setup(x => x.Deliveries).Throws(new Exception());
            var mapper = new Mock<IMapper>();
            var logger = new Mock<ILogger<NewDeliveryRequestHandler>>();
            var sut = new GetDeliveryRequestHandler(dbContext.Object, mapper.Object, logger.Object);

            // Act
            var actual = await sut.Handle(new GetDeliveryRequest(new Guid()), CancellationToken.None);

            // Assert
            actual.Failed.Should().BeTrue();
            actual.ErrorReason.Should().Be(OperationErrorReason.GenericError);
        }
    }
}