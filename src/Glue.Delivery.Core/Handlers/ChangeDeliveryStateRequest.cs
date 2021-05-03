using System;
using Glue.Delivery.Core.Domain;
using MediatR;

namespace Glue.Delivery.Core.Handlers
{
    public sealed class ChangeDeliveryStateRequest : IRequest<OperationResult<Unit>>
    {
        public ChangeDeliveryStateRequest(Guid deliveryId, DeliveryAction action, RequesterRole requesterRole)
        {
            DeliveryId = deliveryId;
            Action = action;
            RequesterRole = requesterRole;
        }

        public Guid DeliveryId { get; }
        public DeliveryAction Action { get; }
        public RequesterRole RequesterRole { get; }
    }
}