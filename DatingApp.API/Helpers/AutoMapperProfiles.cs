using System.Linq;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Model.Model;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl, opt =>
                opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age, opt =>
                opt.MapFrom(src => src.DateOfBirth.CalculateAge()));

            CreateMap<User, UserForDetailDto>()
                .ForMember(dest => dest.PhotoUrl, opt =>
                opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age, opt =>
                opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            
            CreateMap<Photo, PhotosForDetailedDto>();

            CreateMap<Photo, PhotoForReturn>();

            CreateMap<PhotoForCreateDto, Photo>();

            CreateMap<UserForUpdateDto, User>();
        }

    }
}