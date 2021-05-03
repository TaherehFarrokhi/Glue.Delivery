using Glue.Delivery.Core.Domain;

namespace Glue.Delivery.Core.Rules
{
    public interface IStateRuleEngine
    {
        (bool Allowed, DeliveryState? Target) ApplyAction(DeliveryAction action, ActionContext context);
    }
}