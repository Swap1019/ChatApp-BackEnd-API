using ChatApp.Application.Authorization.Common;
using ChatApp.Application.Authorization.Rules;

namespace ChatApp.Application.Authorization.Policies.Conversation
{
    public class ModifyAdminPermissionsPolicy : IPolicy<ModifyAdminPermissionsPolicyRequest>
    {
        private readonly UserRules _userRules;
        private readonly ConversationRules _conversationRules;
        private readonly ConversationAdminRules _conversationAdminRules;

        public ModifyAdminPermissionsPolicy(UserRules userRules, ConversationRules conversationRules, ConversationAdminRules conversationAdminRules)
        {
            _userRules = userRules;
            _conversationRules = conversationRules;
            _conversationAdminRules = conversationAdminRules;
        }

        public async Task<PolicyResult> EvaluateAsync(ModifyAdminPermissionsPolicyRequest request, CancellationToken cancellationToken = default)
        {
            // 1. Self-change check
            if (request.ActorId == request.TargetUserId)
                return PolicyResult.Deny("You cannot change your permission yourself");

            // 2. Actor validation
            var actor = await _userRules.GetUserAsync(request.ActorId, cancellationToken);
            if (actor == null)
                return PolicyResult.Deny("Actor not found");

            if (!_userRules.IsActive(actor))
                return PolicyResult.Deny("Actor is not allowed to perform this action");

            // 3. Target validation
            var target = await _userRules.GetUserAsync(request.TargetUserId, cancellationToken);
            if (target == null)
                return PolicyResult.Deny("Target user not found");

            // 4. Conversation validation
            var conversation = await _conversationRules.GetConversationAsync(request.ConversationId, cancellationToken);
            if (!_conversationRules.Exists(conversation))
                return PolicyResult.Deny("Conversation not found");

            if (!_conversationRules.IsActive(conversation!))
                return PolicyResult.Deny("Conversation is deleted");

            // 5. Membership validation
            if (!await _conversationRules.IsMemberAsync(request.ConversationId, request.ActorId, cancellationToken))
                return PolicyResult.Deny("You are not a member of this conversation");

            if (!await _conversationRules.IsMemberAsync(request.ConversationId, request.TargetUserId, cancellationToken))
                return PolicyResult.Deny("Target user is not a member of this conversation");

            // 6. Exception: Prevent changing the admin permissions from conversation creator/owner
            if (_conversationRules.IsOwner(conversation!, request.TargetUserId))
                return PolicyResult.Deny("You cannot change the permissions of the owner");

            // 7. Check if target is currently an admin
            bool isTargetAdmin = await _conversationRules.IsAdminAsync(request.ConversationId, request.TargetUserId, cancellationToken);
            if (!isTargetAdmin)
                return PolicyResult.Deny("Target user is not an admin in this conversation");

            // 8. Check if actor is banned
            if (await _conversationRules.IsBannedAsync(request.ConversationId, request.ActorId, cancellationToken))
                return PolicyResult.Deny("You are banned from this conversation");

            // 9. Authority checks
            bool isActorOwner = _conversationRules.IsOwner(conversation!, request.ActorId);
            bool isActorAdmin = await _conversationRules.IsAdminAsync(request.ConversationId, request.ActorId, cancellationToken);
            bool canManageRoles = await _conversationAdminRules.CanManageRolesAsync(request.ConversationId, request.ActorId, cancellationToken);
            Guid? grantedBy = await _conversationAdminRules.GetGrantedByAsync(request.ConversationId, request.TargetUserId, cancellationToken);

            if (!isActorOwner && (!isActorAdmin || !canManageRoles))
                return PolicyResult.Deny("You do not have permission to edit admin permissions");

            // 10. Prevent non-owner admin from changing permissions of an admin promoted by the owner
            if (!isActorOwner && grantedBy == conversation!.CreatedById)
                return PolicyResult.Deny("Only the conversation owner can change permissions of this admin");
            
            // 11. Prevent non-owner admin from changing permissions of admins they didn't promote
            if (!isActorOwner && grantedBy != actor.Id)
                return PolicyResult.Deny("Only the promoter of this admin can change permissions of this admin");

            return PolicyResult.Allow();
        }
    
    }
}
