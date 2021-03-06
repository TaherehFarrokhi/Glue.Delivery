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
    public sealed class GetDeliveryRequestHandler :
        IRequestHandler<GetDeliveryRequest, OperationResult<OrderDeliveryDto>>
    {
        private readonly DeliveryDbContext _dbContext;
        private readonly ILogger<NewDeliveryRequestHandler> _logger;
        private readonly IMapper _mapper;

        public GetDeliveryRequestHandler(DeliveryDbContext dbContext, IMapper mapper,
            ILogger<NewDeliveryRequestHandler> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<OrderDeliveryDto>> Handle(GetDeliveryRequest request,
            CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            _logger.LogInformation($"Get deliveryRequest for Id {request.DeliveryId}");

            try
            {
                var orderDelivery =
                    await _dbContext.Deliveries.FirstOrDefaultAsync(x => x.DeliveryId == request.DeliveryId);
                if (orderDelivery == null)
                    return OperationResult<OrderDeliveryDto>.Error(OperationErrorReason.ResourceNotFound,
                        $"The deliveryRequest not found for Id {request.DeliveryId}");

                var result = _mapper.Map<OrderDeliveryDto>(orderDelivery);
                return OperationResult<OrderDeliveryDto>.Success(result);
            }
            catch (Exception e)
            {
                var message = $"Error in getting the deliveryRequest for Id {request.DeliveryId}";

                _logger.LogError(message, e);
                return OperationResult<OrderDeliveryDto>.Error(message);
            }
        }
    }
}