using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using AspRest.API.Utils;

namespace AspRest.API.Middlewares
{
    public class GlobalErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalErrorHandlerMiddleware(RequestDelegate next)
        {
            _next=next;
        }

        public async Task Invoke(HttpContext context)
        {
            try{
                await _next(context);
            }
            catch(Exception error)
            {
               var response=context.Response;
               response.ContentType="application/json";

               switch(error)
               {
                   case AppException e:
                     response.StatusCode=(int)HttpStatusCode.BadRequest;
                     break;
                //other exception types here
                   default:
                     response.StatusCode=(int) HttpStatusCode.InternalServerError;
                     break;
               }

               var result=JsonSerializer.Serialize(new {message=error?.Message});
               await response.WriteAsync(result);
            }
        }       
    }
}