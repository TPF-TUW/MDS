﻿using System;
using System.Text;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Extensions;
using DBConnection;
using MDS00;
using System.Drawing;
using DevExpress.XtraGrid.Views.Grid;

namespace M10
{
    public partial class M10 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private Functionality.Function FUNC = new Functionality.Function();
        //private Functionality.Function FUNC = new Functionality.Function();
        public M10()
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
            sbSQL.Append("SELECT OIDSIZE AS No, SizeNo, SizeName, CreatedBy, CreatedDate ");
            sbSQL.Append("FROM ProductSize ");
            sbSQL.Append("ORDER BY OIDSIZE ");
            new ObjDevEx.setGridControl(gcSize, gvSize, sbSQL).getData(false, false, false, true);

        }

        private void NewData()
        {
            txeID.EditValue = new DBQuery("SELECT CASE WHEN ISNULL(MAX(OIDSIZE), '') = '' THEN 1 ELSE MAX(OIDSIZE) + 1 END AS NewNo FROM ProductSize").getString();
            txeSizeNo.EditValue = "";
            txeSizeName.EditValue = "";

            lblStatus.Text = "* Add Size";
            lblStatus.ForeColor = Color.Green;

            txeCREATE.EditValue = "0";
            txeDATE.EditValue = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            txeSizeNo.Focus();
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
            NewData();
        }

        private void gvGarment_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            
        }

        private void txeSizeNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeSizeName.Focus();
            }
        }

        private void txeSizeNo_LostFocus(object sender, EventArgs e)
        {
            
        }

        private void txeSizeName_LostFocus(object sender, EventArgs e)
        {

        }

        private bool chkDuplicateNo()
        {
            bool chkDup = true;
            if (txeSizeNo.Text != "")
            {
                txeSizeNo.Text = txeSizeNo.Text.Trim();
                if (txeSizeNo.Text.Trim() != "" && lblStatus.Text == "* Add Size")
                {
                    StringBuilder sbSQL = new StringBuilder();
                    sbSQL.Append("SELECT TOP(1) SizeNo FROM ProductSize WHERE (SizeNo = N'" + txeSizeNo.Text.Trim() + "') ");
                    if (new DBQuery(sbSQL).getString() != "")
                    {
                        chkDup = false;
                    }
                }
                else if (txeSizeNo.Text.Trim() != "" && lblStatus.Text == "* Edit Size")
                {
                    StringBuilder sbSQL = new StringBuilder();
                    sbSQL.Append("SELECT TOP(1) OIDSIZE ");
                    sbSQL.Append("FROM ProductSize ");
                    sbSQL.Append("WHERE (SizeNo = N'" + txeSizeNo.Text.Trim().Replace("'", "''") + "') ");
                    string strCHK = new DBQuery(sbSQL).getString();
                    if (strCHK != "" && strCHK != txeID.Text.Trim())
                    {
                        chkDup = false;
                    }
                }
            }
            return chkDup;
        }

        private bool chkDuplicateName()
        {
            bool chkDup = true;
            if (txeSizeName.Text != "")
            {
                txeSizeName.Text = txeSizeName.Text.Trim();
                if (txeSizeName.Text.Trim() != "" && lblStatus.Text == "* Add Size")
                {
                    StringBuilder sbSQL = new StringBuilder();
                    sbSQL.Append("SELECT TOP(1) SizeName FROM ProductSize WHERE (SizeName = N'" + txeSizeName.Text.Trim() + "') ");
                    if (new DBQuery(sbSQL).getString() != "")
                    {
                        FUNC.msgWarning("Duplicate size name. !! Please Change.");
                        txeSizeName.Text = "";
                        txeSizeName.Focus();
                        chkDup = false;
                    }
                }
                else if (txeSizeName.Text.Trim() != "" && lblStatus.Text == "* Edit Size")
                {
                    StringBuilder sbSQL = new StringBuilder();
                    sbSQL.Append("SELECT TOP(1) OIDSIZE ");
                    sbSQL.Append("FROM ProductSize ");
                    sbSQL.Append("WHERE (SizeName = N'" + txeSizeName.Text.Trim().Replace("'", "''") + "') ");
                    string strCHK = new DBQuery(sbSQL).getString();

                    if (strCHK != "" && strCHK != txeID.Text.Trim())
                    {
                        FUNC.msgWarning("Duplicate size name. !! Please Change.");
                        txeSizeName.Text = "";
                        txeSizeName.Focus();
                        chkDup = false;
                    }
                }
            }
            return chkDup;
        }

        private void gvSize_RowStyle(object sender, RowStyleEventArgs e)
        {
            
        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (txeSizeNo.Text.Trim() == "")
            {
                FUNC.msgWarning("Please input size no.");
                txeSizeNo.Focus();
            }
            else if (txeSizeName.Text.Trim() == "")
            {
                FUNC.msgWarning("Please input size name.");
                txeSizeName.Focus();
            }
            else
            {
                if (FUNC.msgQuiz("Confirm save data ?") == true)
                {
                    StringBuilder sbSQL = new StringBuilder();

                    string strCREATE = "0";
                    if (txeCREATE.Text.Trim() != "")
                    {
                        strCREATE = txeCREATE.Text.Trim();
                    }

                    if (lblStatus.Text == "* Add Size")
                    {
                        sbSQL.Append("  INSERT INTO ProductSize(SizeNo, SizeName, CreatedBy, CreatedDate) ");
                        sbSQL.Append("  VALUES(N'" + txeSizeNo.Text.Trim().Replace("'", "''") + "', N'" + txeSizeName.Text.Trim().Replace("'", "''") + "', '" + strCREATE + "', GETDATE()) ");
                    }
                    else if (lblStatus.Text == "* Edit Size")
                    {
                        sbSQL.Append("  UPDATE ProductSize SET ");
                        sbSQL.Append("      SizeNo = N'" + txeSizeNo.Text.Trim().Replace("'", "''") + "', SizeName = N'" + txeSizeName.Text.Trim().Replace("'", "''") + "' ");
                        sbSQL.Append("  WHERE (OIDSIZE = '" + txeID.Text.Trim() + "') ");
                    }

                    //sbSQL.Append("IF NOT EXISTS(SELECT OIDSIZE FROM ProductSize WHERE OIDSIZE = '" + txeID.Text.Trim() + "') ");
                    //sbSQL.Append(" BEGIN ");
                    //sbSQL.Append("  INSERT INTO ProductSize(SizeNo, SizeName, CreatedBy, CreatedDate) ");
                    //sbSQL.Append("  VALUES(N'" + txeSizeNo.Text.Trim().Replace("'", "''") + "', N'" + txeSizeName.Text.Trim().Replace("'", "''") + "', '" + strCREATE + "', GETDATE()) ");
                    //sbSQL.Append(" END ");
                    //sbSQL.Append("ELSE ");
                    //sbSQL.Append(" BEGIN ");
                    //sbSQL.Append("  UPDATE ProductSize SET ");
                    //sbSQL.Append("      SizeNo = N'" + txeSizeNo.Text.Trim().Replace("'", "''") + "', SizeName = N'" + txeSizeName.Text.Trim().Replace("'", "''") + "' ");
                    //sbSQL.Append("  WHERE (OIDSIZE = '" + txeID.Text.Trim() + "') ");
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

        private void bbiExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string pathFile = new ObjSet.Folder(@"C:\MDS\Export\").GetPath() + "SizeList_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
            gvSize.ExportToXlsx(pathFile);
            System.Diagnostics.Process.Start(pathFile);
        }

        private void txeSizeName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeSizeNo.Focus();
            }
        }

        private void gvSize_RowClick(object sender, RowClickEventArgs e)
        {
            if (gvSize.IsFilterRow(e.RowHandle)) return;
            lblStatus.Text = "* Edit Size";
            lblStatus.ForeColor = Color.Red;
            txeID.Text = gvSize.GetFocusedRowCellValue("No").ToString();
            txeSizeNo.Text = gvSize.GetFocusedRowCellValue("SizeNo").ToString();
            txeSizeName.Text = gvSize.GetFocusedRowCellValue("SizeName").ToString();

            txeCREATE.Text = gvSize.GetFocusedRowCellValue("CreatedBy").ToString();
            txeDATE.Text = gvSize.GetFocusedRowCellValue("CreatedDate").ToString();
        }

        private void bbiPrintPreview_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gcSize.ShowPrintPreview();
        }

        private void bbiPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gcSize.Print();
        }

        private void txeSizeNo_Leave(object sender, EventArgs e)
        {
            txeSizeNo.Text = txeSizeNo.Text.ToUpper().Trim();
            bool chkDup = chkDuplicateNo();
            if (chkDup == true)
            {
                txeSizeName.Focus();
            }
            else
            {
                txeSizeNo.Text = "";
                txeSizeNo.Focus();
                FUNC.msgWarning("Duplicate size no. !! Please Change.");
                
            }
        }

        private void txeSizeName_Leave(object sender, EventArgs e)
        {
            txeSizeName.Text = txeSizeName.Text.ToUpper().Trim();
        }
    }
}