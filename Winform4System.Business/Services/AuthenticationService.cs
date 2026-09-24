using System;
using System.Text.RegularExpressions;
using Winform4System.Business.Security;
using Winform4System.Core.Models;
using Winform4System.DataAccess.Models;
using Winform4System.DataAccess.Repositories;

namespace Winform4System.Business.Services
{
    public sealed class AuthenticationService : IAuthenticationService
    {
        private const int MaximumFailedAttempts = 5;
        private const int LockoutMinutes = 15;
        private static readonly Regex UserIdPattern = new Regex(@"^VNW\d{7}$", RegexOptions.CultureInvariant);

        private readonly IUserAccountRepository _repository;
        private readonly PasswordHasher _passwordHasher;
        private readonly string _dummyPasswordHash;

        public AuthenticationService(IUserAccountRepository repository, PasswordHasher passwordHasher)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _dummyPasswordHash = _passwordHasher.Hash("Dummy password used only to balance authentication timing.");
        }

        public AuthenticationResult Authenticate(string userId, string password)
        {
            string normalizedUserId = (userId ?? string.Empty).Trim().ToUpperInvariant();
            if (normalizedUserId.Length == 0 || string.IsNullOrEmpty(password))
                return AuthenticationResult.Failure(AuthenticationFailureReason.MissingCredentials);

            UserAccountRecord account = UserIdPattern.IsMatch(normalizedUserId)
                ? _repository.FindByUserId(normalizedUserId)
                : null;
            if (account != null && account.LockoutEndUtc.HasValue && account.LockoutEndUtc.Value > DateTime.UtcNow)
            {
                _passwordHasher.Verify(password, _dummyPasswordHash);
                return AuthenticationResult.Failure(AuthenticationFailureReason.LockedOut);
            }

            bool eligible = account != null
                && account.IsActive
                && string.Equals(account.AuthenticationType, "LOCAL", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(account.PasswordHash);
            bool passwordValid = _passwordHasher.Verify(
                password,
                eligible ? account.PasswordHash : _dummyPasswordHash);
            bool valid = eligible && passwordValid;

            if (!valid)
            {
                DateTime? lockoutEnd = _repository.RecordFailedLogin(
                    normalizedUserId,
                    MaximumFailedAttempts,
                    LockoutMinutes);

                return AuthenticationResult.Failure(
                    lockoutEnd.HasValue ? AuthenticationFailureReason.LockedOut : AuthenticationFailureReason.InvalidCredentials);
            }

            if (!_repository.RecordSuccessfulLogin(account.UserId))
                return AuthenticationResult.Failure(AuthenticationFailureReason.LockedOut);

            return AuthenticationResult.Success(new UserSession
            {
                UserId = account.UserId,
                DisplayNameTW = account.DisplayNameTW,
                DisplayNameVN = account.DisplayNameVN,
                Department = string.IsNullOrWhiteSpace(account.Department) ? "未設定部門" : account.Department,
                DepartmentCode = account.DepartmentCode,
                Role = string.IsNullOrWhiteSpace(account.Roles) ? "一般使用者" : account.Roles,
                PermissionCodes = account.PermissionCodes ?? new string[0]
            });
        }
    }
}
