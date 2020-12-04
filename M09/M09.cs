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
using System.Data;
using DevExpress.Data.Extensions;
using System.Linq;

namespace M08
{
    public partial class M09 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private Functionality.Function FUNC = new Functionality.Function();
        public M09()
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
            sbSQL.Append("SELECT PC.OIDPCAP AS CapacityID, PC.OIDCUST AS CustomerID, CUS.ShortName AS CustomerName, PC.OIDGCATEGORY AS CategoryID, GC.CategoryName, PC.OIDSTYLE AS StyleID, PS.StyleName, PC.QTYPerHour, PC.QTYPerDay, ");
            sbSQL.Append("       PC.QTYPerOT, FORMAT(PC.STDTimeCUT, '###0.####') AS StandardTimeCutting, FORMAT(PC.STDTimePAD, '###0.####') AS StandardTimePadPrint, FORMAT(PC.STDTimeSEW, '###0.####') AS StandardTimeSewing, FORMAT(PC.STDTimePACK, '###0.####') AS StandardTimePacking, FORMAT(PC.STDTime, '###0.####') AS StandardTime, ");
            sbSQL.Append("       PC.ProductionStartDate, PC.CreatedBy, PC.CreatedDate ");
            sbSQL.Append("FROM   ProductionCapacity AS PC INNER JOIN ");
            sbSQL.Append("       Customer AS CUS ON PC.OIDCUST = CUS.OIDCUST INNER JOIN ");
            sbSQL.Append("       ProductStyle AS PS ON PC.OIDSTYLE = PS.OIDSTYLE INNER JOIN ");
            sbSQL.Append("       GarmentCategory AS GC ON PC.OIDGCATEGORY = GC.OIDGCATEGORY ");
            sbSQL.Append("ORDER BY CapacityID ");
            new ObjDevEx.setGridControl(gcCapacity, gvCapacity, sbSQL).getData(false, false, false, true);
            //MessageBox.Show("1");

       
            //*******************************

