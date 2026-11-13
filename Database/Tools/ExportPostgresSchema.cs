using System;
using System.Data.Entity.Infrastructure;
using System.IO;
using Winform4System.DataAccess;

// CÃ´ng cá»¥ quáº£n trá»‹ Ä‘á»ƒ xuáº¥t DDL review trÆ°á»›c khi Ã¡p dá»¥ng; á»©ng dá»¥ng khÃ´ng tá»± táº¡o schema.
public static class ExportPostgresSchema
{
    public static void Main(string[] args)
    {
        using (var core = new Winform4SystemDbContext(new Winform4System.DataAccess.Configuration.ConnectionStringProvider().Get()))
        using (var spare = new SparePartDbContext())
        {
            using (var writer = System.Xml.XmlWriter.Create(args[0] + ".core.edmx"))
                EdmxWriter.WriteEdmx(core, writer);
            using (var writer = System.Xml.XmlWriter.Create(args[0] + ".spare.edmx"))
                EdmxWriter.WriteEdmx(spare, writer);
        }
    }
}

