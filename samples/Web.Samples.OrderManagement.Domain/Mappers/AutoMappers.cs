using AutoMapper;
using Core.Plugins.AutoMapper.Extensions;
using Web.Samples.OrderManagement.Domain.DTO;
using Web.Samples.OrderManagement.Domain.Entities;

namespace Web.Samples.OrderManagement.Domain.Mappers
{
    public class AutoMappers : Profile
    {
        public AutoMappers()
        {
            CreateMap<Order, OrderEntity>().IgnoreAuditFields();
            CreateMap<OrderEntity, Order>();
        }
    }
}
