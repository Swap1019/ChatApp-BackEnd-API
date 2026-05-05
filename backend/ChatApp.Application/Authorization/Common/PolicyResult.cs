namespace ChatApp.Application.Authorization.Common 
{
    public class PolicyResult
    {
        public bool IsAllowed { get; }
        public string? Reason { get; }

        private PolicyResult(bool isAllowed, string? reason = null)
        {
            IsAllowed = isAllowed;
            Reason = reason;
        }

        public static PolicyResult Allow() => new(true);

        public static PolicyResult Deny(string reason) => new(false, reason);
    }
}

