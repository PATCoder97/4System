using System;
using System.Collections.Generic;
using System.Linq;
using Winform4System.Core.Models;
using Winform4System.DataAccess.Models;
using Winform4System.DataAccess.Repositories;

namespace Winform4System.Business.Services
{
    public sealed class EfMainMenuService : IMainMenuService
    {
        private readonly IMainMenuRepository _repository;
        private readonly string _userId;

        public EfMainMenuService(IMainMenuRepository repository, string userId)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _userId = userId;
        }

        public IReadOnlyList<MenuItemDefinition> GetMenuItems()
        {
            return _repository.GetAuthorizedMenuItems(_userId)
                .Select(item => new MenuItemDefinition(
                    item.Code,
                    item.GroupName,
                    item.Title,
                    item.Description ?? string.Empty,
                    ParseDevelopmentStatus(item.DevelopmentStatus),
                    item.IsWide))
                .ToList();
        }

        private static MenuDevelopmentStatus ParseDevelopmentStatus(string value)
        {
            switch ((value ?? string.Empty).Trim().ToUpperInvariant())
            {
                case "COMPLETED":
                    return MenuDevelopmentStatus.Completed;
                case "IN_PROGRESS":
                    return MenuDevelopmentStatus.InProgress;
                default:
                    return MenuDevelopmentStatus.NotStarted;
            }
        }
    }
}
