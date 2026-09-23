using System.Collections.Generic;
using Winform4System.Core.Models;

namespace Winform4System.Business.Services
{
    public interface IMainMenuService
    {
        IReadOnlyList<MenuItemDefinition> GetMenuItems();
    }
}
