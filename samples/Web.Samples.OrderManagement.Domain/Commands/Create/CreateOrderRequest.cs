using MediatR;
using Web.Samples.OrderManagement.Domain.DTO;

namespace Web.Samples.OrderManagement.Domain.Commands.Create
{
    public class CreateOrderRequest : IRequest<CreateOrderResponse>
    {
        public Order Order { get; set; }
    }
}
