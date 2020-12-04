using System;
using System.Text;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Extensions;
using DBConnection;
using MDS00;
using System.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.XtraGrid.Views.Grid;

namespace M13
{
    public partial class M13 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private Functionality.Function FUNC = new Functionality.Function();
        public M13()
        {
            InitializeComponent();
            UserLookAndFeel.Default.StyleChanged += MyStyleChanged;
            iniConfig = new IniFile("Config.ini");
            UserLookAndFeel.Default.SetSkinStyle(iniConfig.Read("SkinName", "DevExpress"), iniConfig.Read("SkinPalette", "DevExpress"));
        }

        private IniFile iniConfig;

        private void MyStyleChanged(object sender, EventArgs e)
        {
            UserLookAndFeel userLookAndFeel = (UserLookAndFeel)sender;
            LookAndFeelChangedEventArgs lookAndFeelChangedEventArgs = (DevExpress.LookAndFeel.LookAndFeelChangedEventArgs)e;
            //MessageBox.Show("MyStyleChanged: " + lookAndFeelChangedEventArgs.Reason.ToString() + ", " + userLookAndFeel.SkinName + ", " + userLookAndFeel.ActiveSvgPaletteName);
            iniConfig.Write("SkinName", userLookAndFeel.SkinName, "DevExpress");
            iniConfig.Write("SkinPalette", userLookAndFeel.ActiveSvgPaletteName, "DevExpress");
        }

        private void XtraForm1_Load(object sender, EventArgs e)
        {
            bbiNew.PerformClick();
        }

        private void LoadData()
        {
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("SELECT OIDUNIT AS No, UnitName, CreatedBy, CreatedDate ");
            sbSQL.Append("FROM Unit ");
            sbSQL.Append("ORDER BY OIDUNIT ");
            new ObjDevEx.setGridControl(gcUnit, gvUnit, sbSQL).getData(false, false, false, true);

        }

        private void NewData()
        {
            txeUnit.Text = "";
            lblStatus.Text = "* Add Unit";
            lblStatus.ForeColor = Color.Green;

            txeID.Text = new DBQuery("SELECT CASE WHEN ISNULL(MAX(OIDUNIT), '') = '' THEN 1 ELSE MAX(OIDUNIT) + 1 END AS NewNo FROM Unit").getString();

            txeCREATE.Text = "0";
            txeDATE.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
            NewData();
        }

