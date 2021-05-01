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

namespace Glue.Delivery.Core.UnitTests
{
    public class UpdateDeliveryRequestHandlerTests
    {
        private readonly Fixture _fixture = new();

        [Fact]
        public async Task Should_Handle_ReturnTheSuccess_WhenTheDeliveryIdIsExist()
        {
            // Arrange
            var deliveries = _fixture.CreateMany<OrderDelivery>().ToList();
            var deliveryRequestDto = _fixture.Create<DeliveryRequestDto>();
            var dbContext = new Mock<DeliveryDbContext>();
            dbContext.Setup(x => x.Deliveries).ReturnsDbSet(deliveries);
            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<OrderDeliveryDto>(It.IsAny<OrderDelivery>())).Returns(new OrderDeliveryDto());
            var logger = new Mock<ILogger<UpdateDeliveryRequestHandler>>();
            var sut = new UpdateDeliveryRequestHandler(dbContext.Object, mapper.Object, logger.Object);

            // Act
            var actual = await sut.Handle(new UpdateDeliveryRequest(deliveries.First().DeliveryId, deliveryRequestDto),
                CancellationToken.None);

            // Assert
            actual.Succeed.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Handle_ReturnErrorWithResourceNotFoundReason_WhenTheDeliveryIdIsNotExist()
        {
            // Arrange
            var deliveries = _fixture.CreateMany<OrderDelivery>();
            var deliveryRequestDto = _fixture.Create<DeliveryRequestDto>();
            var dbContext = new Mock<DeliveryDbContext>();
            dbContext.Setup(x => x.Deliveries).ReturnsDbSet(deliveries);
            var mapper = new Mock<IMapper>();
            var logger = new Mock<ILogger<UpdateDeliveryRequestHandler>>();
            var sut = new UpdateDeliveryRequestHandler(dbContext.Object, mapper.Object, logger.Object);

            // Act
            var actual = await sut.Handle(new UpdateDeliveryRequest(Guid.NewGuid(), deliveryRequestDto),
                CancellationToken.None);

            // Assert
            actual.Failed.Should().BeTrue();
            actual.ErrorReason.Should().Be(OperationErrorReason.ResourceNotFound);
        }

        [Fact]
        public async Task Should_Handle_ReturnErrorWithGenericErrorReason_WhenExceptionHappened()
        {
            // Arrange
            var deliveryRequestDto = _fixture.Create<DeliveryRequestDto>();
            var dbContext = new Mock<DeliveryDbContext>();
            dbContext.Setup(x => x.Deliveries).Throws(new Exception());
            var mapper = new Mock<IMapper>();
            var logger = new Mock<ILogger<UpdateDeliveryRequestHandler>>();
            var sut = new UpdateDeliveryRequestHandler(dbContext.Object, mapper.Object, logger.Object);

            // Act
            var actual = await sut.Handle(new UpdateDeliveryRequest(Guid.NewGuid(), deliveryRequestDto),
                CancellationToken.None);

            // Assert
            actual.Failed.Should().BeTrue();
            actual.ErrorReason.Should().Be(OperationErrorReason.GenericError);
        }
    }
}