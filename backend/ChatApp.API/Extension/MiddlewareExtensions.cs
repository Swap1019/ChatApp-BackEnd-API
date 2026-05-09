using Microsoft.AspNetCore.Builder;
using ChatApp.API.Middleware;

namespace ChatApp.API.Extension
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseUserState(this IApplicationBuilder app)
        {
            return app.UseMiddleware<UserStateMiddleware>();
        }
    }   
}