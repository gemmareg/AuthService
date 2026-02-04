namespace AuthService.Shared
{
    public static class ErrorMessages
    {
        public const string TYPE_PERMISSION_NOT_NULL = "Permission type cannot be null or empty.";
        public const string VALUE_PERMISSION_NOT_NULL = "Permission value cannot be null or empty.";
        public const string ROLE_NAME_NOT_NULL = "Role name cannot be null or empty.";
        public const string ROLE_ALREADY_ASSIGNED = "Role already assigned.";
        public const string PERMISSION_ALREADY_ASSIGNED = "Permission already assigned";
        public const string PERMISSION_NOT_NULL = "Permission cannot be null.";
        public const string PERMISSION_DOESNT_EXIST_IN_USER = "Permission doesn't exist in user";
        public const string USERNAME_NOT_NULL = "Username is required";
        public const string EMAIL_NOT_NULL = "Email is required";
        public const string ROLE_NOT_NULL = "Role cannot be null.";
    }
}
