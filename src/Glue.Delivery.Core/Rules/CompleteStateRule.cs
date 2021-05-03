using Glue.Delivery.Core.Domain;

namespace Glue.Delivery.Core.Rules
{
    public sealed class CompleteStateRule : IStateRule
    {
        public bool CanApply(DeliveryAction action, ActionContext context) => action == DeliveryAction.Complete; 

        public (bool Allowed, DeliveryState? Target) Apply(DeliveryAction action, ActionContext context)
        {
            var allowed = action == DeliveryAction.Complete &&
                          context.OrderDelivery.State == DeliveryState.Approved &&
                          context.RequesterRole == RequesterRole.Partner;
            return (allowed, DeliveryState.Completed);
        }
    }
}