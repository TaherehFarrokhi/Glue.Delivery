using Glue.Delivery.Core.Domain;

namespace Glue.Delivery.Core.Rules
{
    public sealed class ApproveStateRule : IStateRule
    {
        public bool CanApply(DeliveryAction action, ActionContext context) => action == DeliveryAction.Approve; 

        public (bool Allowed, DeliveryState? Target) Apply(DeliveryAction action, ActionContext context)
        {
            var allowed = action == DeliveryAction.Approve &&
                          context.OrderDelivery.State == DeliveryState.Created &&
                          context.RequesterRole == RequesterRole.User;
            
            return (allowed, allowed ? DeliveryState.Approved: null);
        }
    }
}