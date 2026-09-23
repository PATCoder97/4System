namespace Winform4System.Core.Models
{
    public sealed class UserSession
    {
        public string UserId { get; set; }
        public string DisplayNameTW { get; set; }
        public string DisplayNameVN { get; set; }
        public string DisplayName => !string.IsNullOrWhiteSpace(DisplayNameTW)
            ? DisplayNameTW
            : !string.IsNullOrWhiteSpace(DisplayNameVN)
                ? DisplayNameVN
                : UserId;
        public string Department { get; set; }
        public string Role { get; set; }
    }
}
