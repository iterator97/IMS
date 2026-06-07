using AutoMapper;
using IMS.Application.Products.Dto;
using IMS.Domain.Products;

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