        private void gvGarment_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            
        }

        private bool chkDuplicate()
        {
            bool chkDup = true;
            if (txeUnit.Text != "")
            {
                txeUnit.Text = txeUnit.Text.Trim();
                if (txeUnit.Text.Trim() != "" && lblStatus.Text == "* Add Unit")
                {
                    StringBuilder sbSQL = new StringBuilder();
                    sbSQL.Append("SELECT TOP(1) UnitName FROM Unit WHERE (UnitName = N'" + txeUnit.Text.Trim().Replace("'", "''") + "') ");
                    if (new DBQuery(sbSQL).getString() != "")
                    {
                        chkDup = false;
                    }
                }
                else if (txeUnit.Text.Trim() != "" && lblStatus.Text == "* Edit Unit")
                {
                    StringBuilder sbSQL = new StringBuilder();
                    sbSQL.Append("SELECT TOP(1) OIDUNIT ");
                    sbSQL.Append("FROM Unit ");
                    sbSQL.Append("WHERE (UnitName = N'" + txeUnit.Text.Trim().Replace("'", "''") + "') ");
                    string strCHK = new DBQuery(sbSQL).getString();
                    if (strCHK != "" && strCHK != txeID.Text.Trim())
                    {
                        chkDup = false;
                    }
                }
            }
            return chkDup;
        }

        private void txeUnit_LostFocus(object sender, EventArgs e)
        {
            
        }

        private void txeUnit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeID.Focus();
            }
        }

        private void gvUnit_RowStyle(object sender, RowStyleEventArgs e)
        {
            
        }

        private void bbiExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string pathFile = new ObjSet.Folder(@"C:\MDS\Export\").GetPath() + "UnitList_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
            gvUnit.ExportToXlsx(pathFile);
            System.Diagnostics.Process.Start(pathFile);
        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (txeUnit.Text.Trim() == "")
            {
                FUNC.msgWarning("Please input unit name.");
                txeUnit.Focus();
            }
            else
            {
                txeUnit.Text = txeUnit.Text.Trim();
                bool chkUNIT = chkDuplicate();

                if (chkUNIT == true)
                {
                    if (FUNC.msgQuiz("Confirm save data ?") == true)
                    {
                        StringBuilder sbSQL = new StringBuilder();
                        string strCREATE = "0";
                        if (txeCREATE.Text.Trim() != "")
                        {
                            strCREATE = txeCREATE.Text.Trim();
                        }

                        if (lblStatus.Text == "* Add Unit")
                        {
                            sbSQL.Append("  INSERT INTO Unit(UnitName, CreatedBy, CreatedDate) ");
                            sbSQL.Append("  VALUES(N'" + txeUnit.Text.Trim().Replace("'", "''") + "', '" + strCREATE + "', GETDATE()) ");
                        }
                        else if (lblStatus.Text == "* Edit Unit")
                        {
                            sbSQL.Append("  UPDATE Unit SET ");
                            sbSQL.Append("      UnitName = N'" + txeUnit.Text.Trim().Replace("'", "''") + "' ");
                            sbSQL.Append("  WHERE(OIDUNIT = '" + txeID.Text.Trim() + "') ");
                        }

                        //sbSQL.Append("IF NOT EXISTS(SELECT OIDUNIT FROM Unit WHERE OIDUNIT = N'" + txeID.Text.Trim() + "') ");
                        //sbSQL.Append(" BEGIN ");
                        //sbSQL.Append("  INSERT INTO Unit(UnitName, CreatedBy, CreatedDate) ");
                        //sbSQL.Append("  VALUES(N'" + txeUnit.Text.Trim().Replace("'", "''") + "', '" + strCREATE + "', GETDATE()) ");
                        //sbSQL.Append(" END ");
                        //sbSQL.Append("ELSE ");
                        //sbSQL.Append(" BEGIN ");
                        //sbSQL.Append("  UPDATE Unit SET ");
                        //sbSQL.Append("      UnitName = N'" + txeUnit.Text.Trim().Replace("'", "''") + "' ");
                        //sbSQL.Append("  WHERE(OIDUNIT = '" + txeID.Text.Trim() + "') ");
                        //sbSQL.Append(" END ");
                        //MessageBox.Show(sbSQL.ToString());
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

        private void gvUnit_RowClick(object sender, RowClickEventArgs e)
        {
            if (gvUnit.IsFilterRow(e.RowHandle)) return;
            lblStatus.Text = "* Edit Unit";
            lblStatus.ForeColor = Color.Red;

            txeID.Text = gvUnit.GetFocusedRowCellValue("No").ToString();
            txeUnit.Text = gvUnit.GetFocusedRowCellValue("UnitName").ToString();

            txeCREATE.Text = gvUnit.GetFocusedRowCellValue("CreatedBy").ToString();
            txeDATE.Text = gvUnit.GetFocusedRowCellValue("CreatedDate").ToString();
        }

        private void bbiPrintPreview_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gcUnit.ShowPrintPreview();
        }

        private void bbiPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gcUnit.Print();
        }

        private void txeUnit_Leave(object sender, EventArgs e)
        {
            if (txeUnit.Text.Trim() != "")
            {
                bool chkDup = chkDuplicate();
                if (chkDup == false)
                {
                    txeUnit.Text = "";
                    txeUnit.Focus();
                    FUNC.msgWarning("Duplicate unit. !! Please Change.");
                }
            }
        }
    }
}