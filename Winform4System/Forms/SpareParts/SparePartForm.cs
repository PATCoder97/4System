using System;
using Winform4System.Core.Models;
using Winform4System.Forms.Common;
namespace Winform4System.Forms.SpareParts
{
    public sealed class SparePartForm : ConfiguredModuleForm
    {
        public SparePartForm(UserSession session, string moduleCode = "ASSET.SPARE_PART") : base(moduleCode, "sparePart")
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            SparePartConfiguration.Initialize(session);
        }
    }
}
