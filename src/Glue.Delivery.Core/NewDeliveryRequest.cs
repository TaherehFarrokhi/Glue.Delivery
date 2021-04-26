using System;
using Glue.Delivery.Core.Domain;
using Glue.Delivery.Core.Dto;
using MediatR;

namespace Glue.Delivery.Core
{
    public sealed class NewDeliveryRequest : IRequest<OperationResult<OrderDeliveryDto>>
    {
        public NewDeliveryDto DeliveryDto { get; }

        public NewDeliveryRequest(NewDeliveryDto deliveryDto)
        {
            DeliveryDto = deliveryDto ?? throw new ArgumentNullException(nameof(deliveryDto));
        }
    }
}