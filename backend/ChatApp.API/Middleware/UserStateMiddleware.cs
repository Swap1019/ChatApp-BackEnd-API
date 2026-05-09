using ChatApp.Application.Services;
using ChatApp.API.Extension;

namespace ChatApp.API.Middleware
{
    public class UserStateMiddleware
    {
        private readonly RequestDelegate _next;

        public UserStateMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUserStateService userStateService)
        {
            // If not authenticated → skip (let [Authorize] handle it)
            if (context.User?.Identity?.IsAuthenticated != true)
            {
                await _next(context);
                return;
            }

            var userId = context.User.GetUserId(); // extension method

            var state = await userStateService.GetUserStateAsync(userId, context.RequestAborted);

            if (state == null || state.IsBanned || state.IsSuspended)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }

            await _next(context);
        }
    }
}
