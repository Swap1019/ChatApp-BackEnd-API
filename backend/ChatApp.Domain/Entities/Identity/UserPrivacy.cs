using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;

namespace ChatApp.Domain.Entities
{
    public class UserPrivacy
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public PrivacyLevel PhoneNumberPrivacy { get; set; } = PrivacyLevel.ContactsOnly;
        public PrivacyLevel SearchByPhoneNumberPrivacy { get; set; } = PrivacyLevel.ContactsOnly;
        public PrivacyLevel EmailPrivacy { get; set; } = PrivacyLevel.ContactsOnly;
        public PrivacyLevel LastSeenAndOnlinePrivacy { get; set; } = PrivacyLevel.ContactsOnly;
        public PrivacyLevel ProfilePhotoPrivacy { get; set; } = PrivacyLevel.ContactsOnly;
        public PrivacyLevel ForwardedMessagesPrivacy { get; set; } = PrivacyLevel.ContactsOnly;
        public PrivacyLevel CallPrivacy { get; set; } = PrivacyLevel.ContactsOnly;
        public PrivacyLevel BioPrivacy { get; set; } = PrivacyLevel.ContactsOnly;
        public PrivacyLevel GroupInvitationsPrivacy { get; set; } = PrivacyLevel.ContactsOnly;
        public PrivacyLevel ReadReceiptsPrivacy { get; set; } = PrivacyLevel.ContactsOnly;
        public PrivacyLevel AllowDirectMessagesPrivacy { get; set; } = PrivacyLevel.ContactsOnly;
        public PrivacyLevel StoriesPrivacy { get; set; } = PrivacyLevel.ContactsOnly;
        public PrivacyLevel PostsPrivacy { get; set; } = PrivacyLevel.ContactsOnly;
        private DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        private DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        private DateTime? DeletedAt { get; set; }
        private bool IsDeleted { get; set; } = false;
    }
}
