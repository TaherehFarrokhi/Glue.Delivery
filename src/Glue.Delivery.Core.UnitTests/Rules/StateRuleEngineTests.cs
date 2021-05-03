using AutoFixture;
using FluentAssertions;
using Glue.Delivery.Core.Domain;
using Glue.Delivery.Core.Rules;
using Moq;
using Xunit;

namespace Glue.Delivery.Core.UnitTests.Rules
{
    public class StateRuleEngineTests
    {
        private readonly Fixture _fixture = new();
        
        [Theory]
        [InlineData(DeliveryAction.Approve, RequesterRole.User, true, DeliveryState.Approved)]
        [InlineData(DeliveryAction.Complete, RequesterRole.User, false, null)]
        public void Should_ApplyAction_ReturnCorrectResult_WhenTheRightActionAndContextAreProvided(
            DeliveryAction action, RequesterRole role, bool expectedResult, DeliveryState? target)
        {
            // Arrange
            var context = new ActionContext(_fixture.Create<OrderDelivery>(), role);
            var rule = new Mock<IStateRule>();
            rule.Setup(x => x.CanApply(DeliveryAction.Approve, It.IsAny<ActionContext>())).Returns(true);
            rule.Setup(x => x.Apply(DeliveryAction.Approve, It.IsAny<ActionContext>())).Returns((true, DeliveryState.Approved));
            var sut = new StateRuleEngine(new []{rule.Object});

            // Act
            var actual = sut.ApplyAction(action, context);

            // Assert
            actual.Should().Be((expectedResult, target));
        }
    }
}