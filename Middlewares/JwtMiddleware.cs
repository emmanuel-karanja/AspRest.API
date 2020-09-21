using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspRest.API.Utils;
using AspRest.API.Repositories;
using Microsoft.Extensions.Logging;

/*Extracts the token from the request header, and adds the current user to the context
so that the user profile is available down the pipeline especially in the authorization phase
 done either per method or per class in the controller or service level*/
namespace AspRest.API.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppConfigSettings _appConfigSettings;
        private readonly ILogger _logger;
        

        public JwtMiddleware(RequestDelegate next, 
                            IOptions<AppConfigSettings> appConfigSettings,
                            ILogger<JwtMiddleware> logger)
        {
            _next=next;
            _appConfigSettings=appConfigSettings.Value;
            _logger=logger;  
        }

        public async Task Invoke(HttpContext context,ApplicationDbContext dbContext)
        {
            var token=getTokenFromHeader(context);
            if(token!=null)
                 await attachUserAccountToContext(context,dbContext,token);
            //else continue without adding the user account
            await _next(context);
                
        }

        private string getTokenFromHeader(HttpContext context){
            //_logger.LogInformation(context.Request.Headers["Authorization"].ToString());
            return (string)context.Request.Headers["Authorization"]
                                          .FirstOrDefault()?.Split(" ")
                                          .Last();
        }

        private async Task attachUserAccountToContext(HttpContext context,ApplicationDbContext dbContext, string token)
        {              
               //a hack we should call validateToken to validate the token first//
               var isTokenValid=validateToken(token);
               if(isTokenValid)
               {
                  long userAccountId=getAccountIdFromToken(token);
                  var account=await dbContext.UserAccounts.FindAsync(userAccountId);
                  context.Items["CurrentUser"]=account;
               }
               //else do nothing, proceed without adding current user account to the context                         
        }

        private bool validateToken(string token)
        {
            var jwtSecret = _appConfigSettings.JwtSecret;
	        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret));
	        var claimsIssuer = _appConfigSettings.JwtClaimsIssuer;
	        var claimsAudience = _appConfigSettings.JwtClaimsAudience;

	        var tokenHandler = new JwtSecurityTokenHandler();
	        try
	        {
	           	tokenHandler.ValidateToken(token, new TokenValidationParameters
		         {
		             	ValidateIssuerSigningKey = true,
			            ValidateIssuer = true,
		            	ValidateAudience = true,
		             	ValidIssuer = claimsIssuer,
		            	ValidAudience = claimsAudience,
		            	IssuerSigningKey = securityKey
		         }, out SecurityToken validatedToken);
	       }
	       catch
	       {
		       return false;
	       }
	           return true;
        }

        private long getAccountIdFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
	        var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            /*
             this is the form:
             var stringClaimValue = securityToken.Claims.First(claim => claim.Type == claimType).Value;
             where claimType could be passed as argument
            */
            long id=long.Parse(securityToken.Claims.First(x=> x.Type=="id").Value);
            return id;
        }
    }
}