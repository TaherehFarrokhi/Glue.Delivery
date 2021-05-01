using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Glue.Delivery.Core.Dto;
using Xunit;

namespace Glue.Delivery.Core.UnitTests
{
    public class NewDeliveryRequestHandlerTests
    {
        private readonly Fixture _fixture = new Fixture();
        
        [Fact]
        public async Task Should_Handle_ReturnTheDelivery_WhenTheDeliveryIdIsValid()
        {
            // Arrange
            var delivery = _fixture.Create<DeliveryRequestDto>();
            var sut = new NewDeliveryRequestHandler(null, null, null);

            // Act
            var actual = await sut.Handle(new NewDeliveryRequest(delivery), CancellationToken.None);
            
            // Assert
            actual.Succeed.Should().BeTrue();
        }        
    }
}