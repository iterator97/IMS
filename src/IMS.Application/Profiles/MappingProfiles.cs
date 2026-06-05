using AutoMapper;
using IMS.Application.Products.Dto;
using IMS.Domain;

namespace IMS.Application.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductDto>();
        }
    }
}
