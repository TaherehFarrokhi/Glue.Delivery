using Glue.Delivery.Core.Domain;

namespace Glue.Delivery.Core.Rules
{
    public interface IStateRule
    {
        bool CanApply(DeliveryAction action, ActionContext context);
        (bool Allowed, DeliveryState? Target) Apply(DeliveryAction action, ActionContext context);
    }
}