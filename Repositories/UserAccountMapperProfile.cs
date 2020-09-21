using AutoMapper;
using AspRest.API.Entities;
using AspRest.API.Models.UserAccounts;

namespace AspRest.API.Repositories
{
    /* maps the UserAccount entity to and from the various input and output DTOs*/
    public class UserAccountMapperProfile: Profile
    {
        public UserAccountMapperProfile()
        {
             CreateMap<UserAccount, UserAccountResponse>();
       
             CreateMap<UserAccount,AuthenticateUserResponse>();

             CreateMap<RegisterUserAccountRequest,UserAccount>();

             CreateMap<CreateUserAccountRequest,UserAccount>();

             CreateMap<UpdateUserAccountRequest,UserAccount>()
                   .ForAllMembers(x=> x.Condition(
                    (src,dest,prop)=>
                    {
                        //ignore null and empty string properties
                        if(prop == null) return false;

                        if(prop.GetType()==typeof(string) 
                          && string.IsNullOrEmpty((string) prop)) return false;

                        if(x.DestinationMember.Name == "Role" && src.Role == null) return false;

                        return true;
                    }
                   ));
        }
    }
}