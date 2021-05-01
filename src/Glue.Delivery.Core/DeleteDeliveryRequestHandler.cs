using System;
using System.Threading;
using System.Threading.Tasks;
using Glue.Delivery.Core.Domain;
using Glue.Delivery.Core.Stores;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Glue.Delivery.Core
{
    public sealed class DeleteDeliveryRequestHandler :
        IRequestHandler<DeleteDeliveryRequest, OperationResult<Unit>>
    {
        private readonly DeliveryDbContext _dbContext;
        private readonly ILogger<DeleteDeliveryRequestHandler> _logger;

        public DeleteDeliveryRequestHandler(DeliveryDbContext dbContext,
            ILogger<DeleteDeliveryRequestHandler> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<Unit>> Handle(DeleteDeliveryRequest request,
            CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            _logger.LogInformation($"Delete deliveryRequest for Id {request.DeliveryId}");

            try
            {
                var orderDelivery =
                    await _dbContext.Deliveries.FirstOrDefaultAsync(x => x.DeliveryId == request.DeliveryId,
                        cancellationToken: cancellationToken);
                if (orderDelivery == null)
                    return OperationResult<Unit>.Error(OperationErrorReason.ResourceNotFound,
                        $"The deliveryRequest not found for Id {request.DeliveryId}");

                _dbContext.Deliveries.Remove(orderDelivery);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return OperationResult<Unit>.Success(Unit.Value);
            }
            catch (Exception e)
            {
                var message = $"Error in deleteing the deliveryRequest for Id {request.DeliveryId}";

                _logger.LogError(message, e);
                return OperationResult<Unit>.Error(message);
            }
        }
    }
}