using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using AspRest.API.Models.UserAccounts;
using AspRest.API.Services;
using AspRest.API.Utils;
using AspRest.API.Entities;

namespace AspRest.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserAccountController : BaseController
    {
        private readonly IUserAccountService _service;
        private readonly IMapper _mapper;

        public UserAccountController(
            IUserAccountService service,
            IMapper mapper
        )
        {
            _service=service;
            _mapper=mapper;
        }
        
        [HttpPost("login")]
        public ActionResult<AuthenticateUserResponse> Login(AuthenticateUserRequest authCreds)
        {
            var response=_service.Authenticate(authCreds,ipAddress());
            setTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public ActionResult<AuthenticateUserResponse> RefreshToken()
        {
            var refreshToken=Request.Cookies["refreshToken"];
            var response=_service.RefreshToken(refreshToken,ipAddress());
            setTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        
        [HttpPost("register")]
        public ActionResult Register(RegisterUserAccountRequest regAccountDTO)
        {
            _service.Register(regAccountDTO,Request.Headers["origin"]);
            return Ok(new { message= " Registration successful"});
        }

        //secured route: Admin only
        [Authorize(Role.Admin)]
        [HttpGet("all")]
        public ActionResult<IEnumerable<UserAccountResponse>> GetAll(){
            var accounts=_service.GetAll();
            return Ok(accounts);
        }
       
       //protected route: current user or admin
       [Authorize]
       [HttpGet("{id:long}")]
       public ActionResult<UserAccountResponse> GetById(long id)
       {
           //a user can only view their own account unless it's admin
           if(id != CurrentUser.Id && CurrentUser.Role !=Role.Admin)
               return Unauthorized(new { message="Unauthorized"});
           var account=_service.GetById(id);
           return Ok(account);
       }

       //only admin can create new users

       [Authorize(Role.Admin)]
       [HttpPost]
       public ActionResult<UserAccountResponse> Create(CreateUserAccountRequest newAccountDTO)
       {
           var account=_service.Create(newAccountDTO);
           return Ok(account);
       }

       //protected route
       [Authorize]
       [HttpPut("{id:long}")]
       public ActionResult<UserAccountResponse> Update(long id,UpdateUserAccountRequest updateAccountDTO)
       {
        //restrict to current user and admin
           if(id !=CurrentUser.Id && CurrentUser.Role !=Role.Admin)
               return Unauthorized(new {message= "Unauthorized"});

        //only admin can update role
        if(CurrentUser.Role != Role.Admin)
            updateAccountDTO.Role=null;
        var account=_service.Update(id,updateAccountDTO);
        return Ok(account);
       }

       [Authorize]
       [HttpDelete("{id:long}")]
       public IActionResult Delete(long id)
       {
        if(id != CurrentUser.Id && CurrentUser.Role !=Role.Admin)
            return Unauthorized(new {message="Unauthorized"});
        
        _service.Delete(id);
        return Ok(new {message="User Account deleted successfully"});
       }

       [Authorize]
       [HttpPost("revoke-token")]
       public IActionResult RevokeToken(RevokeTokenRequest revokeTokenDTO)
       {
           var token=revokeTokenDTO.Token ?? Request.Cookies["refreshToken"];
           if(string.IsNullOrEmpty(token))
              return BadRequest(new {message="Token is required"});

            //users can revoke their own tokens and admin can for all
            if(!CurrentUser.OwnsToken(token) && CurrentUser.Role != Role.Admin)
              return Unauthorized(new {message="Unauthorized"});

         _service.RevokeToken(token,ipAddress());
         return Ok(new {message="Token revoked"});
       }

       [HttpPost("forgot-password")]
       public IActionResult ForgotPassword(ForgotPasswordRequest forgotPasswordDTO)
       {
           _service.ForgotPassword(forgotPasswordDTO,Request.Headers["origin"]);
           return Ok(new{message ="Please check your email for password reset instructions"});
       }
       
       [HttpPost("validate-reset-token")]
       public IActionResult ValidateResetToken(ValidateResetTokenRequest requestDTO)
       {
           _service.ValidateResetToken(requestDTO);
           return Ok(new {message="Token is valid"});
       }

       [HttpPost("reset-password")]
       public IActionResult ResetPassword(ResetPasswordRequest resetRequestDTO)
       {
           _service.ResetPassword(resetRequestDTO);
           return Ok(new {message="Password reset successfully, you can now login"});
       }
       //utility methods
       //sets the refreshToken in the response cookie
       private void setTokenCookie(string token)
       {
           var cookieOptions=new CookieOptions
           {
               HttpOnly=true,
               Expires=DateTime.UtcNow.AddDays(7)
           };
           Response.Cookies.Append("refreshToken",token,cookieOptions);
       }

       //obtains the current request's IP Address
       private string ipAddress()
       {
           if(Request.Headers.ContainsKey("X-Forwarded-For"))
               return Request.Headers["X-Forwarded-For"];
            //else
            return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
       }
    }
}