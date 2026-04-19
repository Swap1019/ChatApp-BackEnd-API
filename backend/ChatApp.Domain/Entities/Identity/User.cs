using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using ChatApp.Domain.Enums;

namespace ChatApp.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }

        public string PasswordHash { get; private set; }

        public bool IsEmailVerified { get; set; }
        public bool IsPhoneNumberVerified { get; set; }

        public int FailedLoginAttempts { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public bool IsLockedOut => LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;

        public string SecurityStamp { get; set; } = Guid.NewGuid().ToString();
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        // Presence system
        public OnlineStatus OnlineStatus { get; set; } = OnlineStatus.Offline;
        public DateTime? LastActiveAt { get; set; }

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }

        //Account Suspension properties
        public UserAccountStatus AccountStatus { get; set; } = UserAccountStatus.Active;
        public bool IsSuspended =>
            AccountStatus == UserAccountStatus.Suspended;
        public bool IsBanned =>
            AccountStatus == UserAccountStatus.Banned;

        // One to many relationship with UserSuspension for tracking suspension history
        public ICollection<UserSuspension> Suspensions { get; set; } = new List<UserSuspension>();

        //One to many relationship with UserSession and UserToken
        public ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
        public ICollection<UserToken> Tokens { get; set; } = new List<UserToken>();

        // Many-to-many relationship with Role for app-level permissions
        public ICollection<UserRole> Roles { get; set; } = new List<UserRole>();

        //Many to many relationship with Conversation through ConversationUser
        public ICollection<ConversationUser> Conversations { get; set; } = new List<ConversationUser>();
        // Conversation bans where this user is the banned participant
        public ICollection<ConversationUserBan> ConversationBans { get; set; } = new List<ConversationUserBan>();

        // Bans this user has issued to other users
        public ICollection<ConversationUserBan> BansIssued { get; set; } = new List<ConversationUserBan>();        
        //One to many relationship with Conversation for tracking which conversations the user created
        public ICollection<Conversation> CreatedConversations { get; set; } = new List<Conversation>();

        // Navigation property for messages sent by this user
        public ICollection<Message> Messages { get; set; } = new List<Message>();

        // One to many relationship with Contact for managing user's contacts
        public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        // One to many relationship with BlockedUser for managing blocked users
        public ICollection<BlockedUser> BlockedUsers { get; set; } = new List<BlockedUser>();
        // Inverse navigation property for users who have blocked this user
        public ICollection<BlockedUser> BlockedByUsers { get; set; } = new List<BlockedUser>();
        public User(string username, string phoneNumber, string email, string passwordHash)
        {
            Id = Guid.NewGuid();

            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is required.", nameof(username));

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number is required.", nameof(phoneNumber));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            if (!email.Contains("@"))
                throw new ArgumentException("Email must contain '@'.", nameof(email));

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash is required.", nameof(passwordHash));

            Username = username;
            PhoneNumber = phoneNumber;
            Email = email;
            PasswordHash = passwordHash;
        }

        public void ChangeEmail(string newEmail)
        {
            if (string.IsNullOrWhiteSpace(newEmail))
                throw new ArgumentException("Email is required.", nameof(newEmail));

            if (!newEmail.Contains("@"))
                throw new ArgumentException("Email must contain '@'.", nameof(newEmail));

            Email = newEmail;
        }

        public void ChangePhoneNumber(string newPhoneNumber)
        {
            // Need to validate phone number format here if necessary
            // Need to validate existing phone number if necessary
            if (string.IsNullOrWhiteSpace(newPhoneNumber))
                throw new ArgumentException("Phone number is required.", nameof(newPhoneNumber));

            PhoneNumber = newPhoneNumber;
        }
    }
}

