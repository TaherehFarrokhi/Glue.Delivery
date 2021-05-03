using AutoFixture;
using FluentAssertions;
using Glue.Delivery.Core.Domain;
using Glue.Delivery.Core.Rules;
using Xunit;

namespace Glue.Delivery.Core.UnitTests.Rules
{
    public class CancelStateRuleTests
    {
        private readonly Fixture _fixture = new();
        
        [Theory]
        [InlineData(DeliveryAction.Cancel, RequesterRole.User, true)]
        [InlineData(DeliveryAction.Cancel, RequesterRole.Partner, true)]        
        [InlineData(DeliveryAction.Complete, RequesterRole.User, false)]
        [InlineData(DeliveryAction.Complete, RequesterRole.Partner, false)]
        [InlineData(DeliveryAction.Approve, RequesterRole.User, false)]
        [InlineData(DeliveryAction.Approve, RequesterRole.Partner, false)]
        public void Should_CanApply_ReturnCorrectResult_WhenTheRightActionAndContextAreProvided(DeliveryAction action, RequesterRole role, bool expectedResult)
        {
            // Arrange
            var context = new ActionContext(_fixture.Create<OrderDelivery>(), role);
            var sut = new CancelStateRule();

            // Act
            var actual = sut.CanApply(action, context);

            // Assert
            actual.Should().Be(expectedResult);
        }
        
        [Theory]
        [InlineData(DeliveryState.Created, DeliveryAction.Cancel, RequesterRole.User, true, DeliveryState.Cancelled)]
        [InlineData(DeliveryState.Created, DeliveryAction.Cancel, RequesterRole.Partner, true, DeliveryState.Cancelled)]
        [InlineData(DeliveryState.Approved, DeliveryAction.Cancel, RequesterRole.User, true, DeliveryState.Cancelled)]
        [InlineData(DeliveryState.Approved, DeliveryAction.Cancel, RequesterRole.Partner, true, DeliveryState.Cancelled)]
        [InlineData(DeliveryState.Cancelled, DeliveryAction.Cancel, RequesterRole.Partner, false, null)]
        [InlineData(DeliveryState.Completed, DeliveryAction.Cancel, RequesterRole.Partner, false, null)]
        [InlineData(DeliveryState.Expired, DeliveryAction.Cancel, RequesterRole.Partner, false, null)]
        public void Should_Apply_ReturnCorrectResult_WhenTheRightActionAndContextAreProvided(
            DeliveryState current, DeliveryAction action, RequesterRole role, bool allowed, DeliveryState? target)
        {
            // Arrange
            var context = new ActionContext(_fixture.Create<OrderDelivery>(), role);
            context.OrderDelivery.State = current;
            
            var sut = new CancelStateRule();

            // Act
            var actual = sut.Apply(action, context);

            // Assert
            actual.Should().Be((allowed, target));
        }
    }
}