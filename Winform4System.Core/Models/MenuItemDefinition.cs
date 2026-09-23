using System.Drawing;

namespace Winform4System.Core.Models
{
    public sealed class MenuItemDefinition
    {
        public MenuItemDefinition(string code, string group, string title, string description, Color accentColor, bool isWide = true)
        {
            Code = code;
            Group = group;
            Title = title;
            Description = description;
            AccentColor = accentColor;
            IsWide = isWide;
        }

        public string Code { get; }
        public string Group { get; }
        public string Title { get; }
        public string Description { get; }
        public Color AccentColor { get; }
        public bool IsWide { get; }
    }
}
