﻿using System;
using System.Threading.Tasks;
using Glue.Delivery.Core;
using Glue.Delivery.Core.Domain;
using Glue.Delivery.Core.Dto;
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
            return response.ErrorReason == OperationErrorReason.ResourceNotFound ? NotFound() : StatusCode(StatusCodes.Status500InternalServerError);
        }                 
        
        [HttpPost("")]
        public async Task<ActionResult<OrderDeliveryDto>> Post([FromBody] NewDeliveryDto delivery)
        {
            var response = await _mediator.Send(new NewDeliveryRequest(delivery));
            if (response.Failed)
                return BadRequest();

            return CreatedAtAction(nameof(Get), new { deliveryId = response.Result.DeliveryId}, response.Result);
        }         
        
        // [HttpPut("{orderId}/delivery/{action}")]
        // public async Task<ActionResult<OrderDelivery>> Put(string orderId, DeliveryAction action)
        // {
        //     var result = await _mediator.Send(new DeliveryChangeStateRequest(orderId, action));
        //     return result.ToActionResult();
        // } 
    }
}