using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraGrid.Views.Grid;
using TheepClass;
using DBConnection;
using System.Collections.Generic;
using DevExpress.XtraGrid.Views.BandedGrid;

namespace DEV04
{
    public partial class DEV04 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private Functionality.Function FUNC = new Functionality.Function();
        List<PatternZone> PZone;

        public DEV04()
        {
            InitializeComponent();
            UserLookAndFeel.Default.StyleChanged += MyStyleChanged;

            PZone = new List<PatternZone>();
            PZone.Add(new PatternZone { ID = 0, Zone = "Japan" });
            PZone.Add(new PatternZone { ID = 1, Zone = "EU" });
            PZone.Add(new PatternZone { ID = 2, Zone = "US" });
        }

        private void MyStyleChanged(object sender, EventArgs e)
        {
            UserLookAndFeel userLookAndFeel = (UserLookAndFeel)sender;
            cUtility.SaveRegistry(@"Software\MDS", "SkinName", userLookAndFeel.SkinName);
            cUtility.SaveRegistry(@"Software\MDS", "SkinPalette", userLookAndFeel.ActiveSvgPaletteName);
        }

        private void XtraForm1_Load(object sender, EventArgs e)
        {
            LoadData();
            NewData();



            List<string> listSize = new List<string>();
            listSize.Add("S");
            listSize.Add("M");
            listSize.Add("S");
            listSize.Add("M");

            List<string> listColor = new List<string>();
            listColor.Add("03 Gray");
            listColor.Add("03 Gray");
            listColor.Add("16 Red");
            listColor.Add("16 Red");

            gbEstimateCost.Children.Clear();

            int i = 0;
            foreach (string item in listSize) // Loop through List with foreach
            {
                GridBand bandSize = new GridBand();
                bandSize.Name = item + "_" + i.ToString();
                bandSize.Caption = item;
                bandSize.RowCount = 1;
                bandSize.Children.AddBand(listColor[i]);
                gbEstimateCost.Children.Add(bandSize);
                i++;
            }

            

        }

        private void LoadBOM()
        {
            gcBOM.DataSource = null;
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("SELECT B.BOMNo AS [BOM No.], B.ModelName AS Model, B.OIDSIZE, PS.SizeNo AS [Size], PS.SizeName, B.OIDColor, PC.ColorNo AS [Color], PC.ColorName, B.OIDBOM AS ID ");
            sbSQL.Append("FROM   BOM AS B INNER JOIN ");
            sbSQL.Append("       ProductColor AS PC ON B.OIDColor = PC.OIDCOLOR INNER JOIN ");
            sbSQL.Append("       ProductSize AS PS ON B.OIDSIZE = PS.OIDSIZE ");
            if (glueSeason.Text.Trim() != "" || slueStyle.Text.Trim() != "" || glueZone.Text.Trim() != "")
            {
                sbSQL.Append("WHERE (B.OIDBOM > 0)  ");
                if (glueSeason.Text.Trim() != "")
                    sbSQL.Append("AND (B.Season = N'" + glueSeason.EditValue.ToString() + "')  ");
                if (slueStyle.Text.Trim() != "")
                    sbSQL.Append("AND (B.OIDSTYLE = '" + slueStyle.EditValue.ToString() + "')  ");
                if (glueZone.Text.Trim() != "")
                    sbSQL.Append("AND (B.PatternZone = '" + glueZone.EditValue.ToString() + "') ");
            }
            else
                sbSQL.Append("WHERE (B.OIDBOM = 0)  ");
            sbSQL.Append("ORDER BY B.OIDBOM ");

            new ObjDevEx.setGridControl(gcBOM, gvBOM, sbSQL).getData(false, false , false, true);
            gvBOM.Columns["OIDSIZE"].Visible = false;
            gvBOM.Columns["OIDColor"].Visible = false;
            gvBOM.Columns["ID"].Visible = false;
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
            NewData();
        }

        private void LoadData()
        {
            glueZone.Properties.DataSource = PZone;
            glueZone.Properties.DisplayMember = "Zone";
            glueZone.Properties.ValueMember = "ID";
            glueZone.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;

            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("SELECT DISTINCT Season FROM BOM ORDER BY Season");
            new ObjDevEx.setGridLookUpEdit(glueSeason, sbSQL, "Season", "Season").getData();

            sbSQL.Clear();
            sbSQL.Append("SELECT PS.StyleName, GC.CategoryName, PS.OIDSTYLE AS ID ");
            sbSQL.Append("FROM   ProductStyle AS PS INNER JOIN ");
            sbSQL.Append("       GarmentCategory AS GC ON PS.OIDGCATEGORY = GC.OIDGCATEGORY ");
            sbSQL.Append("ORDER BY PS.StyleName");
            new ObjDevEx.setSearchLookUpEdit(slueStyle, sbSQL, "StyleName", "ID").getData();

            LoadBOM();
        }

        private void NewData()
        {
            txeCostSheet.Text = "";
            dteDate.EditValue = DateTime.Now;
            glueSeason.EditValue = "";
            slueStyle.EditValue = "";
            glueZone.EditValue = 0;
            gcBOM.DataSource = null;
        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void bbiExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
        }

        private void bbiPrintPreview_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void bbiPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void glueSeason_EditValueChanged(object sender, EventArgs e)
        {
            LoadBOM();
            slueStyle.Focus();
        }

        private void slueStyle_EditValueChanged(object sender, EventArgs e)
        {
            LoadBOM();
            glueZone.Focus();
        }

        private void glueZone_EditValueChanged(object sender, EventArgs e)
        {
            LoadBOM();
        }

        private void gvBOM_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator) e.Info.DisplayText = (e.RowHandle + 1).ToString();
        }

        private void gvBOM_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            if (e.RowHandle > -1)
            {
                txeCustomer.Text = "";
                txeItemNo.Text = "";
                txePatternNo.Text = "";
                txeModelNo.Text = "";
                txeModelName.Text = "";

                StringBuilder sbSQL = new StringBuilder();
                sbSQL.Append("SELECT TOP (1) B.OIDCUST, CUS.Name AS Customer, B.OIDITEM, IT.Code AS ItemNo, SR.SMPLPatternNo, B.BOMNo, SR.ModelName ");
                sbSQL.Append("FROM   BOM AS B LEFT OUTER JOIN ");
                sbSQL.Append("       SMPLRequest AS SR ON B.OIDSMPL = SR.OIDSMPL LEFT OUTER JOIN ");
                sbSQL.Append("       Customer AS CUS ON B.OIDCUST = CUS.OIDCUST LEFT OUTER JOIN ");
                sbSQL.Append("       Items AS IT ON B.OIDITEM = IT.OIDITEM ");
                sbSQL.Append("WHERE (B.OIDBOM = '" + gvBOM.GetRowCellValue(e.RowHandle, "ID").ToString() + "') ");
                string[] arrBOM = new DBQuery(sbSQL).getMultipleValue();
                if (arrBOM.Length > 0)
                {
                    txeCustomer.Text = arrBOM[1];
                    txeItemNo.Text = arrBOM[3];
                    txePatternNo.Text = arrBOM[4];
                    txeModelNo.Text = arrBOM[5];
                    txeModelName.Text = arrBOM[6];
                }
            }
        }

        private void ribbonControl_Click(object sender, EventArgs e)
        {

        }
    }

    public class PatternZone
    {
        public int ID { get; set; }
        public string Zone { get; set; }
    }
}