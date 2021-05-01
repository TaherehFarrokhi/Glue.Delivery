using System;

namespace Glue.Delivery.Core.Domain
{
    public sealed class OrderDelivery
    {
        public Guid DeliveryId { get; set; } = Guid.NewGuid();
        public DeliveryState State { get; set; } = DeliveryState.Created;
        public AccessWindow AccessWindow { get; set; }
        public Recipient Recipient { get; set; }
        public Order Order { get; set; }
    }
}