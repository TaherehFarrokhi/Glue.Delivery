using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Glue.Delivery.Core.Domain;
using Glue.Delivery.Core.Dto;
using Glue.Delivery.Core.Stores;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Glue.Delivery.Core.Handlers
{
    public sealed class UpdateDeliveryRequestHandler :
        IRequestHandler<UpdateDeliveryRequest, OperationResult<OrderDeliveryDto>>
    {
        private readonly DeliveryDbContext _dbContext;
        private readonly ILogger<UpdateDeliveryRequestHandler> _logger;
        private readonly IMapper _mapper;

        public UpdateDeliveryRequestHandler(DeliveryDbContext dbContext, IMapper mapper,
            ILogger<UpdateDeliveryRequestHandler> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<OrderDeliveryDto>> Handle(UpdateDeliveryRequest request,
            CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            _logger.LogInformation($"Update deliveryRequest for Id {request.DeliveryId}");

            try
            {
                var orderDelivery = await _dbContext.Deliveries
                    .FirstOrDefaultAsync(x => x.DeliveryId == request.DeliveryId, cancellationToken);
                if (orderDelivery == null)
                    return OperationResult<OrderDeliveryDto>.Error(OperationErrorReason.ResourceNotFound,
                        $"The deliveryRequest not found for Id {request.DeliveryId}");

                orderDelivery.Order = _mapper.Map<Order>(request.DeliveryRequestDto.Order);
                orderDelivery.Recipient = _mapper.Map<Recipient>(request.DeliveryRequestDto.Recipient);
                orderDelivery.AccessWindow = _mapper.Map<AccessWindow>(request.DeliveryRequestDto.AccessWindow);
                _dbContext.Update(orderDelivery);

                await _dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation($"Delivery for Id {request.DeliveryId} has been updated successfully");

                var result = _mapper.Map<OrderDeliveryDto>(orderDelivery);
                return OperationResult<OrderDeliveryDto>.Success(result);
            }
            catch (Exception e)
            {
                var message = $"Error in updating the deliveryRequest for for Id {request.DeliveryId}";

                _logger.LogError(message, e);
                return OperationResult<OrderDeliveryDto>.Error(message);
            }
        }
    }
}