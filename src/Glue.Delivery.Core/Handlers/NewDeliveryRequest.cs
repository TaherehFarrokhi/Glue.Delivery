using System;
using Glue.Delivery.Core.Domain;
using Glue.Delivery.Core.Dto;
using MediatR;

namespace Glue.Delivery.Core.Handlers
{
    public sealed class NewDeliveryRequest : IRequest<OperationResult<OrderDeliveryDto>>
    {
        public NewDeliveryRequest(DeliveryRequestDto deliveryRequestDto)
        {
            DeliveryRequestDto = deliveryRequestDto ?? throw new ArgumentNullException(nameof(deliveryRequestDto));
        }

        public DeliveryRequestDto DeliveryRequestDto { get; }
    }
}