using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;

namespace ChatApp.Domain.Entities
{
    public class UserPrivacyException
    {
        public Guid Id { get; set; }

        public Guid OwnerUserId { get; set; }     // whose privacy
        public User OwnerUser {get; set;} = null!;
        public Guid TargetUserId { get; set; }    // who this rule applies to
        public User TargetUser { get; set; } = null!;

        public PrivacyField Field { get; set; }   // Phone, Photo, etc.

        public PrivacyOverrideType OverrideType { get; set; } // Allow / Deny
    }
}