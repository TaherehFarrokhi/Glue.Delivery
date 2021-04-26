using System;
using Glue.Delivery.Core.Domain;
using Glue.Delivery.Core.Dto;
using MediatR;

namespace Glue.Delivery.Core
{
    public sealed class GetDeliveryRequest : IRequest<OperationResult<OrderDeliveryDto>>
    {
        public Guid DeliveryId { get; }

        public GetDeliveryRequest(Guid deliveryId)
        {
            DeliveryId = deliveryId;
        }
    }
}