using System;
using FluentValidation;
using Glue.Delivery.Core.Dto;

namespace Glue.Delivery.WebApi.Validators
{
    public class DeliveryRequestValidator : AbstractValidator<DeliveryRequestDto>
    {
        public DeliveryRequestValidator()
        {
            RuleFor(m => m.Order).NotNull();
            RuleFor(m => m.Order.Sender).NotEmpty();
            RuleFor(m => m.Order.OrderNumber).NotEmpty();
            RuleFor(m => m.AccessWindow).NotNull();
            RuleFor(m => m.AccessWindow.EndTime).NotEqual(DateTime.MinValue);
            RuleFor(m => m.AccessWindow.StartTime).NotEqual(DateTime.MinValue);
            RuleFor(m => m.AccessWindow.StartTime).LessThan(m => m.AccessWindow.EndTime).WithMessage("AccessWindow StartTime should be less than EndTime");
            RuleFor(m => m.Recipient).NotNull();
            RuleFor(m => m.Recipient.Name).NotEmpty();
            RuleFor(m => m.Recipient.Email).NotEmpty().EmailAddress();
            RuleFor(m => m.Recipient.PhoneNumber).NotEmpty();
            RuleFor(m => m.Recipient.Address).NotEmpty();
        }
    }
}