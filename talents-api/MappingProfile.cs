using AutoMapper;
using TalentsApi.Models.Entities;
using TalentsApi.Models.User;
using TalentsApi.Models.UserRole;

namespace TalentsApi
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserRole, UserRoleModel>().ReverseMap();
            CreateMap<User, CreateUpdateUserRoleModel>().ReverseMap();
            CreateMap<User, UserModel>().ReverseMap();
            CreateMap<User, CreateUpdateUserModel>().ReverseMap();
        }
    }
}
