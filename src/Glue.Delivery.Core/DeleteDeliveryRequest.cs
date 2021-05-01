using System;
using Glue.Delivery.Core.Domain;
using Glue.Delivery.Core.Dto;
using MediatR;

namespace Glue.Delivery.Core
{
    public sealed class DeleteDeliveryRequest : IRequest<OperationResult<Unit>>
    {
        public Guid DeliveryId { get; }

        public DeleteDeliveryRequest(Guid deliveryId)
        {
            DeliveryId = deliveryId;
        }
    }
}