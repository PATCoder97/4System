using System;
using Winform4System.DataAccess.Models;

namespace Winform4System.DataAccess.Repositories
{
    public interface IUserAccountRepository
    {
        UserAccountRecord FindByUserId(string userId);
        DateTime? RecordFailedLogin(string userId, int maximumAttempts, int lockoutMinutes);
        bool RecordSuccessfulLogin(string userId);
    }
}
