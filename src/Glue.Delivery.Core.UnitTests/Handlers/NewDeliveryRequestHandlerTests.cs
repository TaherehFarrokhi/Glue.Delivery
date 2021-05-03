using System;
using System.Collections.Generic;
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
    public class NewDeliveryRequestHandlerTests
    {
        private readonly Fixture _fixture = new();

        [Fact]
        public async Task Should_Handle_ReturnTheSuccessAndCreatedDelivery_WhenTheDeliveryIsValid()
        {
            // Arrange
            var newDeliveryRequest = _fixture.Create<NewDeliveryRequest>();
            var dbContext = new Mock<DeliveryDbContext>();
            dbContext.Setup(x => x.Deliveries).ReturnsDbSet(new List<OrderDelivery>());
            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<OrderDeliveryDto>(It.IsAny<OrderDelivery>())).Returns(new OrderDeliveryDto());
            var logger = new Mock<ILogger<NewDeliveryRequestHandler>>();
            var sut = new NewDeliveryRequestHandler(dbContext.Object, mapper.Object, logger.Object);

            // Act
            var actual = await sut.Handle(newDeliveryRequest, CancellationToken.None);

            // Assert
            actual.Succeed.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Handle_ReturnError_WhenExceptionHappened()
        {
            // Arrange
            var newDeliveryRequest = _fixture.Create<NewDeliveryRequest>();
            var dbContext = new Mock<DeliveryDbContext>();
            dbContext.Setup(x => x.Deliveries).Throws(new Exception());
            var mapper = new Mock<IMapper>();
            var logger = new Mock<ILogger<NewDeliveryRequestHandler>>();
            var sut = new NewDeliveryRequestHandler(dbContext.Object, mapper.Object, logger.Object);

            // Act
            var actual = await sut.Handle(newDeliveryRequest, CancellationToken.None);

            // Assert
            actual.Failed.Should().BeTrue();
            actual.ErrorReason.Should().Be(OperationErrorReason.GenericError);
        }
    }
}