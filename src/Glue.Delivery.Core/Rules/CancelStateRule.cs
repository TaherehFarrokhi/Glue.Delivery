using System.Linq;
using Glue.Delivery.Core.Domain;

namespace Glue.Delivery.Core.Rules
{
    public sealed class CancelStateRule : IStateRule
    {
        private readonly DeliveryState[] _allowedStates = {DeliveryState.Created, DeliveryState.Approved};

        public bool CanApply(DeliveryAction action, ActionContext context) => action == DeliveryAction.Cancel;

        public (bool Allowed, DeliveryState? Target) Apply(DeliveryAction action, ActionContext context)
        {
            var allowed = action == DeliveryAction.Cancel && 
                          _allowedStates.Contains(context.OrderDelivery.State);
            return (allowed, DeliveryState.Cancelled);
        }
    }
}