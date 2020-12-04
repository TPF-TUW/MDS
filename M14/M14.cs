using System;
using System.Text;
using DBConnection;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Extensions;
using MDS00;
using System.Drawing;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;

namespace M14
{
    public partial class M14 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private Functionality.Function FUNC = new Functionality.Function();
        bool chkShow = false;
        public M14()
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
            sbSQL.Append("SELECT Code, Name, ShortName, OIDCUST AS ID ");
            sbSQL.Append("FROM  Customer ");
            sbSQL.Append("ORDER BY Name ");
            new ObjDevEx.setSearchLookUpEdit(slueCode, sbSQL, "Name", "ID").getData(true);

            sbSQL.Clear();
            sbSQL.Append("SELECT CUS.Code, CUS.Name, ADDR.Code AS DestinationCode, ADDR.ShipToName, ADDR.ShipToAddress1, ADDR.ShipToAddress2, ADDR.ShipToAddress3, ADDR.Country, ADDR.PostCode, ADDR.TelephoneNo, ADDR.FaxNo, ");
            sbSQL.Append("       ADDR.CreatedBy, ADDR.CreatedDate, ADDR.UpdatedBy, ADDR.UpdatedDate, ADDR.OIDCUST AS CUSID, ADDR.OIDCUSTAdd AS ID ");
            sbSQL.Append("FROM   Customer AS CUS INNER JOIN ");
            sbSQL.Append("       CustomerAddress AS ADDR ON CUS.OIDCUST = ADDR.OIDCUST ");
            sbSQL.Append("ORDER BY CUS.Code, DestinationCode ");
            new ObjDevEx.setGridControl(gcCustDes, gvCustDes, sbSQL).getDataShowOrder(false, false, false, true);
        }

        private void NewData()
        {
            slueCode.EditValue = "";
            lblStatus.Text = "* Add Destination";
            lblStatus.ForeColor = Color.Green;

            txeID.Text = new DBQuery("SELECT CASE WHEN ISNULL(MAX(OIDCUSTAdd), '') = '' THEN 1 ELSE MAX(OIDCUSTAdd) + 1 END AS NewNo FROM CustomerAddress").getString();
            txeCustCode.Text = "";
            txeDes.Text = "";
            txeShip.Text = "";
            txeAddr1.Text = "";
            txeAddr2.Text = "";
            txeAddr3.Text = "";
            txeCountry.Text = "";
            txePostCode.Text = "";
            txeTelNo.Text = "";
            txeFaxNo.Text = "";

            txeCREATE.Text = "0";
            txeCDATE.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            txeUPDATE.Text = "0";
            txeUDATE.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            ////txeID.Focus();
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
            NewData();
        }

        private void gvGarment_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            
        }

        private void slueCode_EditValueChanged(object sender, EventArgs e)
        {
            if (chkShow == false)
            {
                lblStatus.Text = "* Add Destination";
                lblStatus.ForeColor = Color.Green;

                txeID.Text = new DBQuery("SELECT CASE WHEN ISNULL(MAX(OIDCUSTAdd), '') = '' THEN 1 ELSE MAX(OIDCUSTAdd) + 1 END AS NewNo FROM CustomerAddress").getString();
                object xcode = slueCode.Properties.View.GetFocusedRowCellValue("Code");
                if (xcode != null)
                {
                    txeCustCode.Text = xcode.ToString();
                }
                else
                {
                    txeCustCode.Text = "";
                }

                txeDes.Text = "";
                txeShip.Text = "";
                txeAddr1.Text = "";
                txeAddr2.Text = "";
                txeAddr3.Text = "";
                txeCountry.Text = "";
                txePostCode.Text = "";
                txeTelNo.Text = "";
                txeFaxNo.Text = "";

                txeCREATE.Text = "0";
                txeCDATE.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                txeUPDATE.Text = "0";
                txeUDATE.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");


                StringBuilder sbSQL = new StringBuilder();
                if (slueCode.Text.Trim() != "")
                {
                    sbSQL.Append("SELECT CUS.Code, CUS.Name, ADDR.Code AS DestinationCode, ADDR.ShipToName, ADDR.ShipToAddress1, ADDR.ShipToAddress2, ADDR.ShipToAddress3, ADDR.Country, ADDR.PostCode, ADDR.TelephoneNo, ADDR.FaxNo, ");
                    sbSQL.Append("       ADDR.CreatedBy, ADDR.CreatedDate, ADDR.UpdatedBy, ADDR.UpdatedDate, ADDR.OIDCUST AS CUSID, ADDR.OIDCUSTAdd AS ID ");
                    sbSQL.Append("FROM   Customer AS CUS INNER JOIN ");
                    sbSQL.Append("       CustomerAddress AS ADDR ON CUS.OIDCUST = ADDR.OIDCUST ");
                    sbSQL.Append("WHERE (ADDR.OIDCUST = '" + slueCode.EditValue.ToString() + "') ");
                    sbSQL.Append("ORDER BY CUS.Code, DestinationCode ");
                }
                else
                {
                    sbSQL.Append("SELECT CUS.Code, CUS.Name, ADDR.Code AS DestinationCode, ADDR.ShipToName, ADDR.ShipToAddress1, ADDR.ShipToAddress2, ADDR.ShipToAddress3, ADDR.Country, ADDR.PostCode, ADDR.TelephoneNo, ADDR.FaxNo, ");
                    sbSQL.Append("       ADDR.CreatedBy, ADDR.CreatedDate, ADDR.UpdatedBy, ADDR.UpdatedDate, ADDR.OIDCUST AS CUSID, ADDR.OIDCUSTAdd AS ID ");
                    sbSQL.Append("FROM   Customer AS CUS INNER JOIN ");
                    sbSQL.Append("       CustomerAddress AS ADDR ON CUS.OIDCUST = ADDR.OIDCUST ");
                    sbSQL.Append("ORDER BY CUS.Code, DestinationCode ");
                }
                new ObjDevEx.setGridControl(gcCustDes, gvCustDes, sbSQL).getDataShowOrder(false, false, false, true);
                chkShow = false;
                txeDes.Focus();
            }
        }


        private void gvCustDes_RowStyle(object sender, RowStyleEventArgs e)
        {
            
        }

        private void txeDes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeShip.Focus();
            }
        }

        private void txeDes_LostFocus(object sender, EventArgs e)
        {
            
        }

        private void txeShip_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeAddr1.Focus();
            }
        }

        private void txeAddr1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeAddr2.Focus();
            }
        }

        private void txeAddr2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeAddr3.Focus();
            }
        }

        private void txeAddr3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeCountry.Focus();
            }
        }

        private void txeCountry_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txePostCode.Focus();
            }
        }

        private void txePostCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeTelNo.Focus();
            }
        }

        private void txeTelNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeFaxNo.Focus();
            }
        }

        private void txeFaxNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeCREATE.Focus();
            }
        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (slueCode.Text.Trim() == "")
            {
                FUNC.msgWarning("Please input customer.");
                slueCode.Focus();
            }
            else if (txeDes.Text.Trim() == "")
            {
                FUNC.msgWarning("Please input destination code.");
                txeDes.Focus();
            }
            else if(txeShip.Text.Trim() == "")
            {
                FUNC.msgWarning("Please input ship to name.");
                txeShip.Focus();
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

                    string strUPDATE = "0";
                    if (txeUPDATE.Text.Trim() != "")
                    {
                        strUPDATE = txeUPDATE.Text.Trim();
                    }

                    bool chkGMP = chkDuplicate();
                    if (chkGMP == true)
                    {
                        if (lblStatus.Text == "* Add Destination")
                        {
                            sbSQL.Append("  INSERT INTO CustomerAddress(OIDCUST, Code, ShipToName, ShipToAddress1, ShipToAddress2, ShipToAddress3, Country, PostCode, TelephoneNo, FaxNo, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) ");
                            sbSQL.Append("  VALUES('" + slueCode.EditValue.ToString() + "', N'" + txeDes.Text.Trim().Replace("'", "''") + "', N'" + txeShip.Text.Trim().Replace("'", "''") + "', N'" + txeAddr1.Text.Trim().Replace("'", "''") + "', N'" + txeAddr2.Text.Trim().Replace("'", "''") + "', N'" + txeAddr3.Text.Trim().Replace("'", "''") + "', N'" + txeCountry.Text.Trim() + "', N'" + txePostCode.Text.Trim() + "', N'" + txeTelNo.Text.Trim() + "', ");
                            sbSQL.Append("         N'" + txeFaxNo.Text.Trim() + "', '" + strCREATE + "', GETDATE(), '" + strUPDATE + "', GETDATE()) ");
                        }
                        else if (lblStatus.Text == "* Edit Destination")
                        {
                            sbSQL.Append("  UPDATE CustomerAddress SET ");
                            sbSQL.Append("      OIDCUST='" + slueCode.EditValue.ToString() + "', Code=N'" + txeDes.Text.Trim().Replace("'", "''") + "', ");
                            sbSQL.Append("      ShipToName=N'" + txeShip.Text.Trim().Replace("'", "''") + "', ShipToAddress1=N'" + txeAddr1.Text.Trim().Replace("'", "''") + "', ");
                            sbSQL.Append("      ShipToAddress2=N'" + txeAddr2.Text.Trim().Replace("'", "''") + "', ShipToAddress3=N'" + txeAddr3.Text.Trim().Replace("'", "''") + "', ");
                            sbSQL.Append("      Country=N'" + txeCountry.Text.Trim() + "', PostCode=N'" + txePostCode.Text.Trim() + "', TelephoneNo=N'" + txeTelNo.Text.Trim() + "', ");
                            sbSQL.Append("      FaxNo=N'" + txeFaxNo.Text.Trim() + "', UpdatedBy='" + strUPDATE + "', UpdatedDate=GETDATE() ");
                            sbSQL.Append("  WHERE(OIDCUSTAdd = '" + txeID.Text.Trim() + "') ");
                        }

                        //MessageBox.Show(sbSQL.ToString());
                        if (sbSQL.Length > 0)
                        {
                            try
                            {
                                bool chkSAVE = new DBQuery(sbSQL).runSQL();
                                if (chkSAVE == true)
                                {
                                    FUNC.msgInfo("Save complete.");
                                    bbiNew.PerformClick();
                                }
                            }
                            catch (Exception)
                            { }
                        }
                    }
                }
                else
                {
                    txeDes.Text = "";
                    txeDes.Focus();
                    FUNC.msgWarning("Duplicate destination code. !! Please Change.");
                }
            }
        }


        private bool chkDuplicate()
        {
            bool chkDup = true;
            if (txeDes.Text != "")
            {
                txeDes.Text = txeDes.Text.Trim();

                string strCUSID = "";
                if (slueCode.Text.Trim() != "")
                {
                    strCUSID = slueCode.EditValue.ToString();
                }

                if (txeDes.Text.Trim() != "" && lblStatus.Text == "* Add Destination")
                {
                    StringBuilder sbSQL = new StringBuilder();
                    sbSQL.Append("SELECT TOP(1) Code FROM CustomerAddress WHERE (OIDCUST = '" + strCUSID + "') AND (Code = N'" + txeDes.Text.Trim() + "') ");
                    if (new DBQuery(sbSQL).getString() != "")
                    {
                        chkDup = false;
                    }
                }
                else if (txeDes.Text.Trim() != "" && lblStatus.Text == "* Edit Destination")
                {
                    StringBuilder sbSQL = new StringBuilder();
                    sbSQL.Append("SELECT TOP(1) OIDCUSTAdd ");
                    sbSQL.Append("FROM CustomerAddress ");
                    sbSQL.Append("WHERE (OIDCUST = '" + strCUSID + "') AND (Code = N'" + txeDes.Text.Trim() + "') ");
                    string strCHK = new DBQuery(sbSQL).getString();
                    if (strCHK != "" && strCHK != txeID.Text.Trim())
                    {
                        chkDup = false;
                    }
                }
            }
            return chkDup;
        }

        private void bbiExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string pathFile = new ObjSet.Folder(@"C:\MDS\Export\").GetPath() + "CustomerDestinationList_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
            gvCustDes.ExportToXlsx(pathFile);
            System.Diagnostics.Process.Start(pathFile);
        }

        private void gvCustDes_RowClick(object sender, RowClickEventArgs e)
        {
            if (gvCustDes.IsFilterRow(e.RowHandle)) return;
            lblStatus.Text = "* Edit Destination";
            lblStatus.ForeColor = Color.Red;
            chkShow = true;
            txeID.Text = gvCustDes.GetFocusedRowCellValue("ID").ToString();
            slueCode.EditValue = gvCustDes.GetFocusedRowCellValue("CUSID").ToString();
            txeCustCode.Text = gvCustDes.GetFocusedRowCellValue("Code").ToString();
            txeDes.Text = gvCustDes.GetFocusedRowCellValue("DestinationCode").ToString();
            txeShip.Text = gvCustDes.GetFocusedRowCellValue("ShipToName").ToString();
            txeAddr1.Text = gvCustDes.GetFocusedRowCellValue("ShipToAddress1").ToString();
            txeAddr2.Text = gvCustDes.GetFocusedRowCellValue("ShipToAddress2").ToString();
            txeAddr3.Text = gvCustDes.GetFocusedRowCellValue("ShipToAddress3").ToString();
            txeCountry.Text = gvCustDes.GetFocusedRowCellValue("Country").ToString();
            txePostCode.Text = gvCustDes.GetFocusedRowCellValue("PostCode").ToString();
            txeTelNo.Text = gvCustDes.GetFocusedRowCellValue("TelephoneNo").ToString();
            txeFaxNo.Text = gvCustDes.GetFocusedRowCellValue("FaxNo").ToString();

            txeCREATE.Text = gvCustDes.GetFocusedRowCellValue("CreatedBy").ToString();
            txeCDATE.Text = gvCustDes.GetFocusedRowCellValue("CreatedDate").ToString();
            txeUPDATE.Text = gvCustDes.GetFocusedRowCellValue("UpdatedBy").ToString();
            txeUDATE.Text = gvCustDes.GetFocusedRowCellValue("UpdatedDate").ToString();
        }

        private void bbiPrintPreview_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gcCustDes.ShowPrintPreview();
        }

        private void bbiPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gcCustDes.Print();
        }

        private void txeDes_Leave(object sender, EventArgs e)
        {
            if (txeDes.Text.Trim() != "")
            {
                txeDes.Text = txeDes.Text.ToUpper().Trim();
                bool chkDup = chkDuplicate();
                if (chkDup == false)
                {
                    txeDes.Text = "";
                    txeDes.Focus();
                    FUNC.msgWarning("Duplicate destination code. !! Please Change.");
                }
                else
                {
                    txeShip.Focus();
                }
            }
        }
    }
}