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
using System.IO;
using DevExpress.Spreadsheet;
using System.Text.RegularExpressions;

namespace M16
{
    public partial class M16 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private Functionality.Function FUNC = new Functionality.Function();
        private const string xlsxPathFile = @"\\172.16.0.190\MDS_Project\MDS\ImportFile\Vessel\";
        string LongGestDays = "";
        string VID = "";
        string nowSheet = "";
        public M16()
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
            sbSQL.Append("SELECT Code AS VendorCode, Name AS Vendor, ShotName AS ShortName, OIDVEND AS ID ");
            sbSQL.Append("FROM Vendor ");
            sbSQL.Append("WHERE (VendorType = 5) ");
            sbSQL.Append("ORDER BY VendorCode ");
            new ObjDevEx.setSearchLookUpEdit(slueCarrier, sbSQL, "Vendor", "ID").getData();

            sbSQL.Clear();
            sbSQL.Append("SELECT PortCode, PortName, City, Country, OIDPORT AS ID ");
            sbSQL.Append("FROM PortAndCity ");
            sbSQL.Append("ORDER BY PortCode ");
            new ObjDevEx.setSearchLookUpEdit(slueFrom, sbSQL, "City", "ID").getData();
            new ObjDevEx.setSearchLookUpEdit(slueTo, sbSQL, "City", "ID").getData();

            sbSQL.Clear();
            //sbSQL.Append("SELECT TOP(1) OIDPORT FROM PortAndCity WHERE (City = N'Bangkok') ");
            //slueFrom.EditValue = new DBQuery(sbSQL).getInt();

