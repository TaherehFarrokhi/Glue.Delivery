using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace Glue.Delivery.Core.UnitTests
{
    public class GetDeliveryRequestHandlerTests
    {
        private readonly Fixture _fixture = new Fixture();
        
        [Fact]
        public async Task Should_Handle_ReturnTheDelivery_WhenTheDeliveryIdIsValid()
        {
            // Arrange
            var sut = new GetDeliveryRequestHandler(null, null, null);

            // Act
            var actual = await sut.Handle(new GetDeliveryRequest(Guid.NewGuid()), CancellationToken.None);
            
            // Assert
            actual.Succeed.Should().BeTrue();

        }        
    }
}