using System;
using System.Text;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Extensions;
using DBConnection;
using MDS00;
using System.Drawing;
using System.Data;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using TheepClass;

namespace M01
{
    public partial class M01 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        //private Functionality.Function FUNC = new Functionality.Function();
        public M01()
        {
            InitializeComponent();
            UserLookAndFeel.Default.StyleChanged += MyStyleChanged;
        }

        private void MyStyleChanged(object sender, EventArgs e)
        {
            UserLookAndFeel userLookAndFeel = (UserLookAndFeel)sender;
            cUtility.SaveRegistry(@"Software\MDS", "SkinName", userLookAndFeel.SkinName);
            cUtility.SaveRegistry(@"Software\MDS", "SkinPalette", userLookAndFeel.ActiveSvgPaletteName);
        }
        private void XtraForm1_Load(object sender, EventArgs e)
        {
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("SELECT FL.FunctionNo, FL.FunctionName, VH.VersionNo ");
            sbSQL.Append("FROM VersionHistory AS VH INNER JOIN ");
            sbSQL.Append("     FunctionList AS FL ON VH.OIDFunction = FL.OIDFunction ");
            sbSQL.Append("WHERE (VH.ActiveType = 1) ");
            sbSQL.Append("ORDER BY FL.FunctionNo ");
            new ObjDevEx.setGridControl(gcAbout, gvAbout, sbSQL).getData(false, false, true, true);
        }
    }
}