            //sbSQL.Append("SELECT OIDPayment AS No, Name, Description, DuedateCalculation, Status, CreatedBy, CreatedDate ");
            //sbSQL.Append("FROM PaymentTerm ");
            //sbSQL.Append("ORDER BY OIDPayment ");
            //new ObjDevEx.setGridControl(gcPTerm, gvPTerm, sbSQL).getData(false, false, false, true);

        }

        private void NewData()
        {
            //lblStatus.Text = "* Add Vessel";
            //lblStatus.ForeColor = Color.Green;

            //txeID.Text = new DBQuery("SELECT CASE WHEN ISNULL(MAX(OIDVessel), '') = '' THEN 1 ELSE MAX(OIDVessel) + 1 END AS NewNo FROM Vessel").getString();

            slueCarrier.EditValue = "";
            slueFrom.EditValue = "";
            dteFileDate.EditValue = DateTime.Now;

            teTime.Value = 1;

            slueTo.EditValue = "";

            txeLimit.Text = "";
            txeStdDay.Text = "";
            txeCyCut.Text = "";
            txeEtdEta.Text = "";
            txeEtaWh.Text = "";
            rgStatus.EditValue = 1;

            txeFilePath.Text = "";

            txeCREATE.Text = "0";
            txeDATE.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            spsVessel.CloseCellEditor(DevExpress.XtraSpreadsheet.CellEditorEnterValueMode.Default);
            spsVessel.CreateNewDocument();

            slueCarrier.Focus();
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
            //if (txeName.Text != "")
            //{
            //    txeName.Text = txeName.Text.Trim();
            //    if (txeName.Text.Trim() != "" && lblStatus.Text == "* Add Payment Term")
            //    {
            //        StringBuilder sbSQL = new StringBuilder();
            //        sbSQL.Append("SELECT TOP(1) Name FROM PaymentTerm WHERE (Name = N'" + txeName.Text.Trim() + "') ");
            //        if (new DBQuery(sbSQL).getString() != "")
            //        {
            //            chkDup = false;
            //        }
            //    }
            //    else if (txeName.Text.Trim() != "" && lblStatus.Text == "* Edit Payment Term")
            //    {
            //        StringBuilder sbSQL = new StringBuilder();
            //        sbSQL.Append("SELECT TOP(1) OIDPayment ");
            //        sbSQL.Append("FROM PaymentTerm ");
            //        sbSQL.Append("WHERE (Name = N'" + txeName.Text.Trim() + "') ");
            //        string strCHK = new DBQuery(sbSQL).getString();
            //        if (strCHK != "" && strCHK != txeID.Text.Trim())
            //        {
            //            chkDup = false;
            //        }
            //    }
            //}
            return chkDup;
        }

        private void txeName_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //{
            //    txeDescription.Focus();
            //}
        }

        private void txeName_LostFocus(object sender, EventArgs e)
        {
            
        }

        private void txeDescription_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //{
            //    txeCity.Focus();
            //}
        }

        private void txeDueDate_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //{
            //    rgStatus.Focus();
            //}
        }

        private void gvPTerm_RowStyle(object sender, RowStyleEventArgs e)
        {
            
        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (slueCarrier.Text.Trim() == "")
            {
                FUNC.msgWarning("Please select carrier.");
                slueCarrier.Focus();
            }
            else if (slueFrom.Text.Trim() == "")
            {
                FUNC.msgWarning("Please select from.");
                slueFrom.Focus();
            }
            else if (slueTo.Text.Trim() == "")
            {
                FUNC.msgWarning("Please select destination.");
                slueTo.Focus();
            }
            else if (dteFileDate.Text.Trim() == "")
            {
                FUNC.msgWarning("Please select date.");
                dteFileDate.Focus();
            }
            else if (txeFilePath.Text.Trim() == "")
            {
                FUNC.msgWarning("Please select excel file.");
                txeFilePath.Focus();
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

                    string Status = "NULL";
                    if (rgStatus.SelectedIndex != -1)
                    {
                        Status = rgStatus.Properties.Items[rgStatus.SelectedIndex].Value.ToString();
                    }

                    //**** SAVE EXCEL FILE ******
                    IWorkbook workbook = spsVessel.Document;
                    // Save the modified document to a stream.
                    System.IO.FileInfo fi = new System.IO.FileInfo(txeFilePath.Text);
                    string extn = fi.Extension;
                    string newFileName = slueCarrier.Text.Trim().Replace(" ", "_") + "-" + teTime.Value.ToString() + extn;
                    string newPathFileName = xlsxPathFile + newFileName;
                    using (FileStream stream = new FileStream(newPathFileName, FileMode.Create, FileAccess.ReadWrite))
                    {
                        workbook.SaveDocument(stream, DocumentFormat.Xlsx);
                    }

                    // MessageBox.Show(Status);
                    //if (lblStatus.Text == "* Add Vessel")
                    //{
                    //    sbSQL.Append("  INSERT INTO PaymentTerm(Name, Description, DueDateCalculation, Status, CreatedBy, CreatedDate) ");
                    //    sbSQL.Append("  VALUES(N'" + txeName.Text.Trim().Replace("'", "''") + "', N'" + txeDescription.Text.Trim().Replace("'", "''") + "', N'" + txeCity.Text.Trim().Replace("'", "''") + "', " + Status + ", '" + strCREATE + "', GETDATE()) ");
                    //}
                    //else if (lblStatus.Text == "* Edit Vessel")
                    //{
                    //    sbSQL.Append("  UPDATE PaymentTerm SET ");
                    //    sbSQL.Append("      Name=N'" + txeName.Text.Trim().Replace("'", "''") + "', ");
                    //    sbSQL.Append("      Description=N'" + txeDescription.Text.Trim().Replace("'", "''") + "', ");
                    //    sbSQL.Append("      DueDateCalculation=N'" + txeCity.Text.Trim().Replace("'", "''") + "', ");
                    //    sbSQL.Append("      Status=" + Status + " ");
                    //    sbSQL.Append("  WHERE(OIDPayment = '" + txeID.Text.Trim() + "') ");
                    //}

                    ////MessageBox.Show(sbSQL.ToString());
                    //if (sbSQL.Length > 0)
                    //{
                    //    try
                    //    {
                    //        bool chkSAVE = new DBQuery(sbSQL).runSQL();
                    //        if (chkSAVE == true)
                    //        {
                    //            FUNC.msgInfo("Save complete.");
                    //            bbiNew.PerformClick();
                    //        }
                    //    }
                    //    catch (Exception)
                    //    { }
                    //}
                }
            }

            
        }

        private void bbiExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            IWorkbook workbook = spsVessel.Document;
            Worksheet worksheet2 = workbook.Worksheets[1];
            for (int i = 5; i < worksheet2.GetDataRange().RowCount; i++)
            {
                string SIZE = worksheet2.Rows[i][0].Value.ToString();
                string COLOR_LD = worksheet2.Rows[i][1].Value.ToString();
                string COLOR_TUW = worksheet2.Rows[i][2].Value.ToString();
                string ITEM_CODE = worksheet2.Rows[i][3].Value.ToString();
                string QTY_PCS = worksheet2.Rows[i][4].Value.ToString();
                string SEND = worksheet2.Rows[i][5].Value.ToString();
                string STICKER = worksheet2.Rows[i][6].Value.ToString();
                string REMARK = worksheet2.Rows[i][7].Value.ToString();

                MessageBox.Show(SIZE + ", " + COLOR_LD + ", " + COLOR_TUW + ", " + ITEM_CODE + ", " + QTY_PCS + ", " + SEND + ", " + STICKER + ", " + REMARK);
            }

            //string pathFile = new ObjSet.Folder(@"C:\MDS\Export\").GetPath() + "PaymentTermList_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
            //gvPTerm.ExportToXlsx(pathFile);
            //System.Diagnostics.Process.Start(pathFile);
        }

        private void gvPTerm_RowClick(object sender, RowClickEventArgs e)
        {
            //if (gvPTerm.IsFilterRow(e.RowHandle)) return;

            //lblStatus.Text = "* Edit Payment Term";
            //lblStatus.ForeColor = Color.Red;

            //txeID.Text = gvPTerm.GetFocusedRowCellValue("No").ToString();
            //txeName.Text = gvPTerm.GetFocusedRowCellValue("Name").ToString();
            //txeDescription.Text = gvPTerm.GetFocusedRowCellValue("Description").ToString();
            //txeCity.Text = gvPTerm.GetFocusedRowCellValue("DuedateCalculation").ToString();

            //int status = -1;
            //if (gvPTerm.GetFocusedRowCellValue("Status").ToString() != "")
            //{
            //    status = Convert.ToInt32(gvPTerm.GetFocusedRowCellValue("Status").ToString());
            //}

            //selectStatus(status);

            //txeCREATE.Text = gvPTerm.GetFocusedRowCellValue("CreatedBy").ToString();
            //txeDATE.Text = gvPTerm.GetFocusedRowCellValue("CreatedDate").ToString();
        }

        private void bbiPrintPreview_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //gcPTerm.ShowPrintPreview();
        }

        private void bbiPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //gcPTerm.Print();
        }

        private void txeName_Leave(object sender, EventArgs e)
        {
            //if (txeName.Text.Trim() != "")
            //{
            //    txeName.Text = txeName.Text.ToUpper().Trim();
            //    bool chkDup = chkDuplicate();
            //    if (chkDup == false)
            //    {
            //        txeName.Text = "";
            //        txeName.Focus();
            //        FUNC.msgWarning("Duplicate payment term. !! Please Change.");
            //    }
            //    else
            //    {
            //        txeDescription.Focus();
            //    }
            //}
        }

        private void sbOpenFile_Click(object sender, EventArgs e)
        {
            xtraOpenFileDialog1.Filter = "Excel Files|*.xlsx";
            xtraOpenFileDialog1.FileName = "";
            xtraOpenFileDialog1.Title = "Select Excel File";

            if (xtraOpenFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txeFilePath.Text = xtraOpenFileDialog1.FileName;

                IWorkbook workbook = spsVessel.Document;

                try
                {
                    //workbook.LoadDocument(txeFilePath.Text, DocumentFormat.OpenXml);
                    // Load a workbook from a stream.
                    using (FileStream stream = new FileStream(txeFilePath.Text, FileMode.Open))
                    {
                        workbook.LoadDocument(stream, DocumentFormat.Xlsx);
                        workbook.Worksheets.ActiveWorksheet = workbook.Worksheets[0];

                        LoadSheetHead(workbook.Worksheets[0]);
                    }
                }
                catch (Exception)
                {
                    FUNC.msgWarning("Please close excel file before import.");
                    txeFilePath.Text = "";
                }

                //// Access a collection of worksheets.
                //WorksheetCollection worksheets = workbook.Worksheets;

                // Access a worksheet by its index.
                //Worksheet worksheet2 = workbook.Worksheets[1];

                //// Access a worksheet by its name.
                //Worksheet worksheet2 = workbook.Worksheets["Sheet2"];

               // txeLimit.Text = worksheet2.Rows[0]["B"].DisplayText;


                
            }



        }

        private void spsVessel_SelectionChanged(object sender, EventArgs e)
        {
            Worksheet worksheet = spsVessel.ActiveWorksheet;
            callSheetActive(worksheet);
        }

        private void spsVessel_ActiveSheetChanged(object sender, ActiveSheetChangedEventArgs e)
        {
            //Worksheet worksheet = spsVessel.ActiveWorksheet;
            //callSheetActive(worksheet);
        }

        private void callSheetActive(Worksheet wsActive)
        {
            //if (this.nowSheet != wsActive.Name)
            //{
            //    LoadSheetHead(wsActive);


            //}
            //this.nowSheet = wsActive.Name;
        }


        private void LoadSheetHead(Worksheet wsActive)
        {
            //Set null
            txeLimit.Text = "";
            txeStdDay.Text = "";
            txeCyCut.Text = "";
            txeEtdEta.Text = "";
            txeEtaWh.Text = "";

            //Set Value
            //**** Limit of LCL >> FCL *****
            string LimitOfCBM = "";
            if (wsActive.Rows[1]["E"].DisplayText != "")
            {
                LimitOfCBM = wsActive.Rows[1]["E"].DisplayText;
            }
            else if (wsActive.Rows[1]["F"].DisplayText != "")
            {
                LimitOfCBM = wsActive.Rows[1]["F"].DisplayText;
            }
            else if (wsActive.Rows[1]["G"].DisplayText != "")
            {
                LimitOfCBM = wsActive.Rows[1]["G"].DisplayText;
            }
            LimitOfCBM = Regex.Match(LimitOfCBM, @"\d+([,\.]\d+)?").Value;
            txeLimit.Text = LimitOfCBM;

            //**** Standard days (Longest) ****
            string StdDay = wsActive.Rows[1]["P"].DisplayText.Trim();
            txeStdDay.Text = StdDay;

            //**** Longest Days (Check) ****
            string LongGestDay = wsActive.Rows[2]["P"].DisplayText;
            this.LongGestDays = LongGestDay;

            //**** CY Cut >> ETD ****
            string CyCut = wsActive.Rows[2]["G"].DisplayText;
            CyCut = Regex.Match(CyCut, @"\d+([,\.]\d+)?").Value;
            txeCyCut.Text = CyCut;

            //**** ETD >> ETA ****
            string EtdEta = wsActive.Rows[2]["J"].DisplayText;
            EtdEta = Regex.Match(EtdEta, @"\d+([,\.]\d+)?").Value;
            txeEtdEta.Text = EtdEta;

            //**** ETD >> ETA ****
            string EtaWh = wsActive.Rows[2]["Q"].DisplayText;
            EtaWh = Regex.Match(EtaWh, @"\d+([,\.]\d+)?").Value;
            txeEtaWh.Text = EtaWh;
        }

        private void M16_Shown(object sender, EventArgs e)
        {
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("SELECT TOP(1) OIDPORT FROM PortAndCity WHERE (City = N'Bangkok') ");
            slueFrom.EditValue = new DBQuery(sbSQL).getInt();

        }

        private void slueCarrier_EditValueChanged(object sender, EventArgs e)
        {
            if (slueCarrier.Text.Trim() != "")
            {
                int strTime = new DBQuery("SELECT CASE WHEN ISNULL(MAX(TimeOfDocument), '') = '' THEN 1 ELSE MAX(TimeOfDocument) + 1 END AS NewNo FROM Vessel WHERE (OIDVend = '" + slueCarrier.EditValue.ToString() + "') ").getInt();
                if (strTime == 0)
                {
                    strTime = 1;
                }
                teTime.Value = strTime;
            }
            else
            {
                teTime.Value = 1;
            }
        }

        private void spsVessel_CellEndEdit(object sender, DevExpress.XtraSpreadsheet.SpreadsheetCellValidatingEventArgs e)
        {
            
        }

        private void spsVessel_CellValueChanged(object sender, DevExpress.XtraSpreadsheet.SpreadsheetCellEventArgs e)
        {

        }
    }
}