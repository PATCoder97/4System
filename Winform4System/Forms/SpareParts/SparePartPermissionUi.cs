using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Winform4System.Core.Security;

namespace Winform4System.Forms.SpareParts
{
    public static class SparePartPermissionUi
    {
        public static void Apply(Control owner)
        {
            if (owner == null)
                return;

            ApplyCore(owner);
            owner.HandleCreated += (sender, args) => ApplyCore(owner);
        }

        private static void ApplyCore(Control owner)
        {

            foreach (object component in GetComponents(owner))
            {
                string name = GetName(component);
                string permission = GetRequiredPermission(name);
                if (permission == null)
                    continue;

                bool enabled = HasPermission(permission);
                if (component is BarItem barItem)
                    barItem.Enabled = enabled;
                else if (component is DXMenuItem menuItem)
                    menuItem.Enabled = enabled;
                else if (component is Control control)
                    control.Enabled = enabled;
            }
        }

        private static bool HasPermission(string permission)
        {
            if (permission == "CREATE_OR_UPDATE")
            {
                return CurrentAuthorization.HasPermission("ASSET.SPARE_PART.CREATE")
                    || CurrentAuthorization.HasPermission("ASSET.SPARE_PART.UPDATE")
                    || CurrentAuthorization.HasPermission("ASSET.SPARE_PART.APPROVE");
            }

            return CurrentAuthorization.HasPermission("ASSET.SPARE_PART." + permission);
        }

        private static string GetRequiredPermission(string name)
        {
            string value = (name ?? string.Empty).ToUpperInvariant();
            if (value.Length == 0 || value.Contains("VIEW") || value.Contains("RELOAD") || value.Contains("SEARCH"))
                return null;
            if (value.Contains("EXCEL") || value.Contains("EXPORT") || value.Contains("PRINT") || value.Contains("DOWNLOAD"))
                return "EXPORT";
            if (value.Contains("MANAGEGUIDE"))
                return "ADMIN";
            if (value.Contains("CONFIRMCOMPLETE") || value.Contains("CANCELTICKET") || value.Contains("APPROVE"))
                return "APPROVE";
            if (value.Contains("DELETE") || value.Contains("REMOVE"))
                return "DELETE";
            if (value.Contains("ADD") || value.Contains("CREATE"))
                return "CREATE";
            if (value.Contains("EDIT") || value.Contains("UPDATE") || value.Contains("UPLOAD")
                || value.Contains("DISABLE") || value.Contains("TRANSFER") || value.Contains("PRICE")
                || value.Contains("MATERIALIN") || value.Contains("MATERIALOUT") || value.Contains("CHECK"))
                return "UPDATE";
            if (value.Contains("CONFIRM"))
                return "CREATE_OR_UPDATE";

            return null;
        }

        private static IEnumerable<object> GetComponents(Control owner)
        {
            Type type = owner.GetType();
            while (type != null && type != typeof(object))
            {
                foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    object value = field.GetValue(owner);
                    if (value is BarItem || value is DXMenuItem || value is Control)
                        yield return value;
                }
                type = type.BaseType;
            }
        }

        private static string GetName(object component)
        {
            if (component is BarItem barItem)
                return barItem.Name;
            if (component is Control control)
                return control.Name;
            return component?.GetType().GetProperty("Name")?.GetValue(component, null)?.ToString();
        }
    }
}
