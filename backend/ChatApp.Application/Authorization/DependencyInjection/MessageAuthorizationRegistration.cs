using Microsoft.Extensions.DependencyInjection;
using ChatApp.Application.Authorization.Queries.Messages.PinMessage;
using ChatApp.Application.Authorization.Queries.Messages.SendMessage;
using ChatApp.Application.Authorization.Queries.Messages.DeleteMessage;
using ChatApp.Application.Authorization.Queries.Messages.ReactMessage;

using ChatApp.Application.Authorization.Rules.Messages;
using ChatApp.Application.Authorization.Policies.Messages.PinMessage;
using ChatApp.Application.Authorization.Policies.Messages.SendMessage;
using ChatApp.Application.Authorization.Policies.Messages.DeleteMessage;
using ChatApp.Application.Authorization.Policies.Messages.ReactMessage;

namespace ChatApp.Application.Authorization.DependencyInjection
{
    public static class MessageAuthorizationRegistration
    {
        public static IServiceCollection AddMessageAuthorization(this IServiceCollection services)
        {
            // Send Message
            services.AddScoped<SendMessageAuthorizationQuery>();
            services.AddScoped<SendMessageRules>();
            services.AddScoped<SendMessagePolicy>();

            // Pin Message
            services.AddScoped<PinMessageAuthorizationQuery>();
            services.AddScoped<PinMessageRules>();
            services.AddScoped<PinMessagePolicy>();

            // Delete Message
            services.AddScoped<DeleteMessageAuthorizationQuery>();
            services.AddScoped<DeleteMessageRules>();
            services.AddScoped<DeleteMessagePolicy>();

            // React Message
            services.AddScoped<ReactMessageAuthorizationQuery>();
            services.AddScoped<ReactMessageRules>();
            services.AddScoped<ReactMessagePolicy>();

            return services;
        }
    }
}
