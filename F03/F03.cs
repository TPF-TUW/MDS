using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using DBConnection;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Extensions;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using TheepClass;

namespace F03
{
    public partial class F03 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private Functionality.Function FUNC = new Functionality.Function();
        private string dbDP = "Departments";
        private string dbBranch = "Branchs";
        List<DepartmentType> departmentTypes;

        public F03()
        {
            InitializeComponent();
            UserLookAndFeel.Default.StyleChanged += MyStyleChanged;
            departmentTypes = new List<DepartmentType>();
            departmentTypes.Add(new DepartmentType { name = "Admin", value = 0 });
            departmentTypes.Add(new DepartmentType { name = "Packing", value = 1 });
            departmentTypes.Add(new DepartmentType { name = "NeedleRoom", value= 2 });
            departmentTypes.Add(new DepartmentType { name = "Warehouse", value= 3 });
            departmentTypes.Add(new DepartmentType { name = "StoreFabric", value = 4 });
            departmentTypes.Add(new DepartmentType { name = "StoreAccessory", value = 5 });
            departmentTypes.Add(new DepartmentType { name = "Delivery", value = 6 });
            departmentTypes.Add(new DepartmentType { name = "FQA", value = 7 });
            departmentTypes.Add(new DepartmentType { name = "CMT", value = 8 });
            departmentTypes.Add(new DepartmentType { name = "Sales", value = 9 });
        }

        private void MyStyleChanged(object sender, EventArgs e)
        {
            UserLookAndFeel userLookAndFeel = (UserLookAndFeel)sender;
            cUtility.SaveRegistry(@"Software\MDS", "SkinName", userLookAndFeel.SkinName);
            cUtility.SaveRegistry(@"Software\MDS", "SkinPalette", userLookAndFeel.ActiveSvgPaletteName);
        }

        private void XtraForm1_Load(object sender, EventArgs e)
        {
            glueDPType.Properties.DataSource = departmentTypes;
            glueDPType.Properties.DisplayMember = "name";
            glueDPType.Properties.ValueMember = "value";

            LoadData();
            NewData();
        }

        private void LoadDEPT()
        {
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("SELECT DP.OIDDEPT AS ID, DP.Code AS [Department Code], DP.Name AS [Department Name], DP.DepartmentType AS [Department Type ID], DT.Name AS [Department Type], DP.OIDCOMPANY AS [Company ID], CP.Code AS [Company Code], ");
            sbSQL.Append("       CP.EngName AS[Company Name(En)], CP.THName AS[Company Name(Th)], DP.OIDBRANCH AS [Branch ID], BN.Code AS[Branch Code], BN.Name AS[Branch Name], DP.Status AS [Status ID], CASE WHEN DP.Status = 0 THEN 'Non Active' ELSE CASE WHEN DP.Status = 1 THEN 'Active' ELSE '' END END AS Status, DP.CreatedBy, DP.CreatedDate ");
            sbSQL.Append("FROM   " + this.dbDP + " AS DP LEFT OUTER JOIN ");
            sbSQL.Append("       Company AS CP ON DP.OIDCOMPANY = CP.OIDCOMPANY LEFT OUTER JOIN ");
            sbSQL.Append("       " + this.dbBranch + " AS BN ON DP.OIDBRANCH = BN.OIDBranch LEFT OUTER JOIN ");
            sbSQL.Append("       DepartmentType AS DT ON DP.DepartmentType = DT.Code ");
            sbSQL.Append("WHERE (DP.Code <> N'') ");
            if (glueCompany.Text.Trim() != "")
            {
                sbSQL.Append("AND (DP.OIDCOMPANY = '" + glueCompany.EditValue.ToString() + "') ");
            }
            if (glueBranch.Text.Trim() != "")
            {
                sbSQL.Append("AND (DP.OIDBRANCH = '" + glueBranch.EditValue.ToString() + "') ");
            }
            if (glueDPType.Text.Trim() != "")
            {
                sbSQL.Append("AND (DP.DepartmentType = '" + glueDPType.EditValue.ToString() + "') ");
            }
            sbSQL.Append("ORDER BY[Company ID], [Branch ID], ID ");
            new ObjDevEx.setGridControl(gcDP, gvDP, sbSQL).getData(false, false, false, true);
            gvDP.Columns[3].Visible = false; //Department Type ID
            gvDP.Columns[5].Visible = false; //Company ID
            gvDP.Columns[9].Visible = false; //Branch ID
            gvDP.Columns[12].Visible = false; //Status ID

        }

