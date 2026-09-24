using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Winform4System.Forms.SpareParts;

namespace Winform4System.Forms.SpareParts
{
    public partial class SparePartDateRangeControl : DevExpress.XtraEditors.XtraUserControl
    {
        public SparePartDateRangeControl()
        {
            InitializeComponent();
            SparePartPermissionUi.Apply(this);

            // Gán giá trị trực tiếp vào thuộc tính trước khi binding
            DateForm = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1).AddDays(20);
            DateTo = DateTime.Now;

            // Thiết lập DataBindings
            dateFrom.DataBindings.Add("DateTime", this, nameof(DateForm), false, DataSourceUpdateMode.OnPropertyChanged);
            dateTo.DataBindings.Add("DateTime", this, nameof(DateTo), false, DataSourceUpdateMode.OnPropertyChanged);
        }

        public DateTime DateForm { get; set; }
        public DateTime DateTo { get; set; }
    }
}
