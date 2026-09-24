namespace Winform4System.Core.Models
{
    public enum AuthenticationFailureReason
    {
        None,
        MissingCredentials,
        InvalidCredentials,
        LockedOut,
        DomainUnavailable
    }

    public sealed class AuthenticationResult
    {
        private AuthenticationResult(bool succeeded, AuthenticationFailureReason failureReason, UserSession session)
        {
            Succeeded = succeeded;
            FailureReason = failureReason;
            Session = session;
        }

        public bool Succeeded { get; }
        public AuthenticationFailureReason FailureReason { get; }
        public UserSession Session { get; }

        public static AuthenticationResult Success(UserSession session)
        {
            return new AuthenticationResult(true, AuthenticationFailureReason.None, session);
        }

        public static AuthenticationResult Failure(AuthenticationFailureReason reason)
        {
            return new AuthenticationResult(false, reason, null);
        }
    }
}
