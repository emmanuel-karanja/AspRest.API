using AutoMapper;
using System;
using AspRest.API.Repositories;
using AspRest.API.Models.UserAccounts;
using AspRest.API.Entities;
using BC=BCrypt.Net.BCrypt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AspRest.API.Utils;
using System.Collections.Generic;

namespace AspRest.API.Services
{
    public class UserAccountService : IUserAccountService
    {
        private readonly IUserAccountRepository _repo;
        private readonly IMapper _mapper;
        private readonly AppConfigSettings _appConfigSettings;

        public UserAccountService(
            IUserAccountRepository repo,
            IMapper mapper,
            IOptions<AppConfigSettings> appConfigSettings
        )
        {
           _repo=repo;
           _mapper=mapper;
           _appConfigSettings=appConfigSettings.Value;
        }
        public AuthenticateUserResponse Authenticate(AuthenticateUserRequest authAccountDTO,string ipAddress)
        {
          //get account by email
          var account=_repo.GetByEmail(authAccountDTO.Email);
          if(account == null)
             throw new AppException($"User with Email :{authAccountDTO.Email} is not registered");
          //check if the account is verified
          if(!account.IsVerified)
             throw new AppException($"Account registered with Email: {authAccountDTO.Email} is not verified");
         //verify that the password is correct
          if(! BC.Verify(authAccountDTO.Password,account.PasswordHash))
            throw new AppException("Email or password is incorrect");

        //authentication success so generate the tokens
        var jwtToken=generateJwtToken(account);
        var refreshToken=generateRefreshToken(ipAddress);

        //save the refreshtoken.
        account.RefreshTokens.Add(refreshToken);
        
        account.LastLoginOn=DateTime.UtcNow;
        _repo.Update(account);

       //craft response
        var response=_mapper.Map<AuthenticateUserResponse>(account);
        response.JwtToken=jwtToken;
        response.RefreshToken=refreshToken.Token;

        return response;
        }

        
        public void Register(RegisterUserAccountRequest regAccountDTO,string origin)
        {
            //check if the account already exists
           if(_repo.Exists(regAccountDTO.Email))
             //potentially send an email
              return;
           
           var account=_mapper.Map<UserAccount>(regAccountDTO);
           //the first registered user is the admin
           bool isFirstAccount=_repo.Count()==0;
           account.Role=isFirstAccount? Role.Admin : Role.User;
           account.CreatedOn=DateTime.UtcNow;
           account.VerificationToken=randomTokenString();

           //hash password
           account.PasswordHash=BC.HashPassword(regAccountDTO.Password);
           //save the account
           _repo.Create(account);
           //send the registration email
        }
        public IEnumerable<UserAccountResponse> GetAll()
        {
            var accounts=_repo.GetAll();
            return _mapper.Map<IEnumerable<UserAccountResponse>>(accounts);
        }
        public UserAccountResponse GetById(long id)
        {
            var account=_repo.GetById(id);
            return _mapper.Map<UserAccountResponse>(account);

        }
        public UserAccountResponse Create(CreateUserAccountRequest newAccountDTO)
        {
             //validate that the email is not already in use
             if(_repo.Exists(newAccountDTO.Email))
               throw new AppException($"Email  {newAccountDTO.Email} is already registered");
            var newAccount=_mapper.Map<UserAccount>(newAccountDTO);
            newAccount.CreatedOn=DateTime.UtcNow;
            newAccount.VerifiedOn=DateTime.UtcNow;

            //hashpassword
            newAccount.PasswordHash=BC.HashPassword(newAccountDTO.Password);
            //save
            _repo.Create(newAccount);
            
            return _mapper.Map<UserAccountResponse>(newAccount);
        }

        public UserAccountResponse Update(long id, UpdateUserAccountRequest updatedAccountDTO)
        {
           var account=_repo.GetById(id);
           if(account == null)
                throw new  AppException($"User with Id {id} does not exist");

            //hash password if it was provided
            if(!string.IsNullOrEmpty(updatedAccountDTO.Password))
                  account.PasswordHash=BC.HashPassword(updatedAccountDTO.Password);

            //copy DTO to account and save
            _mapper.Map(updatedAccountDTO,account);
            account.UpdatedOn=DateTime.UtcNow;
            _repo.Update(account);

            return _mapper.Map<UserAccountResponse>(account);
        }

        public void Delete (long id)
        {
           var account=_repo.GetById(id);
           if(account == null)
           {
               throw new AppException($"User with ID {id} does not exist");
           }
           else
           {
              _repo.Delete(account); 
           }        
        }

