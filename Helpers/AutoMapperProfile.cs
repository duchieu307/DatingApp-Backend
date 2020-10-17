using System.Linq;
using AutoMapper;
using Microservices.Dtos;
using Microservices.Models;

// bo loc du lieu
namespace Microservices.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // CreateMap<dau`, cuoi>();
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl,
                    opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age,
                    opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));

            // gan du lieu cua PhotoUrl trong DTO, voi du lieu sau khi qua xu ly cua Photos trong UserModel
            CreateMap<User, UserForDetailDto>()
                .ForMember(dest => dest.PhotoUrl,
                    opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age,
                    opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
                        
            CreateMap<Photo, PhotoForDetailDto>();

            CreateMap<UserForUpdateDto, User>();

            CreateMap<Photo, PhotoForReturnDto>();

            CreateMap<PhotoForCreationDto, Photo>();

            CreateMap<UserForRegisterDto, User>();

            // map 2 chieu
            CreateMap<MessageForCreationDto, Message>().ReverseMap();
            
            // forMember sau do chon gia tri, option
            CreateMap<Message, MessageToReturnDto>()
                .ForMember(member => member.SenderPhotoUrl, option => option
                    .MapFrom(user => user.Sender.Photos.FirstOrDefault(photo => photo.IsMain).Url))
                .ForMember(member => member.RecipientPhotoUrl, option => option
                    .MapFrom(user => user.Recipient.Photos.FirstOrDefault(photo => photo.IsMain).Url));
        }
    }
}
