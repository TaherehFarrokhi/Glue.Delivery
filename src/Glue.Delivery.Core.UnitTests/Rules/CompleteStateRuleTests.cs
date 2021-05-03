using AutoFixture;
using FluentAssertions;
using Glue.Delivery.Core.Domain;
using Glue.Delivery.Core.Rules;
using Xunit;

namespace Glue.Delivery.Core.UnitTests.Rules
{
    public class CompleteStateRuleTests
    {
        private readonly Fixture _fixture = new();
        
        [Theory]
        [InlineData(DeliveryAction.Complete, RequesterRole.User, true)]
        [InlineData(DeliveryAction.Complete, RequesterRole.Partner, true)]        
        [InlineData(DeliveryAction.Approve, RequesterRole.User, false)]
        [InlineData(DeliveryAction.Approve, RequesterRole.Partner, false)]
        [InlineData(DeliveryAction.Cancel, RequesterRole.User, false)]
        [InlineData(DeliveryAction.Cancel, RequesterRole.Partner, false)]
        public void Should_CanApply_ReturnCorrectResult_WhenTheRightActionAndContextAreProvided(DeliveryAction action, RequesterRole role, bool expectedResult)
        {
            // Arrange
            var context = new ActionContext(_fixture.Create<OrderDelivery>(), role);
            var sut = new CompleteStateRule();

            // Act
            var actual = sut.CanApply(action, context);

            // Assert
            actual.Should().Be(expectedResult);
        }
        
        [Theory]
        [InlineData(DeliveryState.Created, DeliveryAction.Complete, RequesterRole.User, false, null)]
        [InlineData(DeliveryState.Created, DeliveryAction.Complete, RequesterRole.Partner, false, null)]
        [InlineData(DeliveryState.Approved, DeliveryAction.Complete, RequesterRole.User, false, null)]
        [InlineData(DeliveryState.Approved, DeliveryAction.Complete, RequesterRole.Partner, true, DeliveryState.Completed)]
        [InlineData(DeliveryState.Cancelled, DeliveryAction.Complete, RequesterRole.Partner, false, null)]
        [InlineData(DeliveryState.Completed, DeliveryAction.Complete, RequesterRole.Partner, false, null)]
        [InlineData(DeliveryState.Expired, DeliveryAction.Complete, RequesterRole.Partner, false, null)]
        public void Should_Apply_ReturnCorrectResult_WhenTheRightActionAndContextAreProvided(
            DeliveryState current, DeliveryAction action, RequesterRole role, bool allowed, DeliveryState? target)
        {
            // Arrange
            var context = new ActionContext(_fixture.Create<OrderDelivery>(), role);
            context.OrderDelivery.State = current;
            
            var sut = new CompleteStateRule();

            // Act
            var actual = sut.Apply(action, context);

            // Assert
            actual.Should().Be((allowed, target));
        }
    }
}