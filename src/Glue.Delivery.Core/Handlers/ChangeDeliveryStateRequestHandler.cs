using System;
using System.Threading;
using System.Threading.Tasks;
using Glue.Delivery.Core.Domain;
using Glue.Delivery.Core.Rules;
using Glue.Delivery.Core.Stores;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Glue.Delivery.Core.Handlers
{
    public sealed class ChangeDeliveryStateRequestHandler :
        IRequestHandler<ChangeDeliveryStateRequest, OperationResult<Unit>>
    {
        private readonly DeliveryDbContext _dbContext;
        private readonly IStateRuleEngine _stateRuleEngine;
        private readonly ILogger<ChangeDeliveryStateRequestHandler> _logger;

        public ChangeDeliveryStateRequestHandler(DeliveryDbContext dbContext,
            IStateRuleEngine stateRuleEngine,
            ILogger<ChangeDeliveryStateRequestHandler> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _stateRuleEngine = stateRuleEngine ?? throw new ArgumentNullException(nameof(stateRuleEngine));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<Unit>> Handle(ChangeDeliveryStateRequest request,
            CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            _logger.LogInformation($"Apply {request.Action} action on delivery Id {request.DeliveryId}");

            try
            {
                var orderDelivery = await _dbContext.Deliveries
                    .FirstOrDefaultAsync(x => x.DeliveryId == request.DeliveryId, cancellationToken);
                if (orderDelivery == null)
                    return OperationResult<Unit>.Error(OperationErrorReason.ResourceNotFound,
                        $"The deliveryRequest not found for Id {request.DeliveryId}");

                var (allowed, target) = _stateRuleEngine.ApplyAction(request.Action,
                    new ActionContext(orderDelivery, request.RequesterRole));
                
                if (!allowed)
                    return OperationResult<Unit>.Error(OperationErrorReason.InvalidOperation, $"{request.Action} is not possible on delivery in {orderDelivery.State} state");

                orderDelivery.State = target.Value;
                await _dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation($"Delivery for Id {request.DeliveryId} has been updated successfully");

                return OperationResult<Unit>.Success(Unit.Value);
            }
            catch (Exception e)
            {
                var message = $"Error in updating the deliveryRequest for for Id {request.DeliveryId}";

                _logger.LogError(message, e);
                return OperationResult<Unit>.Error(message);
            }
        }
    }
}