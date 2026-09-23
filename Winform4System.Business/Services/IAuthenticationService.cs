using Winform4System.Core.Models;

namespace Winform4System.Business.Services
{
    public interface IAuthenticationService
    {
        AuthenticationResult Authenticate(string userId, string password);
    }
}
