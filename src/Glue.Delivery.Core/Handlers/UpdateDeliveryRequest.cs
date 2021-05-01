using System;
using Glue.Delivery.Core.Domain;
using Glue.Delivery.Core.Dto;
using MediatR;

namespace Glue.Delivery.Core.Handlers
{
    public sealed class UpdateDeliveryRequest : IRequest<OperationResult<OrderDeliveryDto>>
    {
        public UpdateDeliveryRequest(Guid deliveryId, DeliveryRequestDto deliveryRequestDto)
        {
            DeliveryId = deliveryId;
            DeliveryRequestDto = deliveryRequestDto ?? throw new ArgumentNullException(nameof(deliveryRequestDto));
        }

        public Guid DeliveryId { get; }
        public DeliveryRequestDto DeliveryRequestDto { get; }
    }
}