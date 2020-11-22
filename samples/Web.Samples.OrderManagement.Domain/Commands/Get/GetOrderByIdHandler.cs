using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Samples.OrderManagement.Domain.Context;
using Web.Samples.OrderManagement.Domain.DTO;
using Web.Samples.OrderManagement.Domain.Entities;

namespace Web.Samples.OrderManagement.Domain.Commands.Get
{
    public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdRequest, GetOrderByIdResponse>
    {
        private readonly OrderContext _dbContext;
        private readonly IMapper _mapper;

        public GetOrderByIdHandler(
            OrderContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<GetOrderByIdResponse> Handle(GetOrderByIdRequest request, CancellationToken cancellationToken)
        {
            OrderEntity orderEntity = await _dbContext.Orders
                .Where(o => o.OrderId == request.OrderId)
                .SingleOrDefaultAsync(cancellationToken);

            var order = _mapper.Map<Order>(orderEntity);

            return new GetOrderByIdResponse
            {
                Order = order
            };
        }
    }
}
