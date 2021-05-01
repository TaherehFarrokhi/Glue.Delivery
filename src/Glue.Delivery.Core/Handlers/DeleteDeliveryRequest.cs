using System;
using Glue.Delivery.Core.Domain;
using MediatR;

namespace Glue.Delivery.Core.Handlers
{
    public sealed class DeleteDeliveryRequest : IRequest<OperationResult<Unit>>
    {
        public DeleteDeliveryRequest(Guid deliveryId)
        {
            DeliveryId = deliveryId;
        }

        public Guid DeliveryId { get; }
    }
}