using System;
using System.DirectoryServices.AccountManagement;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace Winform4System.Business.Security
{
    public sealed class WindowsDomainCredentialValidator : IDomainCredentialValidator
    {
        public const string DomainName = "vn.fpg.com";

        public DomainCredentialValidationResult Validate(string accountName, string password)
        {
            if (string.IsNullOrWhiteSpace(accountName) || string.IsNullOrEmpty(password))
                return DomainCredentialValidationResult.Invalid;

            if (!IsMachineJoinedToCorporateDomain())
                return DomainCredentialValidationResult.Unavailable;

            try
            {
                using (var context = new PrincipalContext(ContextType.Domain, DomainName))
                {
                    return context.ValidateCredentials(
                        NormalizeUserName(accountName),
                        password,
                        ContextOptions.Negotiate)
                        ? DomainCredentialValidationResult.Valid
                        : DomainCredentialValidationResult.Invalid;
                }
            }
            catch (PrincipalServerDownException)
            {
                return DomainCredentialValidationResult.Unavailable;
            }
            catch (COMException)
            {
                return DomainCredentialValidationResult.Unavailable;
            }
        }

        private static bool IsMachineJoinedToCorporateDomain()
        {
            try
            {
                string machineDomain = IPGlobalProperties.GetIPGlobalProperties().DomainName;
                return string.Equals(machineDomain, DomainName, StringComparison.OrdinalIgnoreCase);
            }
            catch (NetworkInformationException)
            {
                return false;
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
