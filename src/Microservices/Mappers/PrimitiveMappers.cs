using AutoMapper;
using System.Text;

namespace Microservices.Mappers
{
    public class PrimitiveMappers : Profile
    {
        public PrimitiveMappers()
        {
            // These facilitate an auto-mapper between the byte[] RowVersion on entities and the string RowVersion that is exposed on the DTO
            CreateMap<byte[], string>().ConvertUsing(bytes => new UTF8Encoding().GetString(bytes, 0, bytes.Length));
            CreateMap<string, byte[]>().ConvertUsing(str => new UTF8Encoding().GetBytes(str));
        }
    }
}
