using AutoMapper;
using BusinessObject.Models;
using SportBookingWebAPI.Dtos.Category;

namespace SportBookingWebAPI.Mappers
{
    public class MapperConfig : Profile
    {
        public MapperConfig() {
            CreateMap<Category, CategoryDto>().ReverseMap();
        }

    }
}
