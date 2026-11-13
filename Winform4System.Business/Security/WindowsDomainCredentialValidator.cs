using System;
using System.DirectoryServices.Protocols;
using System.Net;

namespace Winform4System.Business.Security
{
    public sealed class WindowsDomainCredentialValidator : IDomainCredentialValidator
    {
        public const string DomainName = "vn.fpg.com";

        public DomainCredentialValidationResult Validate(string accountName, string password)
        {
            if (string.IsNullOrWhiteSpace(accountName) || string.IsNullOrEmpty(password))
                return DomainCredentialValidationResult.Invalid;

            try
            {
                var credentials = new NetworkCredential(NormalizeUserName(accountName), password, DomainName);
                using (var connection = new LdapConnection(
                    new LdapDirectoryIdentifier(DomainName), credentials, AuthType.Negotiate))
                {
                    connection.Timeout = TimeSpan.FromSeconds(10);
                    connection.SessionOptions.ProtocolVersion = 3;
                    connection.SessionOptions.Signing = true;
                    connection.SessionOptions.Sealing = true;
                    // Dùng tài khoản được nhập, không phụ thuộc tài khoản Windows hoặc trạng thái join domain.
                    connection.Bind();
                    return DomainCredentialValidationResult.Valid;
                }
            }
            catch (LdapException exception)
            {
                return ClassifyLdapFailure(exception.ErrorCode);
            }
        }

        internal static DomainCredentialValidationResult ClassifyLdapFailure(int errorCode)
        {
            // Chỉ lỗi kết nối/timeout mới cho phép tầng đăng nhập thử mật khẩu cache.
            switch (errorCode)
            {
                case 51: // LDAP_BUSY
                case 52: // LDAP_UNAVAILABLE
                case 81: // LDAP_SERVER_DOWN
                case 85: // LDAP_TIMEOUT
                case 91: // LDAP_CONNECT_ERROR
                    return DomainCredentialValidationResult.Unavailable;
                default:
                    return DomainCredentialValidationResult.Invalid;
            }
        }

        private static string NormalizeUserName(string accountName)
        {
            string normalized = accountName.Trim();
            int slashIndex = normalized.LastIndexOf('\\');
            if (slashIndex >= 0 && slashIndex < normalized.Length - 1)
                normalized = normalized.Substring(slashIndex + 1);

            int atIndex = normalized.IndexOf('@');
            return atIndex > 0 ? normalized.Substring(0, atIndex) : normalized;
        }
    }
}
