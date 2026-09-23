namespace Winform4System.DataAccess.Models
{
    public sealed class MainMenuItemRecord
    {
        public string Code { get; set; }
        public string GroupName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string DevelopmentStatus { get; set; }
        public bool IsWide { get; set; }
        public int GroupSortOrder { get; set; }
        public int SortOrder { get; set; }
    }
}
