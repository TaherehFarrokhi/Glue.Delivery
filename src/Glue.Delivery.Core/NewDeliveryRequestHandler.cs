using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Glue.Delivery.Core.Domain;
using Glue.Delivery.Core.Dto;
using Glue.Delivery.Core.Stores;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Glue.Delivery.Core
{
    public sealed class NewDeliveryRequestHandler : 
        IRequestHandler<NewDeliveryRequest, OperationResult<OrderDeliveryDto>>
    {
        private readonly DeliveryDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<NewDeliveryRequestHandler> _logger;

        public NewDeliveryRequestHandler(DeliveryDbContext dbContext, IMapper mapper, ILogger<NewDeliveryRequestHandler> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<OrderDeliveryDto>> Handle(NewDeliveryRequest request,
            CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var orderNumber = request.DeliveryDto?.Order?.OrderNumber;
            
            _logger.LogInformation($"Create delivery for order {orderNumber}");

            try
            {
                var orderDelivery = _mapper.Map<OrderDelivery>(request.DeliveryDto);
            
                _dbContext.Set<OrderDelivery>().Add(orderDelivery);
                await _dbContext.SaveChangesAsync(cancellationToken);
                
                _logger.LogInformation($"Delivery for {orderNumber} has been created successfully");
                
                var result = _mapper.Map<OrderDeliveryDto>(orderDelivery);
                return OperationResult<OrderDeliveryDto>.Success(result);
            }
            catch (Exception e)
            {
                var message = $"Error in creating the delivery for order {orderNumber}";
                
                _logger.LogError(message, e);
                return OperationResult<OrderDeliveryDto>.Error(message);
            }
        }
    }
}