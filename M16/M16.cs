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

namespace M16
{
    public partial class M16 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private Functionality.Function FUNC = new Functionality.Function();
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
            bbiNew.PerformClick();
        }

        private void LoadData()
        {
            //StringBuilder sbSQL = new StringBuilder();
            //sbSQL.Append("SELECT OIDPayment AS No, Name, Description, DuedateCalculation, Status, CreatedBy, CreatedDate ");
            //sbSQL.Append("FROM PaymentTerm ");
            //sbSQL.Append("ORDER BY OIDPayment ");
            //new ObjDevEx.setGridControl(gcPTerm, gvPTerm, sbSQL).getData(false, false, false, true);

        }

        private void NewData()
        {
            //txeName.Text = "";
            //lblStatus.Text = "* Add Payment Term";
            //lblStatus.ForeColor = Color.Green;

            //txeID.Text = new DBQuery("SELECT CASE WHEN ISNULL(MAX(OIDPayment), '') = '' THEN 1 ELSE MAX(OIDPayment) + 1 END AS NewNo FROM PaymentTerm").getString();
            //txeDescription.Text = "";
            //txeCity.Text = "";
            ////rgStatus.SelectedIndex = -1;

            //txeCREATE.Text = "0";
            //txeDATE.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            //////txeID.Focus();
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //LoadData();
            //NewData();
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
            //if (txeName.Text.Trim() == "")
            //{
            //    FUNC.msgWarning("Please name.");
            //    txeName.Focus();
            //}
            //else if (txeDescription.Text.Trim() == "")
            //{
            //    FUNC.msgWarning("Please input description.");
            //    txeDescription.Focus();
            //}
            //else
            //{
            //    if (FUNC.msgQuiz("Confirm save data ?") == true)
            //    {
            //        StringBuilder sbSQL = new StringBuilder();
            //        string strCREATE = "0";
            //        if (txeCREATE.Text.Trim() != "")
            //        {
            //            strCREATE = txeCREATE.Text.Trim();
            //        }

            //        bool chkGMP = chkDuplicate();
            //        if (chkGMP == true)
            //        {
            //            string Status = "NULL";
            //            if (rgStatus.SelectedIndex != -1)
            //            {
            //                Status = rgStatus.Properties.Items[rgStatus.SelectedIndex].Value.ToString();
            //            }

            //           // MessageBox.Show(Status);
            //            if (lblStatus.Text == "* Add Payment Term")
            //            {
            //                sbSQL.Append("  INSERT INTO PaymentTerm(Name, Description, DueDateCalculation, Status, CreatedBy, CreatedDate) ");
            //                sbSQL.Append("  VALUES(N'" + txeName.Text.Trim().Replace("'", "''") + "', N'" + txeDescription.Text.Trim().Replace("'", "''") + "', N'" + txeCity.Text.Trim().Replace("'", "''") + "', " + Status + ", '" + strCREATE + "', GETDATE()) ");
            //            }
            //            else if (lblStatus.Text == "* Edit Payment Term")
            //            {
            //                sbSQL.Append("  UPDATE PaymentTerm SET ");
            //                sbSQL.Append("      Name=N'" + txeName.Text.Trim().Replace("'", "''") + "', ");
            //                sbSQL.Append("      Description=N'" + txeDescription.Text.Trim().Replace("'", "''") + "', ");
            //                sbSQL.Append("      DueDateCalculation=N'" + txeCity.Text.Trim().Replace("'", "''") + "', ");
            //                sbSQL.Append("      Status=" + Status + " ");
            //                sbSQL.Append("  WHERE(OIDPayment = '" + txeID.Text.Trim() + "') ");
            //            }

            //            //MessageBox.Show(sbSQL.ToString());
            //            if (sbSQL.Length > 0)
            //            {
            //                try
            //                {
            //                    bool chkSAVE = new DBQuery(sbSQL).runSQL();
            //                    if (chkSAVE == true)
            //                    {
            //                        FUNC.msgInfo("Save complete.");
            //                        bbiNew.PerformClick();
            //                    }
            //                }
            //                catch (Exception)
            //                { }
            //            }
            //        }
            //        else
            //        {
            //            txeName.Text = "";
            //            txeName.Focus();
            //            FUNC.msgWarning("Duplicate payment term. !! Please Change.");
            //        }
            //    }
            //}
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
            xtraOpenFileDialog1.Filter = "Excel Files|*.xlsx;*.xls;";
            xtraOpenFileDialog1.FileName = "";
            xtraOpenFileDialog1.Title = "Select Excel File";

            if (xtraOpenFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txeFilePath.Text = xtraOpenFileDialog1.FileName;

                IWorkbook workbook = spsVessel.Document;

                // Load a workbook from a stream.
                using (FileStream stream = new FileStream(txeFilePath.Text, FileMode.Open))
                {
                    workbook.LoadDocument(stream, DocumentFormat.Xlsx);
                }


                //// Access a collection of worksheets.
                //WorksheetCollection worksheets = workbook.Worksheets;

                // Access a worksheet by its index.
                Worksheet worksheet2 = workbook.Worksheets[1];

                //// Access a worksheet by its name.
                //Worksheet worksheet2 = workbook.Worksheets["Sheet2"];

                txeLimit.Text = worksheet2.Rows[0]["B"].DisplayText;


                
            }



        }
    }
}