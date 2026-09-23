using System.Collections.Generic;
using Winform4System.DataAccess.Models;

namespace Winform4System.DataAccess.Repositories
{
    public interface IMainMenuRepository
    {
        IReadOnlyList<MainMenuItemRecord> GetAuthorizedMenuItems(long userId);
    }
}
