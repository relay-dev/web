using AutoMapper;
using System;

namespace Microservices.Mappers
{
    public class SystemMappers : Profile
    {
        public SystemMappers()
        {
            CreateMap<DateTimeOffset, DateTime>().ConvertUsing(src => src.DateTime);
            CreateMap<DateTimeOffset, DateTime?>().ConvertUsing(src => src.DateTime);
            CreateMap<DateTimeOffset?, DateTime>().ConvertUsing(src => src.GetValueOrDefault().DateTime);
            CreateMap<DateTimeOffset?, DateTime?>().ConvertUsing(src => src.HasValue ? src.Value.DateTime : (DateTime?)null);
            CreateMap<DateTime, DateTimeOffset>().ConvertUsing(src => (DateTimeOffset)src);
            CreateMap<DateTime, DateTimeOffset?>().ConvertUsing(src => (DateTimeOffset)src);
            CreateMap<DateTime?, DateTimeOffset>().ConvertUsing(src => (DateTimeOffset)src.GetValueOrDefault());
            CreateMap<DateTime?, DateTimeOffset?>().ConvertUsing(src => src.HasValue ? src.Value : (DateTimeOffset?)null);
        }
    }
}