            sbSQL.Clear();
            sbSQL.Append("SELECT DISTINCT CUS.Code, CUS.ShortName, CUS.Name, CUS.OIDCUST AS ID ");
            sbSQL.Append("FROM   Customer AS CUS INNER JOIN ");
            sbSQL.Append("       ProductionLine AS PL ON CUS.OIDCUST = PL.OIDCUST ");
            sbSQL.Append("ORDER BY CUS.ShortName ");
            new ObjDevEx.setSearchLookUpEdit(slueCustomer, sbSQL, "ShortName", "ID").getData(true);
            //MessageBox.Show("3");
        }

        private void CreateTabPage(string CusID="", string CategoryID="")
        {
            //gcCapacity.DataSource = null;
            tabBranch.TabPages.Clear();

            if (CusID != "" && CategoryID != "")
            {
                StringBuilder sbSQL = new StringBuilder();
                sbSQL.Append("SELECT B.Branch, B.OIDBranch AS ID ");
                sbSQL.Append("FROM ProductionLine AS PL INNER JOIN ");
                sbSQL.Append("     Branch AS B ON PL.Branch = B.OIDBranch ");
                sbSQL.Append("WHERE (PL.OIDCUST = '" + CusID + "') AND(PL.OIDCATEGORY = '" + CategoryID + "') ");
                sbSQL.Append("ORDER BY B.OIDBranch ");
                DataTable dtBranch = new DBQuery(sbSQL).getDataTable();
                foreach (DataRow drRow in dtBranch.Rows)
                {
                    DevExpress.XtraTab.XtraTabPage tabPage = new DevExpress.XtraTab.XtraTabPage();
                    tabPage.Name = "B" + drRow["ID"].ToString();
                    tabPage.Text = drRow["Branch"].ToString();

                    DevExpress.XtraGrid.GridControl grid = new DevExpress.XtraGrid.GridControl();
                    grid.Name = "gc" + drRow["ID"].ToString();
                    GridView view = new GridView();
                    view.Name = "gv" + drRow["ID"].ToString();

                    tabPage.Controls.Add(grid);

                    grid.Dock = DockStyle.Fill;
                    grid.ViewCollection.Add(view);
                    grid.MainView = view;
                    view.GridControl = grid;
                    view.OptionsView.ShowAutoFilterRow = false;
                    view.OptionsBehavior.Editable = false;
                    view.OptionsView.EnableAppearanceEvenRow = true;
                    view.OptionsView.EnableAppearanceOddRow = true;

                    StringBuilder sbLINE = new StringBuilder();
                    sbLINE.Append("SELECT LN.LINENAME AS LineName, LN.OIDLINE AS LineID, LN.Branch AS BranchID  ");
                    sbLINE.Append("FROM   ProductionLine AS PL INNER JOIN ");
                    sbLINE.Append("        LineNumber AS LN ON PL.OIDLine = LN.OIDLINE ");
                    sbLINE.Append("WHERE (PL.OIDCUST = '" + CusID + "') AND(PL.OIDCATEGORY = '" + CategoryID + "') AND(PL.Branch = '" + drRow["ID"].ToString() + "') ");
                    sbLINE.Append("ORDER By LN.LINENAME ");
                    DataTable dtLINE = new DBQuery(sbLINE).getDataTable();

                    grid.DataSource = dtLINE;

                    grid.EndUpdate();
                    grid.ResumeLayout();
                    view.OptionsView.ColumnAutoWidth = true;
                    view.BestFitColumns();
                    view.RowCellClick += gvLine_RowCellClick;
                    view.DataSourceChanged += gvLine_DataSourceChanged;
                    view.RefreshData();
                    grid.RefreshDataSource();

                    tabBranch.TabPages.Add(tabPage);
                    tabBranch.ResumeLayout(false);
                    tabBranch.LayoutChanged();
                }

            }
        }

        private void CreateTabPageLine(string CusID = "", string CategoryID = "", string StyleID = "")
        {
            //gcCapacity.DataSource = null;
            tabBranch.TabPages.Clear();

            if (CusID != "" && CategoryID != "")
            {
                StringBuilder sbSQL = new StringBuilder();
                sbSQL.Append("SELECT DISTINCT B.Branch, B.OIDBranch AS ID ");
                sbSQL.Append("FROM ProductionLine AS PL INNER JOIN ");
                sbSQL.Append("     Branch AS B ON PL.Branch = B.OIDBranch ");
                sbSQL.Append("WHERE (PL.OIDCUST = '" + CusID + "') AND(PL.OIDCATEGORY = '" + CategoryID + "') ");
                sbSQL.Append("ORDER BY B.OIDBranch ");
                DataTable dtBranch = new DBQuery(sbSQL).getDataTable();
                int BCount = new DBQuery(sbSQL).getCount();

                foreach (DataRow drRow in dtBranch.Rows)
                {
                    DevExpress.XtraTab.XtraTabPage tabPage = new DevExpress.XtraTab.XtraTabPage();
                    tabPage.Name = "B" + drRow["ID"].ToString();
                    tabPage.Text = drRow["Branch"].ToString();

                    CheckedListBoxControl clbLine = new CheckedListBoxControl();
                    clbLine.Name = "LN" + drRow["ID"].ToString();


                    tabPage.Controls.Add(clbLine);
                    clbLine.Dock = DockStyle.Fill;

                    StringBuilder sbLINE = new StringBuilder();
                    sbLINE.Append("SELECT LN.LINENAME AS LineName, LN.OIDLINE AS LineID, LN.Branch AS BranchID  ");
                    sbLINE.Append("FROM   ProductionLine AS PL INNER JOIN ");
                    sbLINE.Append("        LineNumber AS LN ON PL.OIDLine = LN.OIDLINE ");
                    sbLINE.Append("WHERE (PL.OIDCUST = '" + CusID + "') AND(PL.OIDCATEGORY = '" + CategoryID + "') AND(PL.Branch = '" + drRow["ID"].ToString() + "') ");
                    sbLINE.Append("ORDER By LN.LINENAME ");
                    DataTable dtLINE = new DBQuery(sbLINE).getDataTable();

                    clbLine.ValueMember = "LineName";
                    clbLine.DisplayMember = "LineName";
                    clbLine.DataSource = dtLINE;

                    StringBuilder sbCapacity = new StringBuilder();
                    sbCapacity.Append("SELECT DISTINCT LINEID AS LineName ");
                    sbCapacity.Append("FROM   ProductionCapacityLine ");
                    sbCapacity.Append("WHERE(OIDCAP IN ");
                    sbCapacity.Append("             (SELECT OIDPCAP ");
                    sbCapacity.Append("              FROM   ProductionCapacity ");
                    sbCapacity.Append("              WHERE (OIDCUST = '" + CusID + "') AND (OIDGCATEGORY = '" + CategoryID + "'))) AND (OIDBranch = '" + drRow["ID"].ToString() + "') ");
                    DataTable dtQC = new DBQuery(sbCapacity).getDataTable();
                    foreach (DataRow row in dtQC.Rows)
                    {
                        for (int i = 0; i < clbLine.ItemCount; i++)
                        {
                            if (row["LineName"].ToString() == clbLine.GetItemValue(i).ToString())
                            {
                                clbLine.SetItemCheckState(i, CheckState.Checked);
                                break;
                            }
                        }
                    }

                    tabBranch.TabPages.Add(tabPage);
                    tabBranch.ResumeLayout(false);
                    tabBranch.LayoutChanged();
                }

                sbSQL.Clear();
                sbSQL.Append("SELECT PC.OIDPCAP AS CapacityID, PC.OIDCUST AS CustomerID, CUS.ShortName AS CustomerName, PC.OIDGCATEGORY AS CategoryID, GC.CategoryName, PC.OIDSTYLE AS StyleID, PS.StyleName, PC.QTYPerHour, PC.QTYPerDay, ");
                sbSQL.Append("       PC.QTYPerOT, FORMAT(PC.STDTimeCUT, '###0.####') AS StandardTimeCutting, FORMAT(PC.STDTimePAD, '###0.####') AS StandardTimePadPrint, FORMAT(PC.STDTimeSEW, '###0.####') AS StandardTimeSewing, FORMAT(PC.STDTimePACK, '###0.####') AS StandardTimePacking, FORMAT(PC.STDTime, '###0.####') AS StandardTime, ");
                sbSQL.Append("       PC.ProductionStartDate, PC.CreatedBy, PC.CreatedDate ");
                sbSQL.Append("FROM   ProductionCapacity AS PC INNER JOIN ");
                sbSQL.Append("       Customer AS CUS ON PC.OIDCUST = CUS.OIDCUST INNER JOIN ");
                sbSQL.Append("       ProductStyle AS PS ON PC.OIDSTYLE = PS.OIDSTYLE INNER JOIN ");
                sbSQL.Append("       GarmentCategory AS GC ON PC.OIDGCATEGORY = GC.OIDGCATEGORY ");
                sbSQL.Append("WHERE  (PC.OIDCUST = '" + CusID + "') ");
                if (CusID != "")
                {
                    sbSQL.Append("AND (PC.OIDGCATEGORY = '" + CategoryID + "') ");
                }
                if (StyleID != "")
                {
                    sbSQL.Append("AND (PC.OIDSTYLE = '" + StyleID + "') ");
                }
                sbSQL.Append("ORDER BY CapacityID ");
                new ObjDevEx.setGridControl(gcCapacity, gvCapacity, sbSQL).getData(false, false, false, true);

            }
        }

        private void gvLine_DataSourceChanged(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            view.Columns["LineID"].Visible = false;
            view.Columns["BranchID"].Visible = false;
        }

        private void gvLine_RowLoaded(object sender, DevExpress.XtraGrid.Views.Base.RowEventArgs e)
        {
            GetVisible((GridView)sender, e.RowHandle);
        }

        private void GetVisible(GridView aView, int aRowHandle)
        {
            aView.Columns[1].Visible = false;
        }

        private void gvLine_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            GetValue((GridView)sender, e.RowHandle);      
        }

        private void GetValue(GridView aView, int aRowHandle)
        {
            string LineName = aView.GetRowCellValue(aRowHandle, aView.Columns[0]).ToString();
            string LineID = aView.GetRowCellValue(aRowHandle, aView.Columns[1]).ToString();
            string BranchID = aView.GetRowCellValue(aRowHandle, aView.Columns[2]).ToString();
            MessageBox.Show("LineName:"+ LineName+", LineID:" + LineID+", BranchID:" + BranchID);
        }

        private void NewData()
        {
            lblStatus.Text = "* Add Capacity";
            lblStatus.ForeColor = Color.Green;

            txeID.Text = new DBQuery("SELECT CASE WHEN ISNULL(MAX(OIDPCAP), '') = '' THEN 1 ELSE MAX(OIDPCAP) + 1 END AS NewNo FROM ProductionCapacity").getString();
            //txeID.Text = "";

            //gcCapacity.DataSource = null;
            tabBranch.TabPages.Clear();

            ClearData();
        }

        private void ClearData()
        {
            txe1Hr.Text = "";
            txe1Day.Text = "";
            txeOT.Text = "";

            txeCutting.Text = "";
            txePadPrint.Text = "";
            txeSewing.Text = "";
            txePacking.Text = "";
            txeStdTime.Text = "";

            dteStart.EditValue = DateTime.Now;
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

        private void slueCustomer_EditValueChanged(object sender, EventArgs e)
        {
            glueCategory.EditValue = "";
            glueCategory.Properties.DataSource = null;
            ClearData();

            if (slueCustomer.Text.Trim() != "")
            {
                string CUSID = slueCustomer.EditValue.ToString();
                StringBuilder sbSQL = new StringBuilder();
                sbSQL.Append("SELECT DISTINCT GC.OIDGCATEGORY AS ID, GC.CategoryName ");
                sbSQL.Append("FROM GarmentCategory AS GC INNER JOIN ");
                sbSQL.Append("     ProductionLine AS PL ON GC.OIDGCATEGORY = PL.OIDCATEGORY ");
                sbSQL.Append("WHERE (PL.OIDCUST = '" + CUSID + "') ");
                sbSQL.Append("ORDER BY ID ");
                new ObjDevEx.setGridLookUpEdit(glueCategory, sbSQL, "CategoryName", "ID").getData(true);

                CreateTabPageLine(CUSID);
            }
            glueCategory.Focus();
        }

        private void glueCategory_EditValueChanged(object sender, EventArgs e)
        {
            slueStyle.EditValue = "";
            slueStyle.Properties.DataSource = null;
            ClearData();

            if (glueCategory.Text.Trim() != "")
            {
                string CATEID = glueCategory.EditValue.ToString();
                StringBuilder sbSQL = new StringBuilder();
                sbSQL.Append("SELECT StyleName, OIDSTYLE AS ID ");
                sbSQL.Append("FROM   ProductStyle ");
                sbSQL.Append("WHERE (OIDGCATEGORY = '" + CATEID + "') ");
                new ObjDevEx.setSearchLookUpEdit(slueStyle, sbSQL, "StyleName", "ID").getData(true);
            }

            //***** LOAD BRANCH *************
            tabBranch.TabPages.Clear();
            string CUSID = "";
            string CATGID = "";

            if (slueCustomer.Text.Trim() != "")
            {
                CUSID = slueCustomer.EditValue.ToString();
            }
            if (glueCategory.Text.Trim() != "")
            {
                CATGID = glueCategory.EditValue.ToString();
            }
            CreateTabPageLine(CUSID, CATGID);
            //*******************************

            slueStyle.Focus();

        }

        private void gvCapacity_RowStyle(object sender, RowStyleEventArgs e)
        {
            
        }

        private void slueStyle_EditValueChanged(object sender, EventArgs e)
        {
            string CUSID = "";
            string CATGID = "";
            string STYLEID = "";

            if (slueCustomer.Text.Trim() != "")
            {
                CUSID = slueCustomer.EditValue.ToString();
            }
            if (glueCategory.Text.Trim() != "")
            {
                CATGID = glueCategory.EditValue.ToString();
            }
            if (slueStyle.Text.Trim() != "")
            {
                STYLEID = slueStyle.EditValue.ToString();
            }
            CreateTabPageLine(CUSID, CATGID, STYLEID);

            //******* LOAD DATA ***********
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("SELECT TOP(1) OIDPCAP, QTYPerHour, QTYPerDay, ");
            sbSQL.Append("       QTYPerOT, FORMAT(STDTimeCUT, '###0.####') AS StandardTimeCutting, FORMAT(STDTimePAD, '###0.####') AS StandardTimePadPrint, FORMAT(STDTimeSEW, '###0.####') AS StandardTimeSewing, FORMAT(STDTimePACK, '###0.####') AS StandardTimePacking, FORMAT(STDTime, '###0.####') AS StandardTime, ");
            sbSQL.Append("       ProductionStartDate, CreatedBy, CreatedDate ");
            sbSQL.Append("FROM   ProductionCapacity ");
            sbSQL.Append("WHERE  (OIDCUST = '" + CUSID + "') AND (OIDGCATEGORY = '" + CATGID + "') AND (OIDSTYLE = '" + STYLEID + "') ");
            string[] arrCapacity = new DBQuery(sbSQL).getMultipleValue();
            if (arrCapacity.Length > 0)
            {
                lblStatus.Text = "* Edit Capacity";
                lblStatus.ForeColor = Color.Red;

                txeID.Text = arrCapacity[0];
                txe1Hr.Text = arrCapacity[1];
                txe1Day.Text = arrCapacity[2];
                txeOT.Text = arrCapacity[3];

                txeCutting.Text = arrCapacity[4];
                txePadPrint.Text = arrCapacity[5];
                txeSewing.Text = arrCapacity[6];
                txePacking.Text = arrCapacity[7];
                txeStdTime.Text = arrCapacity[8];

                dteStart.EditValue = Convert.ToDateTime(arrCapacity[9]);

                txeCREATE.Text = arrCapacity[10];
                txeDATE.Text = arrCapacity[11];
            }
            else
            {
                lblStatus.Text = "* Add Capacity";
                lblStatus.ForeColor = Color.Green;

                txeID.Text = "";
                txe1Hr.Text = "";
                txe1Day.Text = "";
                txeOT.Text = "";

                txeCutting.Text = "";
                txePadPrint.Text = "";
                txeSewing.Text = "";
                txePacking.Text = "";
                txeStdTime.Text = "";

                dteStart.EditValue = DateTime.Now;

                txeCREATE.Text = "0";
                txeDATE.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            }

            txe1Hr.Focus();
        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (slueCustomer.Text.Trim() == "")
            {
                FUNC.msgWarning("Please select customer.");
                slueCustomer.Focus();
            }
            else if (glueCategory.Text.Trim() == "")
            {
                FUNC.msgWarning("Please product category.");
                glueCategory.Focus();
            }
            else if (slueStyle.Text.Trim() == "")
            {
                FUNC.msgWarning("Please select product style.");
                slueStyle.Focus();
            }
            else
            {
                if (FUNC.msgQuiz("Confirm save data ?") == true)
                {
                    StringBuilder sbSAVE = new StringBuilder();
                    string strCREATE = "0";
                    if (txeCREATE.Text.Trim() != "")
                    {
                        strCREATE = txeCREATE.Text.Trim();
                    }

                    //******** save ProductionCapacity table ************
                    string CUSTOMER = slueCustomer.EditValue.ToString();
                    string CATEGORY = glueCategory.EditValue.ToString();
                    string STYLEID = slueStyle.EditValue.ToString();

                    if (lblStatus.Text == "* Add Capacity")
                    {
                        sbSAVE.Append(" INSERT INTO ProductionCapacity(OIDCUST, OIDGCATEGORY, OIDSTYLE, QTYPerHour, QTYPerDay, QTYPerOT, STDTimeCUT, STDTimePAD, STDTimeSEW, STDTimePACK, STDTime, ProductionStartDate, CreatedBy, CreatedDate) ");
                        sbSAVE.Append("  VALUES('" + CUSTOMER + "', '" + CATEGORY + "', '" + STYLEID + "', '" + txe1Hr.Text.Trim() + "', '" + txe1Day.Text.Trim() + "', '" + txeOT.Text.Trim() + "', '" + txeCutting.Text.Trim() + "', '" + txePadPrint.Text.Trim() + "', '" + txeSewing.Text.Trim() + "', '" + txePacking.Text.Trim() + "', '" + txeStdTime.Text.Trim() + "', '" + Convert.ToDateTime(dteStart.Text).ToString("yyyy-MM-dd") + "', '" + strCREATE + "', GETDATE()) ");
                    }
                    else if (lblStatus.Text == "* Edit Capacity")
                    {
                        sbSAVE.Append(" UPDATE ProductionCapacity SET ");
                        sbSAVE.Append("  OIDCUST='" + CUSTOMER + "', OIDGCATEGORY='" + CATEGORY + "', OIDSTYLE='" + STYLEID + "', QTYPerHour='" + txe1Hr.Text.Trim() + "', QTYPerDay='" + txe1Day.Text.Trim() + "', ");
                        sbSAVE.Append("  QTYPerOT='" + txeOT.Text.Trim() + "', STDTimeCUT='" + txeCutting.Text.Trim() + "', STDTimePAD='" + txePadPrint.Text.Trim() + "', STDTimeSEW='" + txeSewing.Text.Trim() + "', STDTimePACK='" + txePacking.Text.Trim() + "', ");
                        sbSAVE.Append("  STDTime='" + txeStdTime.Text.Trim() + "', ProductionStartDate='" + Convert.ToDateTime(dteStart.Text).ToString("yyyy-MM-dd") + "' ");
                        sbSAVE.Append(" WHERE (OIDPCAP = '" + txeID.Text.Trim() + "') ");
                    }

                    if (sbSAVE.Length > 0)
                    {
                        try
                        {
                            bool chkSAVE = new DBQuery(sbSAVE).runSQL();
                            if (chkSAVE == true)
                            {
                                string strCAP = new DBQuery("SELECT TOP (1) OIDPCAP FROM ProductionCapacity WHERE (OIDCUST = '" + CUSTOMER + "') AND(OIDGCATEGORY = '" + CATEGORY + "') AND(OIDSTYLE = '" + STYLEID + "')").getString();

                                if (strCAP != "") //Save ProductionCapacityLine
                                {
                                    sbSAVE.Clear();
                                    StringBuilder sbSQL = new StringBuilder();
                                    sbSQL.Append("SELECT DISTINCT Branch ");
                                    sbSQL.Append("FROM   ProductionLine ");
                                    sbSQL.Append("ORDER BY Branch ");
                                    DataTable dtLINE = new DBQuery(sbSQL).getDataTable();
                                    foreach (DataRow row in dtLINE.Rows)
                                    {
                                        CheckedListBoxControl clb = this.Controls.Find("LN" + row["Branch"], true).FirstOrDefault() as CheckedListBoxControl;
                                        if (clb != null)
                                        {
                                            string strBRANCH = row["Branch"].ToString();
                                            string strLINE = "";
                                            int iCQC = 0;
                                            foreach (DataRowView item in clb.CheckedItems)
                                            {
                                                if (iCQC != 0)
                                                {
                                                    strLINE += ", ";
                                                }
                                                strLINE += "'" + item["LineName"].ToString() + "'";
                                                sbSAVE.Append("IF NOT EXISTS(SELECT OIDLCAPLine FROM ProductionCapacityLine WHERE (OIDCAP = '" + strCAP + "') AND (OIDBranch = '" + strBRANCH + "') AND (LINEID = '" + item["LineName"].ToString() + "')) ");
                                                sbSAVE.Append(" BEGIN ");
                                                sbSAVE.Append("  INSERT INTO ProductionCapacityLine(OIDCAP, OIDBranch, LINEID, CreatedBy, CreatedDate) ");
                                                sbSAVE.Append("  VALUES('" + strCAP + "', '" + strBRANCH + "', '" + item["LineName"].ToString() + "', '" + strCREATE + "', GETDATE()) ");
                                                sbSAVE.Append(" END ");
                                                iCQC++;
                                            }

                                            if (strLINE == "")
                                            {
                                                sbSAVE.Append("DELETE FROM ProductionCapacityLine WHERE (OIDCAP = '" + strCAP + "') AND (OIDBranch = '" + strBRANCH + "')  ");
                                            }
                                            else
                                            {
                                                sbSAVE.Append("DELETE FROM ProductionCapacityLine WHERE (OIDCAP = '" + strCAP + "') AND (OIDBranch = '" + strBRANCH + "') AND (LINEID NOT IN (" + strLINE + "))  ");
                                            }
                                        }
                                    }


                                    if (sbSAVE.Length > 0)
                                    {
                                        //MessageBox.Show(sbSAVE.ToString());
                                        try
                                        {
                                            bool chkSAVECAPA = new DBQuery(sbSAVE).runSQL();
                                            if (chkSAVECAPA == true)
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
                        catch (Exception)
                        { }
                    }
                }
            }
        }

        private void bbiExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string pathFile = new ObjSet.Folder(@"C:\MDS\Export\").GetPath() + "ProductionCapacityList_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
            gvCapacity.ExportToXlsx(pathFile);
            System.Diagnostics.Process.Start(pathFile);
        }

        private void txe1Hr_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txe1Day.Focus();
            }
        }

        private void txe1Day_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeOT.Focus();
            }
        }

        private void txeOT_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeCutting.Focus();
            }
        }

        private void txeCutting_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txePadPrint.Focus();
            }
        }

        private void txePadPrint_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeSewing.Focus();
            }
        }

        private void txeSewing_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txePacking.Focus();
            }
        }

        private void txePacking_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeStdTime.Focus();
            }
        }

        private void txeStdTime_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                dteStart.Focus();
            }
        }

        private void gvCapacity_RowClick(object sender, RowClickEventArgs e)
        {
            if (gvCapacity.IsFilterRow(e.RowHandle)) return;

            string CUSID = gvCapacity.GetFocusedRowCellValue("CustomerID").ToString();
            string CATEID = gvCapacity.GetFocusedRowCellValue("CategoryID").ToString();
            string STYLEID = gvCapacity.GetFocusedRowCellValue("StyleID").ToString();

            lblStatus.Text = "* Edit Capacity";
            lblStatus.ForeColor = Color.Red;

            txeID.Text = gvCapacity.GetFocusedRowCellValue("CapacityID").ToString();

            slueCustomer.EditValue = CUSID;
            glueCategory.EditValue = CATEID;
            slueStyle.EditValue = STYLEID;
        }

        private void bbiPrintPreview_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gcCapacity.ShowPrintPreview();
        }

        private void bbiPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gcCapacity.Print();
        }
    }
}