using AutoMapper;

using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;

namespace MagicVilla_API
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Villa, VillaDto>().ReverseMap(); //<VillaDto, Villa>

            CreateMap<NumeroVilla, NumeroVillaCreateDto>().ReverseMap();
            CreateMap<NumeroVilla, NumeroVillaUpdateDto>().ReverseMap();
            CreateMap<NumeroVilla, NumeroVillaDto>().ReverseMap();
        }
    }
}
