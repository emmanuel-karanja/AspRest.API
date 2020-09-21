//this controller provides the capability to have the current user account
//that's loaded by the JwtMiddleware

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AspRest.API.Entities;

namespace AspRest.API.Controllers
{
 [Controller]
 public abstract class BaseController: ControllerBase
 {
     public UserAccount CurrentUser=> (UserAccount)HttpContext.Items["CurrentUser"];
 }

}