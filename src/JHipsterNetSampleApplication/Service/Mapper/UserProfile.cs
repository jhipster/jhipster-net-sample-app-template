using AutoMapper;
using JHipsterNetSampleApplication.Models;
using JHipsterNetSampleApplication.Service.Dto;

namespace JHipsterNetSampleApplication.Service.Mapper {
    public class UserProfile : Profile {
        public UserProfile()
        {
            CreateMap<User, UserDto>();
        }
    }
}
