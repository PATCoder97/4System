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
                DisplayName = "Người dùng demo",
                Department = "Phòng hệ thống",
                Role = "Chưa kết nối đăng nhập"
            };
        }
    }
}
