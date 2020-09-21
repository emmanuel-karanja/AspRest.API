using System;
using AspRest.API.Entities;
using AspRest.API.Models.UserAccounts;
using AspRest.API.Repositories;
using AspRest.API.Utils;
using System.Collections.Generic;

namespace AspRest.API.Services
{
    public interface IUserAccountService
    {
        AuthenticateUserResponse Authenticate(AuthenticateUserRequest authUserDTO,string ipAddress);
        AuthenticateUserResponse RefreshToken(string token, string ipAddress);
        void Register(RegisterUserAccountRequest regUserDTO,string origin);
        IEnumerable<UserAccountResponse> GetAll();
        UserAccountResponse GetById(long id);
        UserAccountResponse Create(CreateUserAccountRequest newUserDTO);
        UserAccountResponse Update(long id, UpdateUserAccountRequest updatedUserDTO);
        void Delete (long id);
        void RevokeToken(string token,string ipAddress);
        void VerifyEmail(string token);
        void ForgotPassword(ForgotPasswordRequest forgotPasswordDTO,string origin);
        void ValidateResetToken(ValidateResetTokenRequest dto);
        void ResetPassword(ResetPasswordRequest dto);

    }
}