        private void LoadData()
        {
            StringBuilder sbSQL = new StringBuilder();

            sbSQL.Clear();
            sbSQL.Append("SELECT Code AS [Company Code], EngName AS [Company Name (En)], THName AS [Company Name (Th)], OIDCOMPANY AS ID ");
            sbSQL.Append("FROM Company ");
            sbSQL.Append("ORDER BY ID ");
            new ObjDevEx.setGridLookUpEdit(glueCompany, sbSQL, "Company Code", "ID").getData();

            sbSQL.Clear();
            sbSQL.Append("SELECT Name AS [Department Type], Code AS ID ");
            sbSQL.Append("FROM DepartmentType ");
            sbSQL.Append("ORDER BY ID ");
            new ObjDevEx.setGridLookUpEdit(glueDPType, sbSQL, "Department Type", "ID").getData();

            LoadDEPT();
        }

        private void NewData()
        {
            txeName.Text = "";
            lblStatus.Text = "* Add Department";
            lblStatus.ForeColor = Color.Green;

            txeID.Text = new DBQuery("SELECT CASE WHEN ISNULL(MAX(OIDDEPT), '') = '' THEN 1 ELSE MAX(OIDDEPT) + 1 END AS NewNo FROM " + this.dbDP).getString();
            txeCode.Text = "";
            txeName.Text = "";
            rgStatus.EditValue = 1;

            glueDPType.EditValue = "";
            glueCompany.EditValue = "";
            glueBranch.Properties.DataSource = null;

            txeCREATE.Text = "0";
            txeCDATE.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            glueCompany.Focus();
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
            string Company = "";
            if (glueCompany.Text.Trim() != "")
            {
                Company = glueCompany.EditValue.ToString();
            }

            string Branch = "";
            if (glueBranch.Text.Trim() != "")
            {
                Branch = glueBranch.EditValue.ToString();
            }

            string DPType = "";
            if (glueDPType.Text.Trim() != "")
            {
                DPType = glueDPType.EditValue.ToString();
            }

            if (lblStatus.Text == "* Add Department")
            {
                if (txeCode.Text.Trim() != "" || txeName.Text.Trim() != "")
                {
                    StringBuilder sbSQL = new StringBuilder();
                    if (txeCode.Text.Trim() != "" && chkDup == true)
                    {
                        sbSQL.Clear();
                        sbSQL.Append("SELECT TOP(1) Code FROM " + this.dbDP + " WHERE (OIDCOMPANY = '" + Company + "') AND (OIDBRANCH = '" + Branch + "') AND (DepartmentType = '" + DPType + "') AND (Code = N'" + txeCode.Text.Trim() + "') ");
                        string chkNo = new DBQuery(sbSQL).getString();
                        if (chkNo != "")
                        {
                            txeCode.Text = "";
                            txeCode.Focus();
                            chkDup = false;
                            FUNC.msgWarning("Duplicate department code. !! Please Change.");
                        }
                    }

                    if (txeName.Text.Trim() != "" && chkDup == true)
                    {
                        sbSQL.Clear();
                        sbSQL.Append("SELECT TOP(1) Code FROM " + this.dbDP + " WHERE (OIDCOMPANY = '" + Company + "') AND (OIDBRANCH = '" + Branch + "') AND (DepartmentType = '" + DPType + "') AND (Name = N'" + txeName.Text.Trim() + "') ");
                        string chkNo = new DBQuery(sbSQL).getString();
                        if (chkNo != "")
                        {
                            txeName.Text = "";
                            txeName.Focus();
                            chkDup = false;
                            FUNC.msgWarning("Duplicate department name. !! Please Change.");
                        }
                    }

                }
            }
            else if (lblStatus.Text == "* Edit Department")
            {
                if (txeCode.Text.Trim() != "" || txeName.Text.Trim() != "")
                {
                    StringBuilder sbSQL = new StringBuilder();
                    if (txeCode.Text.Trim() != "" && chkDup == true)
                    {
                        sbSQL.Clear();
                        sbSQL.Append("SELECT TOP(1) OIDDEPT FROM " + this.dbDP + " WHERE (OIDCOMPANY = '" + Company + "') AND (OIDBRANCH = '" + Branch + "') AND (DepartmentType = '" + DPType + "') AND (Code = N'" + txeCode.Text.Trim() + "') ");
                        string chkNo = new DBQuery(sbSQL).getString();
                        if (chkNo != "" && chkNo != txeID.Text.Trim())
                        {
                            txeCode.Text = "";
                            txeCode.Focus();
                            chkDup = false;
                            FUNC.msgWarning("Duplicate department code. !! Please Change.");
                        }
                    }

                    if (txeName.Text.Trim() != "" && chkDup == true)
                    {
                        sbSQL.Clear();
                        sbSQL.Append("SELECT TOP(1) OIDDEPT FROM " + this.dbDP + " WHERE (OIDCOMPANY = '" + Company + "') AND (OIDBRANCH = '" + Branch + "') AND (DepartmentType = '" + DPType + "') AND (Name = N'" + txeName.Text.Trim() + "') ");
                        string chkNo = new DBQuery(sbSQL).getString();
                        if (chkNo != "" && chkNo != txeID.Text.Trim())
                        {
                            txeName.Text = "";
                            txeName.Focus();
                            chkDup = false;
                            FUNC.msgWarning("Duplicate department name. !! Please Change.");
                        }
                    }

                }
            }

            return chkDup;
        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (glueCompany.Text.Trim() == "")
            {
                FUNC.msgWarning("Please select company.");
                glueCompany.Focus();
            }
            else if (glueBranch.Text.Trim() == "")
            {
                FUNC.msgWarning("Please select branch.");
                glueBranch.Focus();
            }
            else if (glueDPType.Text.Trim() == "")
            {
                FUNC.msgWarning("Please select department type.");
                glueDPType.Focus();
            }
            else if (txeCode.Text.Trim() == "")
            {
                FUNC.msgWarning("Please input code.");
                txeCode.Focus();
            }
            else if (txeName.Text.Trim() == "")
            {
                FUNC.msgWarning("Please input name.");
                txeName.Focus();
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

                    bool chkGMP = chkDuplicate();
                    if (chkGMP == true)
                    {
                        string Status = "NULL";
                        if (rgStatus.SelectedIndex != -1)
                        {
                            Status = rgStatus.Properties.Items[rgStatus.SelectedIndex].Value.ToString();
                        }

                        if (lblStatus.Text == "* Add Department")
                        {
                            sbSQL.Append("  INSERT INTO " + this.dbDP + "(Code, Name, DepartmentType, OIDCOMPANY, OIDBRANCH, Status, CreatedBy, CreatedDate) ");
                            sbSQL.Append("  VALUES(N'" + txeCode.Text.Trim().Replace("'", "''") + "', N'" + txeName.Text.Trim().Replace("'", "''") + "', '" + glueDPType.EditValue.ToString() + "', '" + glueCompany.EditValue.ToString() + "', '" + glueBranch.EditValue.ToString() + "', " + Status + ", '" + strCREATE + "', GETDATE()) ");
                        }
                        else if (lblStatus.Text == "* Edit Department")
                        {
                            sbSQL.Append("  UPDATE " + this.dbDP + " SET ");
                            sbSQL.Append("      Code=N'" + txeCode.Text.Trim().Replace("'", "''") + "', ");
                            sbSQL.Append("      Name=N'" + txeName.Text.Trim().Replace("'", "''") + "', ");
                            sbSQL.Append("      DepartmentType='" + glueDPType.EditValue.ToString() + "', ");
                            sbSQL.Append("      OIDCOMPANY='" + glueCompany.EditValue.ToString() + "', ");
                            sbSQL.Append("      OIDBRANCH='" + glueBranch.EditValue.ToString() + "', ");
                            sbSQL.Append("      Status=" + Status + " ");
                            sbSQL.Append("  WHERE (OIDDEPT = '" + txeID.Text.Trim() + "') ");
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
            }
        }

        private void bbiExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string pathFile = new ObjSet.Folder(@"C:\MDS\Export\").GetPath() + "DepartmentList_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
            gvDP.ExportToXlsx(pathFile);
            System.Diagnostics.Process.Start(pathFile);
        }

        private void gvPTerm_RowClick(object sender, RowClickEventArgs e)
        {

        }

        private void bbiPrintPreview_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gcDP.ShowPrintPreview();
        }

        private void bbiPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gcDP.Print();
        }

