using System;
using Glue.Delivery.Core.Domain;

namespace Glue.Delivery.Core.Dto
{
    public sealed class OrderDeliveryDto
    {
        public Guid DeliveryId { get; init; }
        public DeliveryState State { get; init; }
        public AccessWindowDto AccessWindow { get; init; }
        public RecipientDto Recipient { get; init; }
        public OrderDto Order { get; init; }
    }
}