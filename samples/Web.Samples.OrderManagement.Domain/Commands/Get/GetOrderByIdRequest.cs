using MediatR;

namespace Web.Samples.OrderManagement.Domain.Commands.Get
{
    public class GetOrderByIdRequest : IRequest<GetOrderByIdResponse>
    {
        public long OrderId { get; set; }
    }
}