        private void F03_Shown(object sender, EventArgs e)
        {
            glueCompany.Focus();
        }


        private void gvDP_DoubleClick(object sender, EventArgs e)
        {
            GridView view = (GridView)sender;
            Point pt = view.GridControl.PointToClient(Control.MousePosition);
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo info = view.CalcHitInfo(pt);
            if (info.InRow || info.InRowCell)
            {
                DataTable dtCP = (DataTable)gcDP.DataSource;
                if (dtCP.Rows.Count > 0)
                {
                    lblStatus.Text = "* Edit Department";
                    lblStatus.ForeColor = Color.Red;

                    DataRow drCP = dtCP.Rows[info.RowHandle];
                    txeID.Text = drCP["ID"].ToString();
                    glueCompany.EditValue = drCP["Company ID"].ToString();
                    glueBranch.EditValue = drCP["Branch ID"].ToString();
                    glueDPType.EditValue = drCP["Department Type ID"].ToString();
                    txeCode.Text = drCP["Department Code"].ToString();
                    txeName.Text = drCP["Department Name"].ToString();

                    rgStatus.EditValue = Convert.ToInt32(drCP["Status ID"].ToString());

                    txeCREATE.Text = drCP["CreatedBy"].ToString();
                    txeCDATE.Text = drCP["CreatedDate"].ToString();
                }
            }
        }

