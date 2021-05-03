using System;
using System.Threading.Tasks;
using Glue.Delivery.Core.Domain;
using Glue.Delivery.Core.Dto;
using Glue.Delivery.Core.Handlers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Glue.Delivery.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeliveriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DeliveriesController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("{deliveryId:guid}")]
        public async Task<ActionResult<OrderDeliveryDto>> Get(Guid deliveryId)
        {
            var response = await _mediator.Send(new GetDeliveryRequest(deliveryId));
            if (!response.Failed)
                return Ok(response.Result);
            return response.ErrorReason == OperationErrorReason.ResourceNotFound
                ? NotFound()
                : StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpPost("")]
        public async Task<ActionResult<OrderDeliveryDto>> Post([FromBody] DeliveryRequestDto deliveryRequest)
        {
            var response = await _mediator.Send(new NewDeliveryRequest(deliveryRequest));
            if (response.Failed)
                return BadRequest();

            return CreatedAtAction(nameof(Get), new {deliveryId = response.Result.DeliveryId}, response.Result);
        }

        [HttpPut("{deliveryId:guid}")]
        public async Task<ActionResult<OrderDeliveryDto>> Put([FromRoute] Guid deliveryId,
            [FromBody] DeliveryRequestDto deliveryRequest)
        {
            var response = await _mediator.Send(new UpdateDeliveryRequest(deliveryId, deliveryRequest));

            if (!response.Failed)
                return Ok(response.Result);

            return response.ErrorReason == OperationErrorReason.ResourceNotFound
                ? NotFound()
                : StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpDelete("{deliveryId:guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid deliveryId)
        {
            var response = await _mediator.Send(new DeleteDeliveryRequest(deliveryId));

            if (!response.Failed)
                return Ok();

            return response.ErrorReason == OperationErrorReason.ResourceNotFound
                ? NotFound()
                : StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpPost("{deliveryId:guid}/state")]
        public async Task<ActionResult> ChangeDeliveryState(
            [FromRoute] Guid deliveryId, [FromQuery] DeliveryAction action, [FromHeader(Name = "role")] RequesterRole? role)
        {
            if (role == null)
                return Unauthorized("Role should be defined in header. Values: User | Partner");
            
            var response = await _mediator.Send(new ChangeDeliveryStateRequest(deliveryId, action, role.Value));
            if (!response.Failed)
                return Ok();

            return response.ErrorReason switch
            {
                OperationErrorReason.ResourceNotFound => NotFound(),
                OperationErrorReason.InvalidOperation => Conflict(),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        } 
    }
}