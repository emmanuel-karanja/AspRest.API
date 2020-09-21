using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using AspRest.API.Services;
using AspRest.API.Repositories;

/* a  way to gather dependencies so that they are not spread all over
and so that we don't have to clutter Startup.cs*/
namespace AspRest.API.IoC
{
    public class DependencyContainer
    {
        public static void RegisterServices(IServiceCollection services)
        {
            //add services from each module
            services.AddScoped<IUserAccountService,UserAccountService>();
            /*..... add more services here....*/

            //add repositories from each module
            services.AddScoped<IUserAccountRepository, UserAccountRepository>();
            /*.....add more repos here.......*/

            /*... any other injectables will be defined here as well..*/
        }
    }
}