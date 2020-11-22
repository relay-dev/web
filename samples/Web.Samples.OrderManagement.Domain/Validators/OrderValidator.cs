using FluentValidation;
using System;
using Web.Samples.OrderManagement.Domain.DTO;

namespace Web.Samples.OrderManagement.Domain.Validators
{
    public class OrderValidator : AbstractValidator<Order>
    {
        public OrderValidator()
        {
            CreateRules();
        }

        private void CreateRules()
        {
            RuleFor(order => order.OrderId).GreaterThan(0);
            RuleFor(order => order.OrderDate).NotEqual(DateTime.MinValue);
        }
    }
}
