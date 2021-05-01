using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Glue.Delivery.Core.Domain;
using Glue.Delivery.Core.Handlers;
using Glue.Delivery.Core.Stores;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;
using Xunit;

namespace Glue.Delivery.Core.UnitTests
{
    public class ExpireDeliveryRequestHandlerTests
    {
        private readonly Fixture _fixture = new();

        [Theory]
        [InlineData(DeliveryState.Created)]
        [InlineData(DeliveryState.Approved)]
        public async Task Should_UpdateDeliveryStateToExpired_WhenTheDeliveryStateIsEligibleToExpired(DeliveryState state)
        {
            // Arrange
            var deliveries = _fixture.CreateMany<OrderDelivery>().ToList();
            deliveries.First().AccessWindow = new AccessWindow{ StartTime = DateTime.UtcNow.AddDays(-1), EndTime = DateTime.UtcNow};
            deliveries.First().State = state;
            
            var dbContext = new Mock<DeliveryDbContext>();
            dbContext.Setup(x => x.Deliveries).ReturnsDbSet(deliveries);
            var logger = new Mock<ILogger<ExpireDeliveryRequestHandler>>();
            var sut = new ExpireDeliveryRequestHandler(dbContext.Object, logger.Object);

            // Act
            await sut.Handle(new ExpireDeliveryRequest(), CancellationToken.None);

            // Assert
            deliveries.First().State.Should().Be(DeliveryState.Expired);
        }
        
        [Theory]
        [InlineData(DeliveryState.Created)]
        [InlineData(DeliveryState.Approved)]
        [InlineData(DeliveryState.Completed)]
        [InlineData(DeliveryState.Expired)]
        [InlineData(DeliveryState.Cancelled)]
        public async Task Should_UpdateDeliveryStateToExpired_WhenTheDeliveryIsNotExpired(DeliveryState state)
        {
            // Arrange
            var deliveries = _fixture.CreateMany<OrderDelivery>().ToList();
            deliveries.First().AccessWindow = new AccessWindow{ StartTime = DateTime.UtcNow.AddDays(-1), EndTime = DateTime.UtcNow.AddSeconds(1)};
            deliveries.First().State = state;
            
            var dbContext = new Mock<DeliveryDbContext>();
            dbContext.Setup(x => x.Deliveries).ReturnsDbSet(deliveries);
            var logger = new Mock<ILogger<ExpireDeliveryRequestHandler>>();
            var sut = new ExpireDeliveryRequestHandler(dbContext.Object, logger.Object);

            // Act
            await sut.Handle(new ExpireDeliveryRequest(), CancellationToken.None);

            // Assert
            deliveries.First().State.Should().Be(state);
        }
        
        [Theory]
        [InlineData(DeliveryState.Completed)]
        [InlineData(DeliveryState.Cancelled)]
        [InlineData(DeliveryState.Expired)]
        public async Task Should_NotUpdateDeliveryStateToExpired_WhenTheDeliveryStateIsEligibleToExpired(DeliveryState state)
        {
            // Arrange
            var deliveries = _fixture.CreateMany<OrderDelivery>().ToList();
            deliveries.First().AccessWindow = new AccessWindow{ StartTime = DateTime.UtcNow.AddDays(-1), EndTime = DateTime.UtcNow};
            deliveries.First().State = state;
            
            var dbContext = new Mock<DeliveryDbContext>();
            dbContext.Setup(x => x.Deliveries).ReturnsDbSet(deliveries);
            var logger = new Mock<ILogger<ExpireDeliveryRequestHandler>>();
            var sut = new ExpireDeliveryRequestHandler(dbContext.Object, logger.Object);

            // Act
            await sut.Handle(new ExpireDeliveryRequest(), CancellationToken.None);

            // Assert
            deliveries.First().State.Should().Be(state);
        }
    }
}