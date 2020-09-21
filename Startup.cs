using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AspRest.API.Utils;
using AspRest.API.Middlewares;
using AspRest.API.IoC;
using AspRest.API.Repositories;

namespace AspRest.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
			
            services.AddDbContext<ApplicationDbContext>( options => 
             options.UseMySql(Configuration.GetConnectionString("DefaultConnection")
              ));
            services.AddCors();
            services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.IgnoreNullValues = true);
            services.AddAutoMapper(typeof(Startup));
            services.AddSwaggerGen();

            // configure strongly typed settings object
            services.Configure<AppConfigSettings>(Configuration.GetSection("AppSettings"));

            // configure DI for application services
            
            RegisterServices(services);
        }

        // configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ApplicationDbContext context)
        {
            // migrate database changes on startup (includes initial db creation) 
            context.Database.Migrate();
            // generated swagger json and swagger ui middleware
            app.UseSwagger();
            app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", "ASP.NET Core API"));

            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .SetIsOriginAllowed(origin => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            // global error handler
            app.UseMiddleware<GlobalErrorHandlerMiddleware>();

            // custom jwt auth middleware  19.09.2020 oh yeah!!!
            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(x => x.MapControllers());
        }

        public static void RegisterServices(IServiceCollection services)
        {
            DependencyContainer.RegisterServices(services);
        }
    }
}
