
namespace PortalBackend.Domain.Enum
{
    public enum Status
    {
        Active = 1,
        Inactive = 2
    }

    public enum UserRole
    {
        Initiator = 1,
        Authorizer = 2,
        Administrator = 3
    }

    public enum AuthStatus
    {
        Pending,
        Approved,
        Rejected,
        Reinitiated
    }

    public enum PinUpdateType
    {
        CreatePin = 1,
        ResetPin = 2
    }

    public enum ProfileStatus
    {
        Enable = 1,
        Disable = 2
    }

    public enum UserStatus
    {
        Enable = 2,
        Disable = 9
    }

    public enum UserRequestType
    {
        New = 1,
        Edit = 2,
        Delete = 3
    }
}