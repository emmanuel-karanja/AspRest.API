//a custom attribute to allow role based authorization on either classes(service or controllers etc)
//and method level. A pretty neat way to do it.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using AspRest.API.Entities;

namespace AspRest.API.Utils
{
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly IList<Role> _roles;

    //populate the roles list from the params passed in the attribute ala [Authorize(params)]
    public AuthorizeAttribute(params Role[] roles)
    {
        _roles = roles ?? new Role[] { };
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var currentUser = (UserAccount)context.HttpContext.Items["CurrentUser"];
        if (currentUser == null || (_roles.Any() && !_roles.Contains(currentUser.Role)))
        {
            // not logged in or role not authorized
            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
  }
}
