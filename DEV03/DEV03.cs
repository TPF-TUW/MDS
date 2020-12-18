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

namespace DEV03
{
    public partial class DEV03 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        goClass.dbConn db   = new goClass.dbConn();
        goClass.ctool ct    = new goClass.ctool();
        hardQuery q         = new hardQuery();

        private Functionality.Function FUNC = new Functionality.Function();
        public DEV03()
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
            //bbiNew.PerformClick(); 

            // Set Tabbed
            tabbed_Master.SelectedTabPageIndex = 0;
            tabbedBom.SelectedTabPageIndex = 0;

            q.get_sl_smplNo(sl_smplNo);
            q.get_sl_Customer(sl_Customer);
            q.get_gl_Season(gl_Season);
            q.get_gcListof_Bom(gcListof_Bom); gvListof_Bom.OptionsBehavior.Editable = false;
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
            //txeDueDate.Text = "";
            //rgStatus.SelectedIndex = -1;

            //txeCREATE.Text = "0";
            //txeDATE.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            //////txeID.Focus();
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (tabbed_Master.SelectedTabPageIndex == 0) //List
            {
                tabbed_Master.SelectedTabPageIndex = 1;
            }
            else //Entry
            { 
                //
            }
        }

        private void gvGarment_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            
        }

        private void selectStatus(int value)
        {
            //switch (value)
            //{
            //    case 0:
            //        rgStatus.SelectedIndex = 0;
            //        break;
            //    case 1:
            //        rgStatus.SelectedIndex = 1;
            //        break;
            //    default:
            //        rgStatus.SelectedIndex = -1;
            //        break;
            //}
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
            //            FUNC.msgWarning("Duplicate payment term. !! Please Change.");
            //            txeName.Text = "";
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
            //            FUNC.msgWarning("Duplicate payment term. !! Please Change.");
            //            txeName.Text = "";
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
            //txeName.Text = txeName.Text.ToUpper().Trim();
            //bool chkDup = chkDuplicate();
            //if (chkDup == false)
            //{
            //    txeName.Text = "";
            //    txeName.Focus();
            //}
            //else
            //{
            //    txeDescription.Focus();
            //}
        }

        private void txeDescription_KeyDown(object sender, KeyEventArgs e)
        {
        //    if (e.KeyCode == Keys.Enter)
        //    {
        //        txeDueDate.Focus();
        //    }
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

            //            if (lblStatus.Text == "* Add Payment Term")
            //            {
            //                sbSQL.Append("  INSERT INTO PaymentTerm(Name, Description, DueDateCalculation, Status, CreatedBy, CreatedDate) ");
            //                sbSQL.Append("  VALUES(N'" + txeName.Text.Trim().Replace("'", "''") + "', N'" + txeDescription.Text.Trim().Replace("'", "''") + "', N'" + txeDueDate.Text.Trim().Replace("'", "''") + "', " + Status + ", '" + strCREATE + "', GETDATE()) ");
            //            }
            //            else if (lblStatus.Text == "* Edit Payment Term")
            //            {
            //                sbSQL.Append("  UPDATE PaymentTerm SET ");
            //                sbSQL.Append("      Name=N'" + txeName.Text.Trim().Replace("'", "''") + "', ");
            //                sbSQL.Append("      Description=N'" + txeDescription.Text.Trim().Replace("'", "''") + "', ");
            //                sbSQL.Append("      DueDateCalculation=N'" + txeDueDate.Text.Trim().Replace("'", "''") + "', ");
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
            //    }
            //}
        }

        private void bbiExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //string pathFile = new ObjSet.Folder(@"C:\MDS\Export\").GetPath() + "PaymentTermList_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
            //gvPTerm.ExportToXlsx(pathFile);
            //System.Diagnostics.Process.Start(pathFile);
        }

        private void gvPTerm_RowClick(object sender, RowClickEventArgs e)
        {
            //lblStatus.Text = "* Edit Payment Term";
            //lblStatus.ForeColor = Color.Red;

            //txeID.Text = gvPTerm.GetFocusedRowCellValue("No").ToString();
            //txeName.Text = gvPTerm.GetFocusedRowCellValue("Name").ToString();
            //txeDescription.Text = gvPTerm.GetFocusedRowCellValue("Description").ToString();
            //txeDueDate.Text = gvPTerm.GetFocusedRowCellValue("DuedateCalculation").ToString();

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

        private void tabbed_Master_SelectedPageChanged(object sender, DevExpress.XtraLayout.LayoutTabPageChangedEventArgs e)
        {
            if (tabbed_Master.SelectedTabPageIndex == 1) //Entry
            {
                q.get_sl_smplNo(sl_smplNo_Entry);
                q.get_gl_Branch(gl_branch_entry);
                q.get_gl_Season(gl_Season_Entry);
                q.get_sl_Customer(sl_Customer_Entry);
                q.get_gcListof_SMPL(gcListof_SMPL); gvListof_SMPL.OptionsBehavior.Editable = false;
                txtCreateBy.EditValue = 0;
                txtCreateDate.EditValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                txtUpdateBy.EditValue = 0;
                txtUpdateDate.EditValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Header
                txtBomNo.EditValue = q.get_running_BomNo(); txtBomNo.ReadOnly = true;
                dtLastDate.EditValue = DateTime.Now;
                q.get_sl_StyleNmae(sl_StyleName);
                q.get_gl_Category(gl_Category);
                q.get_gl_Season(gl_Season_Header);
                q.get_sl_Customer(sl_Customer_Header);
                q.get_sl_Color(sl_Color);
                q.get_sl_Size(sl_Size);
                q.get_gl_Unit(gl_Unit);
                rdoStatus.SelectedIndex = 0;
            }
        }
    }
}