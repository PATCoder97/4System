using System;
using Winform4System.DataAccess.Models;

namespace Winform4System.DataAccess.Repositories
{
    public interface IUserAccountRepository
    {
        UserAccountRecord FindByLoginName(string normalizedLoginName);
        DateTime? RecordFailedLogin(long? userId, string loginName, int maximumAttempts, int lockoutMinutes);
        bool RecordSuccessfulLogin(long userId, string loginName);
    }
}
