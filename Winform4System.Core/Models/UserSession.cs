namespace Winform4System.Core.Models
{
    public sealed class UserSession
    {
        public long AccountId { get; set; }
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public string Department { get; set; }
        public string Role { get; set; }
    }
}
