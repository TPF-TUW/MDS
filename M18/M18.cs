using System;
using System.Text;
using DBConnection;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Extensions;
using System.Drawing;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout.Utils;
using TheepClass;
using System.Globalization;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;

namespace M18
{
    public partial class M18 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private Functionality.Function FUNC = new Functionality.Function();
        public M18()
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
            LoadData();
            NewData();
        }

        private void LoadData()
        {
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Clear();
            sbSQL.Append("SELECT SeasonNo AS [Season No.], SeasonName AS [Season Name] ");
            sbSQL.Append("FROM Season ");
            sbSQL.Append("ORDER BY OIDSEASON");
            new ObjDevEx.setGridLookUpEdit(glueSeason, sbSQL, "Season No.", "Season No.").getData();

            sbSQL.Clear();
            sbSQL.Append("SELECT Code, Name AS Customer, OIDCUST AS ID ");
            sbSQL.Append("FROM Customer ");
            sbSQL.Append("ORDER BY Code ");
            new ObjDevEx.setSearchLookUpEdit(slueCustomer, sbSQL, "Customer", "ID").getData();

            sbSQL.Clear();
            sbSQL.Append("SELECT PS.StyleName, GC.CategoryName, PS.OIDSTYLE AS ID ");
            sbSQL.Append("FROM   ProductStyle AS PS INNER JOIN ");
            sbSQL.Append("       GarmentCategory AS GC ON PS.OIDGCATEGORY = GC.OIDGCATEGORY ");
            sbSQL.Append("ORDER BY PS.StyleName ");
            new ObjDevEx.setSearchLookUpEdit(slueStyle, sbSQL, "StyleName", "ID").getData();

            LoadItemCustomer();
        }

        private void LoadItemCustomer()
        {
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("SELECT IC.OIDCUST, CUS.Name AS Customer, IC.ItemCode, IC.ItemName, IC.StyleNo, IC.OIDSTYLE, PS.StyleName, IC.Season, IC.FabricWidth, IC.FBComposition, IC.OIDCSITEM AS ID ");
            sbSQL.Append("FROM   ItemCustomer AS IC INNER JOIN ");
            sbSQL.Append("       Customer AS CUS ON IC.OIDCUST = CUS.OIDCUST LEFT OUTER JOIN ");
            sbSQL.Append("       ProductStyle AS PS ON IC.OIDSTYLE = PS.OIDSTYLE ");
            sbSQL.Append("WHERE  (IC.OIDCSITEM > 0) ");
            if(slueCustomer.Text.Trim() != "")
                sbSQL.Append("AND  (IC.OIDCUST = '" + slueCustomer.EditValue.ToString() + "') ");
            if(slueStyle.Text.Trim() != "")
                sbSQL.Append("AND  (IC.OIDSTYLE = '" + slueStyle.EditValue.ToString() + "') ");
            if (glueSeason.Text.Trim() != "")
                sbSQL.Append("AND  (IC.Season = N'" + speSeason.Value.ToString() + glueSeason.EditValue.ToString() + "') ");
            sbSQL.Append("ORDER BY IC.OIDCUST, IC.ItemCode ");
            new ObjDevEx.setGridControl(gcCustItem, gvCustItem, sbSQL).getData(false, false, false, true);
            gvCustItem.Columns["ID"].Visible = false;
            gvCustItem.Columns["OIDCUST"].Visible = false;
            gvCustItem.Columns["OIDSTYLE"].Visible = false;
        }

        private void NewData()
        {
            lblStatus.Text = "* Add Item";
            lblStatus.ForeColor = Color.Green;

            txeID.Text = new DBQuery("SELECT CASE WHEN ISNULL(MAX(OIDCSITEM), '') = '' THEN 1 ELSE MAX(OIDCSITEM) + 1 END AS NewNo FROM ItemCustomer").getString();

            if (Convert.ToInt32(DateTime.Now.ToString("yyyy")) > 2500)
                speSeason.Value = Convert.ToInt32(DateTime.Now.ToString("yyyy")) - 543;
            else
                speSeason.Value = Convert.ToInt32(DateTime.Now.ToString("yyyy"));

            glueSeason.EditValue = "";
            slueCustomer.EditValue = "";
            slueStyle.EditValue = "";
            txeItemCode.Text = "";
            txeItemName.Text = "";
            txeFabricWidth.Text = "";
            txeFBComposition.Text = "";
            txeStyleNo.Text = "";
            txeStyleCode.Text = "";

            slueCustomer.Focus();
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
            NewData();
        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (slueCustomer.Text.Trim() == "")
            {
                FUNC.msgWarning("Please select customer");
                slueCustomer.Focus();
            }
            else if (txeItemCode.Text.Trim() == "")
            {
                FUNC.msgWarning("Please input item code.");
                txeItemCode.Focus();
            }
            else if (txeItemName.Text.Trim() == "")
            {
                FUNC.msgWarning("Please input item name.");
                txeItemName.Focus();
            }
            else
            {
                bool chkDup = chkDuplicate();
                if (chkDup == true)
                {
                    if (FUNC.msgQuiz("Confirm save data ?") == true)
                    {
                        StringBuilder sbSQL = new StringBuilder();

                        string OIDSTYLE = slueStyle.Text.Trim() != "" ? "'" + slueStyle.EditValue.ToString() + "'" : "NULL";
                        string Season = glueSeason.Text.Trim() != "" ? speSeason.Value.ToString() + glueSeason.EditValue.ToString() : "";


                        if (lblStatus.Text == "* Add Item")
                        {
                            sbSQL.Append("  INSERT INTO ItemCustomer(OIDCUST, ItemCode, ItemName, OIDSTYLE, Season, FabricWidth, FBComposition, StyleNo) ");
                            sbSQL.Append("  VALUES(N'" + slueCustomer.EditValue.ToString() + "', N'" + txeItemCode.Text.Trim().Replace("'", "''") + "', N'" + txeItemName.Text.Trim().Replace("'", "''") + "', " + OIDSTYLE + ", N'" + Season + "', N'" + txeFabricWidth.Text.Trim().Replace("'", "''") + "', N'" + txeFBComposition.Text.Trim().Replace("'", "''") + "', N'" + txeStyleNo.Text.Trim() + txeStyleCode.Text.Trim() + "') ");
                        }
                        else if (lblStatus.Text == "* Edit Item")
                        {
                            sbSQL.Append("  UPDATE ItemCustomer SET ");
                            sbSQL.Append("      OIDCUST=N'" + slueCustomer.EditValue.ToString() + "', ");
                            sbSQL.Append("      ItemCode=N'" + txeItemCode.Text.Trim().Replace("'", "''") + "', ");
                            sbSQL.Append("      ItemName=N'" + txeItemName.Text.Trim().Replace("'", "''") + "', ");
                            sbSQL.Append("      OIDSTYLE=" + OIDSTYLE + ", ");
                            sbSQL.Append("      Season=N'" + Season + "', ");
                            sbSQL.Append("      FabricWidth=N'" + txeFabricWidth.Text.Trim().Replace("'", "''") + "', ");
                            sbSQL.Append("      FBComposition=N'" + txeFBComposition.Text.Trim().Replace("'", "''") + "', ");
                            sbSQL.Append("      StyleNo=N'" + txeStyleNo.Text.Trim() + txeStyleCode.Text.Trim() + "' ");
                            sbSQL.Append("  WHERE (OIDCSITEM = '" + txeID.Text.Trim() + "') ");
                        }

                        if (sbSQL.Length > 0)
                        {
                            try
                            {
                                bool chkSAVE = new DBQuery(sbSQL).runSQL();
                                if (chkSAVE == true)
                                {
                                    bbiNew.PerformClick();
                                    FUNC.msgInfo("Save complete.");
                                }
                            }
                            catch (Exception)
                            { }
                        }
                    }
                }
                
            }
        }

        private void bbiExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string pathFile = new ObjSet.Folder(@"C:\MDS\Export\").GetPath() + "CustomerItemList_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
            gvCustItem.ExportToXlsx(pathFile);
            System.Diagnostics.Process.Start(pathFile);
        }

        private void bbiPrintPreview_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gcCustItem.ShowPrintPreview();
        }

        private void bbiPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gcCustItem.Print();
        }

        private void slueStyle_EditValueChanged(object sender, EventArgs e)
        {
            txeStyleCode.Text = slueStyle.Text;
            LoadItemCustomer();
            speSeason.Focus();
        }

        private void bbiRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadItemCustomer();
        }

        private void gvCustItem_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator) e.Info.DisplayText = (e.RowHandle + 1).ToString();
        }

        private void gvCustItem_DoubleClick(object sender, EventArgs e)
        {
            GridView view = (GridView)sender;
            Point pt = view.GridControl.PointToClient(Control.MousePosition);
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo info = view.CalcHitInfo(pt);
            if (info.InRow || info.InRowCell)
            {
                DataTable dtCP = (DataTable)gcCustItem.DataSource;
                if (dtCP.Rows.Count > 0)
                {
                    lblStatus.Text = "* Edit Item";
                    lblStatus.ForeColor = Color.Red;

                    DataRow drCP = dtCP.Rows[info.RowHandle];
                    txeID.Text = drCP["ID"].ToString();

                    string Season = drCP["Season"].ToString();
                    speSeason.Value = Convert.ToInt32(Regex.Match(Season, @"\d+([,\.]\d+)?").Value);
                    glueSeason.EditValue = Season.Replace(Regex.Match(Season, @"\d+([,\.]\d+)?").Value, "");
                    slueCustomer.EditValue = drCP["OIDCUST"].ToString();
                    slueStyle.EditValue = drCP["OIDSTYLE"].ToString();
                    txeItemCode.Text = drCP["ItemCode"].ToString();
                    txeItemName.Text = drCP["ItemName"].ToString();
                    txeFabricWidth.Text = drCP["FabricWidth"].ToString();
                    txeFBComposition.Text = drCP["FBComposition"].ToString();

                    string StyleNo = drCP["StyleNo"].ToString();
                    txeStyleNo.Text = Regex.Match(StyleNo, @"\d+([,\.]\d+)?").Value.ToString();
                    txeStyleCode.Text = StyleNo.Replace(Regex.Match(StyleNo, @"\d+([,\.]\d+)?").Value, "");
                }
            }
        }

        private void txeItemCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                txeItemName.Focus();
        }

        private void txeItemCode_Leave(object sender, EventArgs e)
        {
            if (txeItemCode.Text.Trim() != "")
            {
                txeItemCode.Text = txeItemCode.Text.ToUpper().Trim();
                bool chkDup = chkDuplicate();
                if (chkDup == true)
                    txeItemName.Focus();
            }
        }

        private bool chkDuplicate()
        {
            bool chkDup = true;
            if (txeItemCode.Text.Trim() != "")
            {
                txeItemCode.Text = txeItemCode.Text.ToUpper().Trim().Replace("'", "''");
                string CUST = slueCustomer.Text.Trim() != "" ? slueCustomer.EditValue.ToString() : "";
                if (lblStatus.Text == "* Add Item")
                {
                    StringBuilder sbSQL = new StringBuilder();
                    sbSQL.Append("SELECT TOP(1) ItemCode FROM ItemCustomer WHERE (OIDCUST = '" + CUST + "') AND (ItemCode = N'" + txeItemCode.Text + "') ");
                    if (new DBQuery(sbSQL).getString() != "")
                    {
                        txeItemCode.Text = "";
                        txeItemCode.Focus();
                        FUNC.msgWarning("Duplicate item code. !! Please Change.");
                        chkDup = false;
                    }
                }
                else if (lblStatus.Text == "* Edit Item")
                {
                    StringBuilder sbSQL = new StringBuilder();
                    sbSQL.Append("SELECT TOP(1) OIDCSITEM ");
                    sbSQL.Append("FROM ItemCustomer ");
                    sbSQL.Append("WHERE (OIDCUST = '" + CUST + "') AND (ItemCode = N'" + txeItemCode.Text + "') ");
                    string strCHK = new DBQuery(sbSQL).getString();
                    if (strCHK != "" && strCHK != txeID.Text.Trim())
                    {
                        txeItemCode.Text = "";
                        txeItemCode.Focus();
                        FUNC.msgWarning("Duplicate item code. !! Please Change.");
                        chkDup = false;
                    }
                }
            }
            return chkDup;
        }

        private void slueCustomer_EditValueChanged(object sender, EventArgs e)
        {
            bool chkDup = chkDuplicate();
            if (chkDup == true)
            {
                LoadItemCustomer();
                slueStyle.Focus();
            }
        }

        private void glueSeason_EditValueChanged(object sender, EventArgs e)
        {
            LoadItemCustomer();
            txeItemCode.Focus();
        }

        private void txeItemName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                txeFabricWidth.Focus();
        }

        private void txeFabricWidth_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                txeFBComposition.Focus();
        }

        private void txeFBComposition_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                txeStyleNo.Focus();
        }
    }
}