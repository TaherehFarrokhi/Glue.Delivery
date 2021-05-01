using System;
using Glue.Delivery.Core.Domain;
using Glue.Delivery.Core.Dto;
using MediatR;

namespace Glue.Delivery.Core
{
    public sealed class GetDeliveryRequest : IRequest<OperationResult<OrderDeliveryDto>>
    {
        public GetDeliveryRequest(Guid deliveryId)
        {
            DeliveryId = deliveryId;
        }

        public Guid DeliveryId { get; }
    }
}