        private void gvDP_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            if (gvDP.IsFilterRow(e.RowHandle)) return;
        }

        private void glueCompany_EditValueChanged(object sender, EventArgs e)
        {
            glueBranch.Properties.DataSource = null;

            if (glueCompany.Text.Trim() != "")
            {
                StringBuilder sbSQL = new StringBuilder();
                sbSQL.Append("SELECT Code AS [Branch Code], Name AS [Branch Name], CASE WHEN BranchType = 0 THEN 'Branch' ELSE CASE WHEN BranchType = 1 THEN 'Branch Sub Contract' ELSE '' END END AS [Branch Type], OIDBranch AS ID ");
                sbSQL.Append("FROM  " + this.dbBranch + " ");
                sbSQL.Append("WHERE (OIDCOMPANY = '" + glueCompany.EditValue.ToString() + "') ");
                sbSQL.Append("ORDER BY [Branch Code] ");

                new ObjDevEx.setGridLookUpEdit(glueBranch, sbSQL, "Branch Code", "ID").getData();

                bool chkDup = chkDuplicate();
                if (chkDup == true)
                {
                    glueBranch.Focus();
                }
                
            }

            LoadDEPT();
        }

        private void glueBranch_EditValueChanged(object sender, EventArgs e)
        {
            if (glueBranch.Text.Trim() != "")
            {
                bool chkDup = chkDuplicate();
                if (chkDup == true)
                {
                    glueDPType.Focus();
                }
            }
            LoadDEPT();
        }


        private void glueDPType_EditValueChanged(object sender, EventArgs e)
        {
            if (glueDPType.Text.Trim() != "")
            {
                bool chkDup = chkDuplicate();
                if (chkDup == true)
                {
                    txeCode.Focus();
                }
            }
            LoadDEPT();
        }


        private void txeCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeName.Focus();
            }
        }

        private void txeCode_Leave(object sender, EventArgs e)
        {
            if (txeCode.Text.Trim() != "")
            {
                txeCode.Text = txeCode.Text.Trim();
                bool chkDup = chkDuplicate();
                if (chkDup == true)
                {
                    txeName.Focus();
                }
            }
        }

        private void txeName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                rgStatus.Focus();
            }
        }

        private void txeName_Leave(object sender, EventArgs e)
        {
            if (txeName.Text.Trim() != "")
            {
                txeName.Text = txeName.Text.Trim();
                bool chkDup = chkDuplicate();
                if (chkDup == true)
                {
                    rgStatus.Focus();
                }
            }
        }
    }

    public class DepartmentType
    {
        public string name { get;set; }
        public int value { get; set; }
    }
}