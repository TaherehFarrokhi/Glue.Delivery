using System;
using Glue.Delivery.Core.Domain;
using Glue.Delivery.Core.Dto;
using MediatR;

namespace Glue.Delivery.Core
{
    public sealed class UpdateDeliveryRequest : IRequest<OperationResult<OrderDeliveryDto>>
    {
        public Guid DeliveryId { get; }
        public DeliveryRequestDto DeliveryRequestDto { get; }

        public UpdateDeliveryRequest(Guid deliveryId, DeliveryRequestDto deliveryRequestDto)
        {
            DeliveryId = deliveryId;
            DeliveryRequestDto = deliveryRequestDto ?? throw new ArgumentNullException(nameof(deliveryRequestDto));
        }
    }
}   