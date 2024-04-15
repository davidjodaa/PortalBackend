using System.Collections.Generic;

namespace PortalBackend.Domain.Constants
{
    public static class GeneralConstants
    {
        public const string REINITIATED_COMMENT = "This transaction was cancelled and reinitiated in another branch";
        public static readonly char[] NUMBERS = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        public static readonly char[] ALPHABET = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };
        public static readonly char[] EXCLUDED_CHARACTERS = { '\'', '?', '>', '<', '*', '[', ']', '{', '}', '^' };
    }

    public static class UserAPIStatus
    {
        public const string ACTIVE = "2";
        public const string INACTIVE = "9";
    }

    public static class RoleConstants
    {
        public static readonly string Administrator = "Administrator";
        public static readonly string Initiator = "Initiator";
        public static readonly string Authorizer = "Authorizer";
    }
}
