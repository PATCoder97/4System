using System;
using System.Linq;
using System.Text.RegularExpressions;
using Winform4System.DataAccess;
using Winform4System.DataAccess.Configuration;
using Winform4System.DataAccess.Entities;

public static class ProvisionDomainUser
{
    public static void Main(string[] args)
    {
        if (args.Length != 3 || !Regex.IsMatch(args[0], "^VNW[0-9]{7}$")
            || (args[1] != "Test" && args[1] != "Prod")
            || (args[2] != "STANDARD_USERS" && args[2] != "SYSTEM_ADMINISTRATORS"))
            throw new ArgumentException("Expected: UserId Test|Prod STANDARD_USERS|SYSTEM_ADMINISTRATORS");
        using (var db = new Winform4SystemDbContext(new ConnectionStringProvider().Get(args[1] + "Database")))
        using (var transaction = db.Database.BeginTransaction())
        {
            string userId = args[0];
            if (db.UserAccounts.Any(x => x.UserId == userId)) throw new InvalidOperationException("Tài khoản đã tồn tại; không tự động đổi quyền.");
            string groupCode = args[2];
            var group = db.SecurityGroups.Single(x => x.GroupCode == groupCode && x.IsActive);
            db.UserAccounts.Add(new UserAccount { UserId = userId, IsActive = true, CreatedAt = DateTime.UtcNow, SecurityStamp = Guid.NewGuid() });
            db.SaveChanges();
            db.UserGroups.Add(new UserGroup { UserId = userId, GroupId = group.GroupId, IsActive = true });
            db.AuditLogs.Add(new AuditLog { UserId = userId, ActionCode = "AUTH.ACCOUNT.PROVISIONED", Description = "已由管理工具建立網域帳號。", EntityName = "auth_UserAccount", EntityId = userId });
            db.SaveChanges();
            transaction.Commit();
            Console.WriteLine("Đã tạo {0} trong {1}, môi trường {2}.", userId, groupCode, args[1]);
        }
    }
}
