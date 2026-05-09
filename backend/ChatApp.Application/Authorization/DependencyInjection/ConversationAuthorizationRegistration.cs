using Microsoft.Extensions.DependencyInjection;
using ChatApp.Application.Authorization.Queries.Conversations.AddUser;
using ChatApp.Application.Authorization.Queries.Conversations.BanUser;
using ChatApp.Application.Authorization.Queries.Conversations.CreateConversation;
using ChatApp.Application.Authorization.Queries.Conversations.DeleteConversation;
using ChatApp.Application.Authorization.Queries.Conversations.JoinConversation;
using ChatApp.Application.Authorization.Queries.Conversations.ModifyAdminPermissions;
using ChatApp.Application.Authorization.Queries.Conversations.RemoveAdminRole;
using ChatApp.Application.Authorization.Queries.Conversations.UrlGroupCreation;

using ChatApp.Application.Authorization.Policies.Conversations.AddUser;
using ChatApp.Application.Authorization.Policies.Conversations.BanUser;
using ChatApp.Application.Authorization.Policies.Conversations.CreateConversation;
using ChatApp.Application.Authorization.Policies.Conversations.DeleteConversation;
using ChatApp.Application.Authorization.Policies.Conversations.JoinConversation;
using ChatApp.Application.Authorization.Policies.Conversations.ModifyAdminPermissions;
using ChatApp.Application.Authorization.Policies.Conversations.RemoveAdminRole;
using ChatApp.Application.Authorization.Policies.Conversations.UrlGroupCreation;
using ChatApp.Application.Authorization.Rules.Conversations;


namespace ChatApp.Application.Authorization.DependencyInjection
{
    public static class ConversationAuthorizationRegistration
    {
        public static IServiceCollection AddConversationAuthorization(this IServiceCollection services)
        {
            // Add User
            services.AddScoped<AddUserAuthorizationQuery>();
            services.AddScoped<AddUserRules>();
            services.AddScoped<AddUserPolicy>();

            // Ban User
            services.AddScoped<BanUserAuthorizationQuery>();
            services.AddScoped<BanUserRules>();
            services.AddScoped<BanUserPolicy>();

            // Create Conversation
            services.AddScoped<CreateConversationAuthorizationQuery>();
            services.AddScoped<CreateConversationRules>();
            services.AddScoped<CreateConversationPolicy>();

            // Delete Conversation
            services.AddScoped<DeleteConversationAuthorizationQuery>();
            services.AddScoped<DeleteConversationRules>();
            services.AddScoped<DeleteConversationPolicy>();

            // Join Conversation
            services.AddScoped<JoinConversationAuthorizationQuery>();
            services.AddScoped<JoinConversationRules>();
            services.AddScoped<JoinConversationPolicy>();

            // Modify Admin Permissions
            services.AddScoped<ModifyAdminPermissionsAuthorizationQuery>();
            services.AddScoped<ModifyAdminPermissionsRules>();
            services.AddScoped<ModifyAdminPermissionsPolicy>();

            // Remove Admin Role
            services.AddScoped<RemoveAdminRoleAuthorizationQuery>();
            services.AddScoped<RemoveAdminRoleRules>();
            services.AddScoped<RemoveAdminRolePolicy>();

            // Url Group Creation
            services.AddScoped<UrlGroupCreationAuthorizationQuery>();
            services.AddScoped<UrlGroupCreationRules>();
            services.AddScoped<UrlGroupCreationPolicy>();

            return services;
        }
    }
}
