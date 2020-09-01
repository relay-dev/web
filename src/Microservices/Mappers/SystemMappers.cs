using AutoMapper;
using System;

namespace Microservices.Mappers
{
    public class SystemMappers : Profile
    {
        public SystemMappers()
        {
            CreateMap<DateTimeOffset, DateTime>()
                .ForMember(dest => dest, opt => opt.MapFrom(src => src.DateTime));

            CreateMap<DateTimeOffset, DateTime?>()
                .ForMember(dest => dest, opt => opt.MapFrom(src => src.DateTime));

            CreateMap<DateTimeOffset?, DateTime>()
                .ForMember(dest => dest, opt => opt.MapFrom(src => src.GetValueOrDefault().DateTime));

            CreateMap<DateTimeOffset?, DateTime?>()
                .ForMember(dest => dest, opt => opt.MapFrom(src => src.HasValue ? src.Value.DateTime : (DateTime?)null));

            CreateMap<DateTime, DateTimeOffset>()
                .ForMember(dest => dest, opt => opt.MapFrom(src => (DateTimeOffset)src));

            CreateMap<DateTime, DateTimeOffset?>()
                .ForMember(dest => dest, opt => opt.MapFrom(src => (DateTimeOffset)src));

            CreateMap<DateTime?, DateTimeOffset>()
                .ForMember(dest => dest, opt => opt.MapFrom(src => (DateTimeOffset)src.GetValueOrDefault()));

            CreateMap<DateTime?, DateTimeOffset?>()
                .ForMember(dest => dest, opt => opt.MapFrom(src => src.HasValue ? src.Value : (DateTimeOffset?)null));
        }
    }
}