        public AuthenticateUserResponse RefreshToken(string token, string ipAddress)
        {
            //get the refresh token and the account that owns it
            var (refreshToken,account)=getRefreshToken(token);

            //replace old refresh token with a new one and save
            var newRefreshToken=generateRefreshToken(ipAddress);
            refreshToken.RevokedOn=DateTime.UtcNow;
            refreshToken.RevokedByIp=ipAddress;
            refreshToken.ReplacedByToken=newRefreshToken.Token;
            account.RefreshTokens.Add(newRefreshToken);

            _repo.Update(account);

            //generate new jwt token
            var jwtToken=generateJwtToken(account);

            //craft response
            var response=_mapper.Map<AuthenticateUserResponse>(account);
            response.JwtToken=jwtToken;
            response.RefreshToken=newRefreshToken.Token;

            return response;
        }

        public void RevokeToken(string token,string ipAddress)
        {
            //generate the refresh token
            var (refreshToken,account)=getRefreshToken(token);
            //update and save
            refreshToken.RevokedOn=DateTime.UtcNow;
            refreshToken.RevokedByIp=ipAddress;
            _repo.Update(account);
        }
        public void VerifyEmail(string token)
        {
            var account=_repo.GetByToken(token);
            if(account == null) throw new AppException("Verification failed");

            //update account and save
            account.VerifiedOn=DateTime.UtcNow;
            account.VerificationToken=null;
            _repo.Update(account);
        }
        public void ForgotPassword(ForgotPasswordRequest forgotPasswordDTO,string origin)
        {
            var account=_repo.GetByEmail(forgotPasswordDTO.Email);

            //probably shouldn't do this to prevent email enumeration 
            if(account == null) throw new AppException($"Account with email: {forgotPasswordDTO.Email} does not exist");
            //basically 
            account.ResetToken=randomTokenString();
            //the number of days is probably something we need to store in the appsettings
            //and parse
            account.ResetTokenExpiresOn=DateTime.UtcNow.AddDays(3);
            _repo.Update(account);

            /*send a reset password email ala:
              sendPasswordResetEmail(account,origin)
            */
        }
        public void ValidateResetToken(ValidateResetTokenRequest validateResetDTO)
        {
            //get account by reset token
            var account=_repo.GetByResetToken(validateResetDTO.Token);

            if(account == null) throw new AppException("Invalid or expired reset token");

        }
        public void ResetPassword(ResetPasswordRequest resetPasswordDTO)
        {
            var account=_repo.GetByResetToken(resetPasswordDTO.Token);
            if(account == null) throw new AppException("Invalid or expired reset token");

            //else update account and save
            account.PasswordHash=BC.HashPassword(resetPasswordDTO.Password);
            account.PasswordResetLastOn=DateTime.UtcNow;
            account.ResetToken=null;
            account.ResetTokenExpiresOn=null;

            _repo.Update(account);
        }
        //utilities

        private RefreshToken generateRefreshToken(string ipAddress)
        {
             return new RefreshToken
             {
                Token=randomTokenString(),
                ExpiresOn=DateTime.UtcNow.AddDays(7),
                CreatedOn=DateTime.UtcNow,
                CreatedByIp=ipAddress
             };
        }

        private string generateJwtToken(UserAccount account)
        {
           var tokenHandler=new JwtSecurityTokenHandler();
           var key=Encoding.ASCII.GetBytes(_appConfigSettings.JwtSecret);
           var claimsIssuer=_appConfigSettings.JwtClaimsIssuer;
           var claimsAudience=_appConfigSettings.JwtClaimsAudience;
           var tokenDescriptor=new SecurityTokenDescriptor
           {
                Subject = new ClaimsIdentity(new[]{new Claim("id",account.Id.ToString()) }),
                Expires=DateTime.UtcNow.AddMinutes(30),
                Issuer=claimsIssuer,
                Audience=claimsAudience,
                SigningCredentials=new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
           };

           var token = tokenHandler.CreateToken(tokenDescriptor);
           return tokenHandler.WriteToken(token);
        }

        private string randomTokenString()
        {
            using var rngCryptoServiceProvider=new RNGCryptoServiceProvider();
            var randomBytes=new byte[64];
            rngCryptoServiceProvider.GetBytes(randomBytes);

            //convert bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-","");
        }

       //this returns a tuple nothing special here
        private (RefreshToken,UserAccount) getRefreshToken(string token)
        {
            var account=_repo.GetByToken(token);
            if(account == null) throw new AppException("Invalid token");
            var refreshToken=account.RefreshTokens.Single(x=>x.Token==token);
            if(!refreshToken.IsActive) throw new AppException("Invalid token");
            return (refreshToken,account);
        }
    }
}