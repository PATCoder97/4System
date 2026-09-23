namespace Winform4System.Core.Models
{
    public sealed class UserSession
    {
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public string Department { get; set; }
        public string Role { get; set; }

        public static UserSession CreateDemo()
        {
            return new UserSession
            {
                UserId = "DEMO001",
                DisplayName = "示範使用者",
                Department = "系統管理部",
                Role = "尚未連接登入服務"
            };
        }
    }
}
