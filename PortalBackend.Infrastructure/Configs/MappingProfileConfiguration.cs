using AutoMapper;
using PortalBackend.Domain.Auth;
using PortalBackend.Domain.Entities;
using PortalBackend.Service.DTO;
using PortalBackend.Service.DTO.Request;
using PortalBackend.Service.DTO.Response;
using System.DirectoryServices.AccountManagement;
using System.Globalization;

namespace PortalBackend.Infrastructure.Configs
{
    public class MappingProfileConfiguration : Profile
    {
        public MappingProfileConfiguration()
        {
            // USER MAPPINGS
            CreateMap<User, UserResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.MobileNumber, opt => opt.MapFrom(src => src.MobileNumber))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Branch, opt => opt.MapFrom(src => src.Branch))
                .ForMember(dest => dest.Group, opt => opt.MapFrom(src => src.Group))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.IsLoggedIn, opt => opt.MapFrom(src => src.IsLoggedIn))
                .ForMember(dest => dest.LastLoginTime, opt => opt.MapFrom(src => src.LastLoginTime))
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId))
                .ForAllOtherMembers(opts => opts.Ignore());

#pragma warning disable CA1416
            CreateMap<UserPrincipal, UserDetailsResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.SamAccountName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailAddress))
                .ForMember(dest => dest.MobileNumber, opt => opt.MapFrom(src => src.VoiceTelephoneNumber))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<User, AuthenticationResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.MobileNumber, opt => opt.MapFrom(src => src.MobileNumber))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Branch, opt => opt.MapFrom(src => src.Branch))
                .ForMember(dest => dest.Group, opt => opt.MapFrom(src => src.Group))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.IsLoggedIn, opt => opt.MapFrom(src => src.IsLoggedIn))
                .ForMember(dest => dest.LastLoginTime, opt => opt.MapFrom(src => src.LastLoginTime))
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<UserDetailsResponse, ValidateUserResponse>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.BranchCode, opt => opt.MapFrom(src => src.BranchCode))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.MobileNo, opt => opt.MapFrom(src => src.MobileNumber))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<User, AuthorizerResponse>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Branch, opt => opt.MapFrom(src => src.Branch))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<AddPinRequest, PinManagement>()
                .ForMember(dest => dest.AccountNo, opt => opt.MapFrom(src => src.AccountNo))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Mobile, opt => opt.MapFrom(src => src.Mobile))
                .ForMember(dest => dest.UpdateType, opt => opt.MapFrom(src => src.UpdateType))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.UserStatus, opt => opt.MapFrom(src => src.UserStatus))
                .ForMember(dest => dest.Authorizer, opt => opt.MapFrom(src => src.Authorizer))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<QueryUserResponseData, ValidatePinUserResponse>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.customer_id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.email))
                .ForMember(dest => dest.Mobile, opt => opt.MapFrom(src => src.mobile))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.user_id))
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.user_role))
                .ForMember(dest => dest.UserStatus, opt => opt.MapFrom(src => src.user_status == "2" ? "Enabled" : "Disabled"))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<PendingPinManagement, PendingPinResponse>()
                .ForMember(dest => dest.AccountNo, opt => opt.MapFrom(src => src.AccountNo))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Mobile, opt => opt.MapFrom(src => src.Mobile))
                .ForMember(dest => dest.UpdateType, opt => opt.MapFrom(src => src.UpdateType))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.UserStatus, opt => opt.MapFrom(src => src.UserStatus))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.DateInitiated, opt => opt.MapFrom(src => src.DateInitiated))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.InitiatingBranch, opt => opt.MapFrom(src => src.InitiatingBranch))
                .ForMember(dest => dest.Initiator, opt => opt.MapFrom(src => src.Initiator))
                .ForMember(dest => dest.InitiatorEmail, opt => opt.MapFrom(src => src.InitiatorEmail))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<PinManagement, PendingPinResponse>()
                .ForMember(dest => dest.AccountNo, opt => opt.MapFrom(src => src.AccountNo))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Mobile, opt => opt.MapFrom(src => src.Mobile))
                .ForMember(dest => dest.UpdateType, opt => opt.MapFrom(src => src.UpdateType))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.UserStatus, opt => opt.MapFrom(src => src.UserStatus))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.DateInitiated, opt => opt.MapFrom(src => src.DateInitiated))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.InitiatingBranch, opt => opt.MapFrom(src => src.InitiatingBranch))
                .ForMember(dest => dest.Initiator, opt => opt.MapFrom(src => src.Initiator))
                .ForMember(dest => dest.InitiatorEmail, opt => opt.MapFrom(src => src.InitiatorEmail))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<PinManagement, CreatePinApiRequest>()
                .ForMember(dest => dest.account_no, opt => opt.MapFrom(src => src.AccountNo))
                .ForMember(dest => dest.customer_id, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.user_id, opt => opt.MapFrom(src => src.UserId))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<PinManagement, ResetPinApiRequest>()
                .ForMember(dest => dest.customer_id, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.user_id, opt => opt.MapFrom(src => src.UserId))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<QueryUserResponseData, GetUserEnquiryDetailsResponse>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.customer_id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.email))
                .ForMember(dest => dest.Mobile, opt => opt.MapFrom(src => src.mobile))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.user_id))
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.user_role))
                .ForMember(dest => dest.UserStatus, opt => opt.MapFrom(src => src.user_status == "2" ? "Enabled" : "Disabled"))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<UserDevice, ValidateUnlockUserResponse>()
                .ForMember(dest => dest.CurrentUserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Uuid))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<PendingDeviceUnlock, PendingUnlockResponse>()
                .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Uuid))
                .ForMember(dest => dest.NewUser, opt => opt.MapFrom(src => src.NewUser))
                .ForMember(dest => dest.CurrentUser, opt => opt.MapFrom(src => src.CurrentUser))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.DateInitiated, opt => opt.MapFrom(src => src.DateInitiated))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.InitiatingBranch, opt => opt.MapFrom(src => src.InitiatingBranch))
                .ForMember(dest => dest.Initiator, opt => opt.MapFrom(src => src.Initiator))
                .ForMember(dest => dest.InitiatorEmail, opt => opt.MapFrom(src => src.InitiatorEmail))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<DeviceUnlock, PendingUnlockResponse>()
                .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Uuid))
                .ForMember(dest => dest.NewUser, opt => opt.MapFrom(src => src.NewUser))
                .ForMember(dest => dest.CurrentUser, opt => opt.MapFrom(src => src.CurrentUser))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.DateInitiated, opt => opt.MapFrom(src => src.DateInitiated))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.InitiatingBranch, opt => opt.MapFrom(src => src.InitiatingBranch))
                .ForMember(dest => dest.Initiator, opt => opt.MapFrom(src => src.Initiator))
                .ForMember(dest => dest.InitiatorEmail, opt => opt.MapFrom(src => src.InitiatorEmail))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<QueryUserResponseData, ValidateProfileUserResponse>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.customer_id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.email))
                .ForMember(dest => dest.MobileNumber, opt => opt.MapFrom(src => src.mobile))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.user_id))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<PendingProfileUpdate, PendingProfileResponse>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Mobile, opt => opt.MapFrom(src => src.Mobile))
                .ForMember(dest => dest.ProfileStatus, opt => opt.MapFrom(src => src.ProfileStatus.ToString()))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.DateInitiated, opt => opt.MapFrom(src => src.DateInitiated))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.InitiatingBranch, opt => opt.MapFrom(src => src.InitiatingBranch))
                .ForMember(dest => dest.Initiator, opt => opt.MapFrom(src => src.Initiator))
                .ForMember(dest => dest.InitiatorEmail, opt => opt.MapFrom(src => src.InitiatorEmail))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<ProfileUpdate, PendingProfileResponse>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Mobile, opt => opt.MapFrom(src => src.Mobile))
                .ForMember(dest => dest.ProfileStatus, opt => opt.MapFrom(src => src.ProfileStatus.ToString()))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.DateInitiated, opt => opt.MapFrom(src => src.DateInitiated))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.InitiatingBranch, opt => opt.MapFrom(src => src.InitiatingBranch))
                .ForMember(dest => dest.Initiator, opt => opt.MapFrom(src => src.Initiator))
                .ForMember(dest => dest.InitiatorEmail, opt => opt.MapFrom(src => src.InitiatorEmail))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<AddProfileRequest, ProfileUpdate>()
                .ForMember(dest => dest.ProfileStatus, opt => opt.MapFrom(src => src.ProfileStatus))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Mobile, opt => opt.MapFrom(src => src.Mobile))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Authorizer, opt => opt.MapFrom(src => src.Authorizer))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<AddUnlockRequest, DeviceUnlock>()
                .ForMember(dest => dest.Authorizer, opt => opt.MapFrom(src => src.Authorizer))
                .ForMember(dest => dest.CurrentUser, opt => opt.MapFrom(src => src.CurrentUser))
                .ForMember(dest => dest.NewUser, opt => opt.MapFrom(src => src.NewUser))
                .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Uuid))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<UserRequest, PendingUserRequest>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailAddress))
                .ForMember(dest => dest.MobileNumber, opt => opt.MapFrom(src => src.MobileNo))
                .ForMember(dest => dest.Group, opt => opt.MapFrom(src => src.StaffGroup))
                .ForMember(dest => dest.Branch, opt => opt.MapFrom(src => src.StaffBranch))
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Role))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<EditUserRequest, PendingUserRequest>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailAddress))
                .ForMember(dest => dest.MobileNumber, opt => opt.MapFrom(src => src.MobileNo))
                .ForMember(dest => dest.Group, opt => opt.MapFrom(src => src.StaffGroup))
                .ForMember(dest => dest.Branch, opt => opt.MapFrom(src => src.StaffBranch))
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Role))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<PendingUserRequest, PendingUserListResponse>()
                .ForMember(dest => dest.DateInitiated, opt => opt.MapFrom(src => src.DateInitiated))
                .ForMember(dest => dest.DefaultRole, opt => opt.MapFrom(src => src.RoleId))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.MobileNo, opt => opt.MapFrom(src => src.MobileNumber))
                .ForMember(dest => dest.InitiatingBranch, opt => opt.MapFrom(src => src.InitiatingBranch))
                .ForMember(dest => dest.Initiator, opt => opt.MapFrom(src => src.Initiator))
                .ForMember(dest => dest.InitiatorEmail, opt => opt.MapFrom(src => src.InitiatorEmail))
                .ForMember(dest => dest.RequestType, opt => opt.MapFrom(src => src.RequestType))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<PendingUserRequest, PendingUserResponse>()
                .ForMember(dest => dest.DateInitiated, opt => opt.MapFrom(src => src.DateInitiated))
                .ForMember(dest => dest.DefaultRole, opt => opt.MapFrom(src => src.RoleId))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.MobileNo, opt => opt.MapFrom(src => src.MobileNumber))
                .ForMember(dest => dest.Branch, opt => opt.MapFrom(src => src.Branch))
                .ForMember(dest => dest.InitiatingBranch, opt => opt.MapFrom(src => src.InitiatingBranch))
                .ForMember(dest => dest.Initiator, opt => opt.MapFrom(src => src.Initiator))
                .ForMember(dest => dest.InitiatorEmail, opt => opt.MapFrom(src => src.InitiatorEmail))
                .ForMember(dest => dest.RequestType, opt => opt.MapFrom(src => src.RequestType.ToString()))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<DeleteUserRequest, PendingUserRequest>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<User, PendingUserRequest>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.MobileNumber, opt => opt.MapFrom(src => src.MobileNumber))
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId))
                .ForMember(dest => dest.Branch, opt => opt.MapFrom(src => src.Branch))
                .ForMember(dest => dest.Group, opt => opt.MapFrom(src => src.Group))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<PendingUserRequest, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.MobileNumber, opt => opt.MapFrom(src => src.MobileNumber))
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId))
                .ForMember(dest => dest.Branch, opt => opt.MapFrom(src => src.Branch))
                .ForMember(dest => dest.Group, opt => opt.MapFrom(src => src.Group))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<AddPinRequest, InitiateRequestNotification>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<AddProfileRequest, InitiateRequestNotification>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<AddUnlockRequest, InitiateRequestNotification>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.CurrentUser))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<DeviceUnlock, AuthorizeRequestNotification>()
                .ForMember(dest => dest.Authorizer, opt => opt.MapFrom(src => src.Authorizer))
                .ForMember(dest => dest.InitiatorsEmail, opt => opt.MapFrom(src => src.InitiatorEmail))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.CurrentUser))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<PinManagement, AuthorizeRequestNotification>()
                .ForMember(dest => dest.Authorizer, opt => opt.MapFrom(src => src.Authorizer))
                .ForMember(dest => dest.InitiatorsEmail, opt => opt.MapFrom(src => src.InitiatorEmail))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<ProfileUpdate, AuthorizeRequestNotification>()
                .ForMember(dest => dest.Authorizer, opt => opt.MapFrom(src => src.Authorizer))
                .ForMember(dest => dest.InitiatorsEmail, opt => opt.MapFrom(src => src.InitiatorEmail))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<PendingUserRequest, WelcomeNotificationRequest>()
                .ForMember(dest => dest.Branch, opt => opt.MapFrom(src => src.Branch))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.RoleId))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<ProfileUpdate, UpdateUserRequest>()
                .ForMember(dest => dest.userId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.customerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForAllOtherMembers(opts => opts.Ignore());
        }
    }
}
