namespace AuthService.Shared
{
    public static class ErrorMessages
    {
        public const string TYPE_CLAIM_NOT_NULL = "Claim type cannot be null or empty.";
        public const string VALUE_CLAIM_NOT_NULL = "Claim value cannot be null or empty.";
        public const string ROLE_NAME_NOT_NULL = "Role name cannot be null or empty.";
        public const string ROLE_ALREADY_ASSIGNED = "Role already assigned.";
        public const string CLAIM_ALREADY_ASSIGNED = "Claim already assigned";
        public const string CLAIM_NOT_NULL = "Claim cannot be null.";
        public const string USERNAME_NOT_NULL = "Username is required";
        public const string EMAIL_NOT_NULL = "Email is required";
    }
}
