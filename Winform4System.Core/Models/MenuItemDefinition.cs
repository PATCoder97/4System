namespace Winform4System.Core.Models
{
    public enum MenuDevelopmentStatus
    {
        NotStarted,
        InProgress,
        Completed
    }

    public sealed class MenuItemDefinition
    {
        public MenuItemDefinition(
            string code,
            string group,
            string title,
            string description,
            MenuDevelopmentStatus developmentStatus,
            bool isWide = true, string navigationTarget = null, string iconKey = null)
        {
            NavigationTarget = navigationTarget;
            IconKey = iconKey;
            Code = code;
            Group = group;
            Title = title;
            Description = description;
            DevelopmentStatus = developmentStatus;
            IsWide = isWide;
        }

        public string NavigationTarget { get; }
        public string IconKey { get; }
        public string Code { get; }
        public string Group { get; }
        public string Title { get; }
        public string Description { get; }
        public MenuDevelopmentStatus DevelopmentStatus { get; }
        public bool IsWide { get; }
    }
}
