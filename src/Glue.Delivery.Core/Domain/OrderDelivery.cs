using System;

namespace Glue.Delivery.Core.Domain
{
    public sealed class OrderDelivery
    {
        public Guid DeliveryId { get; init; } = Guid.NewGuid();
        public DeliveryState State { get; set; } = DeliveryState.Created;
        public AccessWindow AccessWindow { get; init; }
        public Recipient Recipient { get; init; }
        public Order Order { get; init; }
    }
}