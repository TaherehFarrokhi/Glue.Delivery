using System;
using System.ComponentModel;
using Glue.Delivery.Core.Domain;

namespace Glue.Delivery.Core.Rules
{
    public sealed class ActionContext
    {
        public OrderDelivery OrderDelivery { get; }
        public RequesterRole RequesterRole { get; }

        public ActionContext(OrderDelivery orderDelivery, RequesterRole requesterRole)
        {
            if (!Enum.IsDefined(typeof(RequesterRole), requesterRole))
                throw new InvalidEnumArgumentException(nameof(requesterRole), (int) requesterRole,
                    typeof(RequesterRole));
            OrderDelivery = orderDelivery ?? throw new ArgumentNullException(nameof(orderDelivery));
            RequesterRole = requesterRole;
        }
    }
}