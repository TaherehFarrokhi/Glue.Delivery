using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Glue.Delivery.Core.Domain;
using Glue.Delivery.Core.Stores;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Glue.Delivery.Core.Handlers
{
    public sealed class ExpireDeliveryRequestHandler :
        IRequestHandler<ExpireDeliveryRequest>
    {
        private readonly DeliveryDbContext _dbContext;
        private readonly ILogger<ExpireDeliveryRequestHandler> _logger;
        private readonly Func<OrderDelivery, bool> _expiryDatePredicate = o => o.AccessWindow.EndTime <= DateTime.UtcNow; 
        private readonly Func<OrderDelivery, bool> _expiryStatePredicate = o => o.State is DeliveryState.Created or DeliveryState.Approved; 

        public ExpireDeliveryRequestHandler(DeliveryDbContext dbContext,
            ILogger<ExpireDeliveryRequestHandler> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Unit> Handle(ExpireDeliveryRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Expiring deliveryRequests starting");

            try
            {
                var orderDeliveries = _dbContext.Deliveries.Where(o =>
                    o.AccessWindow.EndTime <= DateTime.UtcNow &&
                    (o.State == DeliveryState.Approved || o.State == DeliveryState.Created)).ToList();
                
                if (!orderDeliveries.Any())
                {
                    _logger.LogInformation("No expired deliveries found");
                    return Unit.Value;
                }
                foreach (OrderDelivery orderDelivery in orderDeliveries)
                {
                    orderDelivery.State = DeliveryState.Expired;
                }

                await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.LogInformation($"Updated {orderDeliveries.Count} deliveries to expired state");

                return Unit.Value;
            }
            catch (Exception e)
            {
                _logger.LogError("Error in expiring the deliveries", e);
            }
            return Unit.Value;
        }
    }
}