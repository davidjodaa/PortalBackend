using Microsoft.Extensions.Logging;
using System;
using PortalBackend.Service.Contract;
using AutoMapper;
using System.Threading.Tasks;
using PortalBackend.Domain.Settings;
using PortalBackend.Service.DTO.Response;
using PortalBackend.Service.DTO.Request;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Microsoft.Extensions.Options;
using PortalBackend.Service.Helpers;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices;
using System.Runtime.Versioning;
using PortalBackend.Domain.Auth;
using PortalBackend.Service.Exceptions;

namespace PortalBackend.Service.Implementation
{
    public class ActiveDirectoryService : IActiveDirectoryService
    {
        private readonly ILogger<ActiveDirectoryService> _logger;
        private readonly ActiveDirectorySettings _activeDirectorySettings;
        private readonly ApiAuthorizationHeaders _apiHeaders;
        private readonly Secrets _secrets;
        private readonly IMapper _mapper;
        private readonly IAPIImplementation _apiCalls;

        public ActiveDirectoryService(ILogger<ActiveDirectoryService> logger,
            IOptions<ActiveDirectorySettings> activeDirectorySettings,
            IOptions<ApiAuthorizationHeaders> apiHeaders,
            IOptions<Secrets> secrets,
            IMapper mapper,
            IAPIImplementation apiCalls)
        {
            _logger = logger;
            _mapper = mapper;
            _activeDirectorySettings = activeDirectorySettings.Value;
            _apiHeaders = apiHeaders.Value;
            _secrets = secrets.Value;
            _apiCalls = apiCalls;
        }
        public async Task<AuthUserResponse> AuthenticateUser(string userId, string password)
        {
            var encryptedPassWord = EncryptPassword(password, _secrets.adSecretKey);

            var authUserBody = new AuthUserRequest()
            {
                key = _secrets.adUserKey,
                channel = _secrets.adUserChannel,
                username = userId,
                password = encryptedPassWord
            };

            return await _apiCalls.ActiveDirectoryLogin(authUserBody, _secrets.adAuthorizationHeader);
        }

#pragma warning disable CA1416
        public bool Login(string userId, string password)
        {
            try
            {
                PrincipalContext oPrincipalContext = GetPrincipalContext();

                bool isValid = ValidateCredentials(oPrincipalContext, userId, password);
                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                if (ex.Message.Contains("operational"))
                {
                    _logger.LogWarning("The active directory server is not Operational at the moment");
                    throw new ApiException($"The active directory server is not Operational at the moment.");
                }
                _logger.LogWarning("Login Failure");
                throw new ApiException($"Invalid username or password.");
            }
        }

        public UserDetailsResponse GetUserDetails(string userName)
        {
            using DirectoryEntry context = createDirectoryEntry();
            DirectorySearcher search = new DirectorySearcher(context);
            search.Filter = "(sAMAccountName=" + userName + ")";

            // create an array of properties that we would like and  
            // add them to the search object  

            string[] requiredProperties = new string[] { "sAMAccountName", "cn", "department", "extensionAttribute9", "mail", "mobile", "streetAddress", "memberOf" };

            foreach (string property in requiredProperties)
                search.PropertiesToLoad.Add(property);

            SearchResult result = search.FindOne();

            if (result != null)
            {
                UserDetailsResponse userDetailsResponse = new UserDetailsResponse()
                {
                    UserName = result.GetAdPropertyValue("samaccountname"),
                    Name = result.GetAdPropertyValue("cn"),
                    BranchCode = result.GetAdPropertyValue("extensionattribute9"),
                    Email = result.GetAdPropertyValue("mail"),
                    MobileNumber = result.GetAdPropertyValue("mobile"),
                    Department = result.GetAdPropertyValue("department"),
                    Groups = result.GetAdPropertyArray("memberof")
                };
                return userDetailsResponse;
            }
            else
            {
                _logger.LogWarning("No user found with the search parameter.");
                throw new ApiException("No user found with the search parameter.");
            }
        }

        /********************************
         * Private Functions
         ********************************/
        protected virtual bool ValidateCredentials(PrincipalContext principalContext, string username, string plainPassword)
        {
            return principalContext.ValidateCredentials(username, plainPassword, ContextOptions.Negotiate);
        }

        private PrincipalContext GetPrincipalContext()
        {

            var oPrincipalContext = new PrincipalContext(ContextType.Domain, _activeDirectorySettings.LdapIp, $"{_activeDirectorySettings.DomainUser}\\{_activeDirectorySettings.AdUserName}", _activeDirectorySettings.AdPassword);
            return oPrincipalContext;
        }
        private DirectoryEntry createDirectoryEntry()
        {
            // create and return new LDAP connection with desired settings  

            DirectoryEntry ldapConnection = new DirectoryEntry(_activeDirectorySettings.LdapCred);
            //ldapConnection.Path = _activeDirectorySettings.LdapPath;
            ldapConnection.Username = $"{_activeDirectorySettings.DomainUser}\\{_activeDirectorySettings.AdUserName}";
            ldapConnection.Password = _activeDirectorySettings.AdPassword;
            ldapConnection.AuthenticationType = AuthenticationTypes.Secure;
            return ldapConnection;
        }
        private byte[] SetSecretKey(string keyvalue)
        {
            try
            {
                if (string.IsNullOrEmpty(keyvalue))
                {
                    // _logger.LogInformation($"Key value for encryting password is empty");
                    return null;
                }
                byte[] keybytes = new byte[16];
                byte[] bytes = Encoding.Default.GetBytes(keyvalue);

                //keyvalue = Encoding.UTF8.GetString(bytes);

                byte[] result;
                SHA1 shaM = new SHA1Managed();
                result = shaM.ComputeHash(bytes);
                Array.Copy(result, keybytes, 16);

                /// this converts it to a string
                var sb = new StringBuilder(keybytes.Length * 2);
                foreach (byte b in keybytes)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("x2"));
                }

                Console.WriteLine(sb.ToString());
                return keybytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred");
                return null;
            }
        }
        private static byte[] Key { get; set; }
        private string EncryptPassword(string passwordToEncrypt, string secretKey)
        {
            try
            {
                byte[] encryptedBtye;

                using (var cipher = new AesManaged())
                {
                    Key = SetSecretKey(secretKey);
                    if (Key == null)
                    {
                        return null;
                    }

                    cipher.Key = Key;
                    cipher.IV = new byte[16];
                    cipher.Mode = CipherMode.ECB;
                    cipher.Padding = PaddingMode.PKCS7;


                    ICryptoTransform encryptor = cipher.CreateEncryptor(cipher.Key, cipher.IV);

                    MemoryStream msEncrypt = new MemoryStream();
                    CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))

                        swEncrypt.Write(passwordToEncrypt);

                    encryptedBtye = msEncrypt.ToArray();
                }

                var encryptedPassWord = Convert.ToBase64String(encryptedBtye);

                Console.WriteLine(encryptedPassWord);
                return encryptedPassWord;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while encrypting password");
                return null;
            }
        }
    }
}
