using System;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using MDS00;
using DevExpress.XtraGrid.Views.Grid;
using System.Data.SqlClient;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.Drawing;
using System.IO;
using System.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using System.Collections;
using DevExpress.XtraPrinting;
using System.Diagnostics;

namespace DEV01
{
    public partial class DEV01 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        //Global Variable
        classConn db    = new classConn();
        classTools ct   = new classTools();

        SqlConnection mainConn = new classConn().MDS();
        SqlConnection conn; // Custom Query OtherDB
        string sql          = string.Empty;
        string picPart      = "\\\\172.16.0.190\\MDS_Project\\MDS\\Pictures\\";

        string currenTab    = string.Empty;
        string dosetOIDSMPL = string.Empty;
        bool PageFBVal      = false;

        string status_Mat = string.Empty;

        //private Functionality.Function FUNC = new Functionality.Function();

        public DEV01()
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

        private void XtraForm1_Load(object sender, EventArgs ex)
        {
            // TODO: This line of code loads data into the 'dsColor.ProductColor' table. You can move, or remove it, as needed.
            conn = db.DellInspiron15();
            //Tab : List of SMPL
            bbiNew.PerformClick();
            tabbedControlGroup1.SelectedTabPageIndex = 0;
            db.getGrid_SMPL(gridControl1);

            //Tab : Main Load
            radioGroup1.SelectedIndex           = 0;
            radioGroup2.SelectedIndex           = 0;
            radioGroup3.SelectedIndex           = 0;
            radioGroup4.SelectedIndex           = 1;
            radioGroup5.SelectedIndex           = 1;
            radioGroup6.SelectedIndex           = 1;
            dtRequestDate_Main.EditValue        = DateTime.Now;
            dtDeliveryRequest_Main.EditValue    = DateTime.Now;
            dtCustomerApproved_Main.EditValue   = DateTime.Now;
            dtACPRBy_Main.EditValue             = DateTime.Now;
            dtFBPRBy_Main.EditValue             = DateTime.Now;
            txtCreateBy_Main.EditValue          = "0";
            txtCreateDate_Main.EditValue        = DateTime.Now;
            txtUpdateBy_Main.EditValue          = "0";
            txtUpdateDate_Main.EditValue        = DateTime.Now;
            //Branch
            db.getGl("Select OIDBranch,Name as Branch From Branchs", mainConn, glBranch_Main, "OIDBranch", "Branch");
            //Department SalcSection
            db.getGl("Select OIDDEPT,Name as Department From Departments", mainConn,glSaleSection_Main, "OIDDEPT", "Department");
            //Season
            db.getGl("Select distinct s.Season as Season From( Select SUBSTRING( cast(Year(GETDATE())-1 as nvarchar(4)) , 3 , 2)+SeasonNo as Season From Season union Select SUBSTRING( cast(Year(GETDATE()) as nvarchar(4)) , 3 , 2) +SeasonNo as Season From Season union Select SUBSTRING( cast(Year(GETDATE())+1 as nvarchar(4)) , 3 , 2)+SeasonNo as Season From Season) as s", mainConn, glSeason_Main, "Season", "Season");
            //Customer
            db.getSl("Select OIDCUST,ShortName,Name From Customer", mainConn, slCustomer_Main, "OIDCUST", "Name");
            //GarmentCategory
            db.getGl("Select OIDGCATEGORY,CategoryName from GarmentCategory", mainConn, glCategoryDivision_Main, "OIDGCATEGORY", "CategoryName");
            //ProductStyle
            db.getSl("select OIDSTYLE,StyleName From ProductStyle", mainConn, slStyleName_Main, "OIDSTYLE", "StyleName");
            /*Set GridAdd Bind Color and Size*/
            gridView2.OptionsView.NewItemRowPosition = NewItemRowPosition.Top;
            gridControl2.DataSource = db.FGRequestDS();
            rep_glColor.Properties.NullText = ""; rep_glSize.Properties.NullText = ""; rep_glUnit.Properties.NullText = "";
            db.get_repGl("Select OIDCOLOR,ColorNo,ColorName From ProductColor", mainConn, rep_glColor, "OIDCOLOR", "ColorName");
            db.get_repGl("Select OIDSIZE,SizeNo,SizeName From ProductSize",mainConn,rep_glSize, "OIDSIZE", "SizeName");
            db.get_repGl("Select OIDUNIT,UnitName From Unit", mainConn,rep_glUnit, "OIDUNIT", "UnitName");
            int iNo = 1;
            gridView2.InitNewRow += (s, e) =>
            {
                GridView view = s as GridView;
                view.SetRowCellValue(e.RowHandle, view.Columns["No"], iNo++);
                view.SetRowCellValue(e.RowHandle, view.Columns["Unit"], 15);
            };
            //gridView2.OptionsBehavior.Editable = true;
            gridControl2.ProcessGridKey += (s, e) =>
            {
                if (e.KeyCode == Keys.Delete && e.Modifiers == Keys.Control)
                {
                    if (XtraMessageBox.Show("Delete row(s)?", "Delete rows dialog", MessageBoxButtons.YesNo) != DialogResult.Yes)
                        return;
                    GridControl grid    = s as GridControl;
                    GridView view       = grid.FocusedView as GridView;
                    view.DeleteSelectedRows();
                }
            };

            // Tab : Fabric Load
            gridView3.OptionsView.ShowGroupPanel = true;
            /*Vendor : Finish*/
            db.getSl("Select distinct v.OIDVEND,v.Code,v.Name From SMPLRequestFabric smplFB inner join Vendor v on v.OIDVEND = smplFB.OIDVEND", mainConn, slVendor_FB, "OIDVEND", "Name");
            /*FBColor : Finish*/
            db.getSl("Select distinct c.OIDCOLOR,c.ColorName From SMPLRequestFabric smplFB inner join ProductColor c on c.OIDCOLOR = smplFB.OIDCOLOR inner join Items i on i.OIDITEM = smplFB.OIDITEM Where i.MaterialType = 1", mainConn, slFBColor_FB, "OIDCOLOR", "ColorName");
            /*FBCode : Finish*/
            db.getSl("Select distinct i.OIDITEM,i.Code From SMPLRequestFabric smplFB inner join Items i on i.OIDITEM = smplFB.OIDITEM Where i.MaterialType = 1", mainConn, slFBCode_FB, "OIDITEM", "Code");
            /*FGColor : Finish*/
            db.getSl("Select distinct c.OIDCOLOR,c.ColorNo,c.ColorName From SMPLQuantityRequired q inner join ProductColor c on c.OIDCOLOR = q.OIDCOLOR inner join Items i on i.OIDCOLOR = c.OIDCOLOR Where i.MaterialType = 0", mainConn, slFGColor_FB, "OIDCOLOR", "ColorName");
            /*Currency : Finish*/
            db.getGl("Select distinct cc.OIDCURR,cc.Currency From SMPLRequestFabric smplFB inner join Currency cc on cc.OIDCURR = smplFB.OIDCURR", mainConn, glCurrency_FB, "OIDCURR", "Currency");
            // Tab : Materials
            // use GridControl 6,7,8
        }

        private void LoadData()
        {
            //StringBuilder sbSQL = new StringBuilder();
            //sbSQL.Append("SELECT OIDPayment AS No, Name, Description, DuedateCalculation, Status, CreatedBy, CreatedDate ");
            //sbSQL.Append("FROM PaymentTerm ");
            //sbSQL.Append("ORDER BY OIDPayment ");
            //new ObjDevEx.setGridControl(gridControl1, gridView1, sbSQL).getData(false, false, false, true);
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

            //txeID.Focus();
        }

        private void newMain()
        {
            //MessageBox.Show("newMain");
            bbiSave.Enabled                             = true;
            bbiEdit.Visibility                          = DevExpress.XtraBars.BarItemVisibility.Never;
            tabbedControlGroup1.SelectedTabPageIndex    = 1;
            btnGenSMPLNo.Enabled                        = false;
            gridControl2.DataSource                     = db.FGRequestDS();
            gridControl2.Enabled                        = true;

            /*UnLock Field*/
            glSaleSection_Main.Enabled      = true;
            txtReferenceNo_Main.Enabled     = true;
            glSeason_Main.Enabled           = true;
            slCustomer_Main.Enabled         = true;
            slStyleName_Main.Enabled        = true;
            txtSMPLPatternNo_Main.Enabled   = true;
            radioGroup3.Enabled             = true;

            /*TextEdit*/
            txtSMPLNo.EditValue = "";
            txtStatus.EditValue = "";
            txtReferenceNo_Main.EditValue = "";
            txtContactName_Main.EditValue = "";
            txtSMPLItemNo_Main.EditValue = "";
            txtModelName_Main.EditValue = "";
            txtSMPLPatternNo_Main.EditValue = "";
            txtSituation_Main.EditValue = "";
            txtStateArrangments_Main.EditValue = "";
            txtPictureFile_Main.Text = "";
            picMain.Image = null;

            /*GridLookup*/
            glBranch_Main.EditValue = "";
            glSaleSection_Main.EditValue = "";
            glSeason_Main.EditValue = "";
            glCategoryDivision_Main.EditValue = "";

            /*SearchLookup*/
            slCustomer_Main.EditValue   = "";
            slStyleName_Main.EditValue  = "";

            /*RadioGroup*/
            radioGroup1.SelectedIndex = 0;
            radioGroup2.SelectedIndex = 0;
            radioGroup3.SelectedIndex = 0;
            radioGroup4.SelectedIndex = 1;
            radioGroup5.SelectedIndex = 1;
            radioGroup6.SelectedIndex = 1;

            /*DateTime*/
            dtDeliveryRequest_Main.EditValue    = DateTime.Now;
            dtCustomerApproved_Main.EditValue   = DateTime.Now;
            dtACPRBy_Main.EditValue             = DateTime.Now;
            dtFBPRBy_Main.EditValue             = DateTime.Now;
        }

        private void newFabric()
        {
            //MessageBox.Show("newFabric");
            if (dosetOIDSMPL != "")
            {
                bbiSave.Visibility      = DevExpress.XtraBars.BarItemVisibility.Always;
                bbiRefresh.Visibility   = DevExpress.XtraBars.BarItemVisibility.Always;
                bbiEdit.Visibility      = DevExpress.XtraBars.BarItemVisibility.Never;
                bbiSave.Enabled = true;
                gridControl3.Enabled = true;
                btngetListFB_FB.Enabled = true;

                //Set New OIDFB
                txtFabricRacordID_FB.EditValue = db.get_newOIDFB();

                //Clear Form
                txtVendorFBCode_FB.EditValue = null;
                txtSampleLotNo_FB.EditValue = null;
                slVendor_FB.EditValue = null;
                slFBColor_FB.EditValue = null;
                slFBCode_FB.EditValue = null;
                slFGColor_FB.EditValue = null;
                txtComposition_FB.EditValue = null;
                txtWeightFB_FB.EditValue = null;
                txtWidthCuttable_FB.EditValue = null;
                txtPrice_FB.EditValue = null;
                glCurrency_FB.EditValue = null;
                txtTotalWidth_FB.EditValue = null;
                txtUsableWidth_FB.EditValue = null;
                txtImgUpload_FB.EditValue = null;
                picUpload_FB.Image = null;

                gcSize_Fabric.DataSource = null;
                gcList_Fabric.DataSource = null;

                db.getGrid_FBListSample(gridControl3, " And smplQR.OIDSMPL = " + dosetOIDSMPL + " ");
                db.getDgv("Select OIDGParts,GarmentParts From GarmentParts", gcPart_Fabric, mainConn);
            }
            else
            {
                ct.showWarningMessage("Please Back to Select List of Sample Request!");
                tabbedControlGroup1.SelectedTabPageIndex = 0;
            }
        }

        private void newMaterials()
        {
            //MessageBox.Show("newMaterials");
            if (dosetOIDSMPL == "")
            {
                ct.showInfoMessage("ไม่สามารถทำรายการได้ กรุณากลับไปเลือกรายการ SMPLNo ใหม่!"); return;
            }
            else
            {
                status_Mat = "new";
                bbiSave.Visibility          = DevExpress.XtraBars.BarItemVisibility.Always;
                bbiEdit.Visibility          = DevExpress.XtraBars.BarItemVisibility.Never;
                gridControl6.Enabled        = true;
                btnGettoLlist_Mat.Enabled   = true;
                gridControl7.DataSource     = null;

                // Set New OIDMat
                txtMatRecordID_Mat.EditValue = db.get_newOIDMat();

                // Clear Form
                glWorkStation_Mat.EditValue = null;
                slVendor_Mat.EditValue      = null;
                slVendor_Mat.EditValue      = null;
                slMatColor_Mat.EditValue    = null;
                slMatCode_Mat.EditValue     = null;
                glCurrency_Mat.EditValue    = null;

                txtVendorMatCode_Mat.Text = "";
                txtSampleLotNo_Mat.Text = "";
                txtMatComposition_Mat.Text = "";
                txtPrice_Mat.Text = "";
                txtSituation_Mat.Text = "";
                txtComment_Mat.Text = "";
                txtRemark_Mat.Text = "";
                txtPathFile_Mat.Text = "";
                picMat.Image = null;

                // Reload ListofMaterial
                db.getListofMaterial(gridControl8, dosetOIDSMPL);
            }
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //LoadData();
            //NewData();

            switch (currenTab)
            {
                /*
                Page is : List of Sample
                Page is : Main
                Page is : Fabric
                Page is : Material
                */
                case "Main"     : newMain(); break;
                case "Fabric"   : newFabric(); break;
                case "Material" : newMaterials(); break;
                default         : newMain();  break;
            }
        }

        private void saveMain()
        {
            //MessageBox.Show("saveMain");

            if (ct.doConfirm("Confirm Save SampleRequest ?") == true)
            {
                GridView gv = gridView2;
                /*Test Save SMPLQuantityRequired*/
                //Loop Row Item in GL2 And CheckDate PrepareData And Save to tbl SMPLQuantityRequired                
                int chk_i = 0;
                for (int ii = 0; ii < gv.DataRowCount; ii++)
                {
                    string No = gridView2.GetRowCellValue(ii, "No").ToString();
                    if (ct.chkCell_isnull(gv, "Color", 1, "เลือก Color ด้วยสิคร๊าบ! ขอร้องหละ") == true) { return; }
                    else if (ct.chkCell_isnull(gv, "Size", 2, "เลือก Size ด้วยสิคร๊าบ! ขอร้องหละ") == true) { return; }
                    else if (ct.chkCell_isnull(gv, "Unit", 4, "เลือก Unit ด้วยสิคร๊าบ! ขอร้องหละ") == true) { return; }
                    else if (Convert.ToInt32( gv.GetRowCellValue(ii, "Quantity").ToString()) <= 0)
                    {
                        ct.showInfoMessage("ป้อนจำนวนที่ตั้งแต่ 1 ขึ้นไปจ้า");
                        gv.FocusedColumn = gv.VisibleColumns[3];
                        gv.ShowEditor();
                        return;
                    }
                    else
                    {
                        string Color    = gridView2.GetRowCellValue(ii, "Color").ToString();
                        string Size     = gridView2.GetRowCellValue(ii, "Size").ToString();
                        string Quantity = gridView2.GetRowCellValue(ii, "Quantity").ToString();
                        string Unit     = gridView2.GetRowCellValue(ii, "Unit").ToString();

                        chk_i++;
                        Console.WriteLine(No + "," + Color + "," + Size + "," + Quantity + "," + Unit);
                    }
                    //MessageBox.Show(Color);
                }
                if (chk_i == 0)
                {
                    ct.showWarningMessage("กรุณาใส่ข้อมูลในตาราง Quantity Required ด้วยพะยาค่ะ");
                    return;
                }

                /*TextEdit*/
                string ReferenceNo          = txtReferenceNo_Main.EditValue.ToString().Trim().Replace("'", "''");
                string ContactName          = txtContactName_Main.EditValue.ToString().Trim().Replace("'", "''");
                string SMPLItem             = txtSMPLItemNo_Main.EditValue.ToString().Trim().Replace("'", "''");
                string ModelName            = txtModelName_Main.EditValue.ToString().Trim().Replace("'", "''");
                string SMPLPatternNo        = txtSMPLPatternNo_Main.EditValue.ToString().Trim().Replace("'", "''");
                string Situation            = txtSituation_Main.EditValue.ToString().Trim().Replace("'", "''");
                string StateArrangements    = txtStateArrangments_Main.EditValue.ToString().Trim().Replace("'", "''");
                //string PictureFile = txtPictureFile_Main.EditValue.ToString().Trim().Replace("'", "''");
                int CreatedBy               = 0;
                string CreatedDate          = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                int UpdatedBy               = 0;
                string UpdatedDate          = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                ///*GridLookup*/
                string OIDBranch    = glBranch_Main.EditValue.ToString();
                string SaleSection  = glSaleSection_Main.EditValue.ToString();
                string Season       = glSeason_Main.Text.ToString().Trim().Replace("'", "''");
                string OIDCATEGORY  = glCategoryDivision_Main.EditValue.ToString();

                ///*SearchLookup*/
                string OIDCUST  = slCustomer_Main.EditValue.ToString();
                string OIDSTYLE = slStyleName_Main.EditValue.ToString();

                ///*RadioGroup*/
                int SpecificationSize   = radioGroup1.SelectedIndex;
                int UseFor              = radioGroup2.SelectedIndex;
                int PatternSizeZone     = radioGroup3.SelectedIndex;
                int CustApproved        = radioGroup4.SelectedIndex;
                int ACPurRecBy          = radioGroup5.SelectedIndex;
                int FBPurRecBy          = radioGroup6.SelectedIndex;

                ///*DateTime*/
                string RequestDate      = Convert.ToDateTime(dtRequestDate_Main.Text.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                string DeliveryRequest  = Convert.ToDateTime(dtDeliveryRequest_Main.EditValue.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                string CustApprovedDate = Convert.ToDateTime(dtCustomerApproved_Main.EditValue.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                string ACPurRecDate     = Convert.ToDateTime(dtACPRBy_Main.EditValue.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                string FBPurRecDate     = Convert.ToDateTime(dtFBPRBy_Main.EditValue.ToString()).ToString("yyyy-MM-dd HH:mm:ss");

                // Check is null or Empty Data
                if (Season == "") {         ct.showWarningMessage("กรุณาเลือก Season!"); glSeason_Main.Focus(); return; }
                if (SaleSection == "") {    ct.showWarningMessage("กรุณาเลือก SaleSection!"); glSaleSection_Main.Focus(); return; }
                if (OIDCUST == "") {        ct.showWarningMessage("กรุณาเลือก Customer!"); slCustomer_Main.Focus(); return; }
                if (OIDCATEGORY == "") {    ct.showWarningMessage("กรุณาเลือก Category!"); glCategoryDivision_Main.Focus(); return; }
                if (OIDSTYLE == "") {       ct.showWarningMessage("กรุณาเลือก Style!"); slStyleName_Main.Focus(); return; }
                if (OIDBranch == "") {      ct.showWarningMessage("กรุณาเลือก Branch!"); glBranch_Main.Focus(); return; }
                if (ContactName == "") {    ct.showWarningMessage("กรุณาใส่ข้อมูล ContactName!"); txtContactName_Main.Focus(); return; }
                if (SMPLItem == "") {       ct.showWarningMessage("กรุณาใส่ข้อมูล SMPLItemNo!"); txtSMPLItemNo_Main.Focus(); return; }
                if (ModelName == "") {      ct.showWarningMessage("กรุณาใส่ข้อมูล ModelName!"); txtModelName_Main.Focus(); return; }
                if (SMPLPatternNo == "") {  ct.showWarningMessage("กรุณาใส่ข้อมูล SMPLPatternNo!"); txtSMPLPatternNo_Main.Focus(); return; }

                /*SpacialVar*/
                string s1 = "Select SUBSTRING( /*string*/'0000'+cast(SUBSTRING(SMPLNo,7,4)+1 as nvarchar(max)) ,/*start*/LEN('0000'+cast(SUBSTRING(SMPLNo,7,4)+1 as nvarchar(max)))-3 ,/*length*/4)+'-0'/*+cast(SUBSTRING(SMPLNo,12,1)+1 as nvarchar(max))*/ as genD4 From SMPLRequest Where OIDSMPL =(Select MAX(OIDSMPL) From SMPLRequest)";
                string genD4 = db.get_oneParameter(s1, mainConn, "genD4");

                string SMPLNo = Season + "S" + SaleSection + genD4; //Season+SaleSection+gen(D4)-0
                //MessageBox.Show(SMPLNo);
                string PictureFile = ct.uploadImg(txtPictureFile_Main, SMPLNo+"-FG");

                sql = "Insert Into SMPLRequest (SMPLNo,SMPLRevise,Status,ReferenceNo,RequestDate,SMPLItem,ModelName,OIDCUST,OIDCATEGORY,OIDSTYLE,OIDBranch,OIDDEPT,SMPLPatternNo,PatternSizeZone,Season,SpecificationSize,ContactName,DeliveryRequest,UseFor,Situation,StateArrangements,PictureFile,CustApproved,CustApprovedDate,ACPurRecBy,ACPurRecDate,FBPurRecBy,FBPurRecDate,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate)";
                sql += " Values('" + SMPLNo + "', 0 , 0 , '" + ReferenceNo + "', '" + RequestDate + "', '" + SMPLItem + "', '" + ModelName + "', " + OIDCUST + ", " + OIDCATEGORY + ", " + OIDSTYLE + ", " + OIDBranch + ", " + SaleSection + ", '" + SMPLPatternNo + "', " + PatternSizeZone + ", '" + Season + "', " + SpecificationSize + ", '" + ContactName + "', '" + DeliveryRequest + "', " + UseFor + ", '" + Situation + "', '" + StateArrangements + "', " + PictureFile + ", " + CustApproved + ", '" + CustApprovedDate + "', " + ACPurRecBy + ", '" + ACPurRecDate + "', " + FBPurRecBy + ", '" + FBPurRecDate + "', " + CreatedBy + ", '" + CreatedDate + "', " + UpdatedBy + ", '" + UpdatedDate + "')";

                Console.WriteLine(sql);
                int i = db.Query(sql, mainConn);
                if (i > 0)
                {
                    bool chkSave = false;
                    //Step1 : Get OIDSMPL ที่บันทึกเมื่อกี้มา
                    //string maxOIDSMPL = db.get_oneParameter("Select MAX(OIDSMPL) as maxOID From SMPLRequest",mainConn, "maxOID");
                    string maxOIDSMPL = db.get_oneParameter("Select Case When isnull(MAX(OIDSMPL),'') = '' Then 1 else max(OIDSMPL) end as maxOID From SMPLRequest", mainConn, "maxOID");
                    //Step2 : Loop List of Quantity Required And Save Data to tbl SMPLQuantityRequired
                    for (int j = 0; j < gv.DataRowCount; j++)
                    {
                        string Color    = gv.GetRowCellValue(j, "Color").ToString();
                        string Size     = gv.GetRowCellValue(j, "Size").ToString();
                        string Quantity = gv.GetRowCellValue(j, "Quantity").ToString();
                        string Unit     = gv.GetRowCellValue(j, "Unit").ToString();

                        string sql2 = "Insert Into SMPLQuantityRequired(OIDSMPL,OIDCOLOR,OIDSIZE,Quantity,OIDUnit)";
                        sql2 += " Values("+ maxOIDSMPL + ","+ Color + ","+ Size + ","+ Quantity + ","+ Unit + ")";
                        int i2 = db.Query(sql2, mainConn);
                        if (i2 > 0)
                        {
                            chkSave = true;
                        }
                    }
                    if (chkSave == true)
                    {
                        ct.showInfoMessage("Save Data is Successfull.");
                        dosetOIDSMPL = maxOIDSMPL;
                        //db.getGrid_SMPL(gridControl1);
                        newMain();
                        //Next to TabPage Fabric
                        tabbedControlGroup1.SelectedTabPageIndex = 2;
                    }
                    else
                    {
                        ct.showErrorMessage("Save SMPL is Failed!");
                        db.getGrid_SMPL(gridControl1);
                        //newMain();
                    }
                }
            }
        }

        private void saveFabric()
        {
            //MessageBox.Show("saveFabric");
            if (ct.doConfirm("Save SMPLFabric") == true)
            {
                //Step 0 : Check Data is null
                //Step 1 : Loop Insert Data to SMPLRequestFabric
                //Step 2 : Loop Insert Data to SMPLRequestFabricParts
                //Step 3 : Loop Insert Data to SMPLQuantityRequired
                //Step 4 : Loop Insert Data to SMPLRequestMaterial

                bool saveStatus = false;

                /*User Customize*/
                string SampleID         = dosetOIDSMPL;
                string FabricRecordID   = db.get_oneParameter("Select case When isnull( MAX(OIDSMPLFB),'') = '' Then 1 else max(OIDSMPLFB) end as MaxOIDFB From SMPLRequestFabric", mainConn, "MaxOIDFB");
                string OIDCOLOR         = slFBColor_FB.Text.ToString();    //sl
                string OIDVEND          = slVendor_FB.Text.ToString();     //sl
                string OIDITEM          = slFBCode_FB.Text.ToString();     //sl
                string OIDCOLORFG       = slFGColor_FB.Text.ToString();    //sl
                string VendFBCode       = txtVendorFBCode_FB.Text.ToString().Trim().Replace("'", "''");
                string SMPLotNo         = txtSampleLotNo_FB.Text.ToString().Trim().Replace("'", "''");

                /*Accept Null*/
                string Composition      = txtComposition_FB.Text.ToString().Trim().Replace("'", "''");      //accept null
                string FBType           = "null";
                string FBWeight         = txtWeightFB_FB.Text.ToString().Trim().Replace("'", "''");         //accept null
                string WidthCuttable    = txtWidthCuttable_FB.Text.ToString().Trim().Replace("'", "''");    //accept null
                string Price            = txtPrice_FB.Text.ToString();                                      //accept null
                string OIDCURR          = glCurrency_FB.Text.ToString();   //gl                             //accept null
                string TotalWidth       = txtTotalWidth_FB.Text.ToString();                                 //accept null
                string UsableWidth      = txtUsableWidth_FB.Text.ToString();                                //accept null
                string PathFile         = ct.uploadImg(txtImgUpload_FB,"FB");

                /*Note : SL/GL เช็คเงื่อนไขด้วย Text isnull แต่เอา Value ไปใช้ :: chkData is null */
                if (VendFBCode == "") {     ct.showWarningMessage("Please Key Vendor Fabric Code"); txtVendorFBCode_FB.Focus(); return; }
                else if (SMPLotNo == "") {  ct.showWarningMessage("Please Key Sample LotNo"); txtSampleLotNo_FB.Focus(); return; }
                else if (OIDVEND == "") {   ct.showWarningMessage("Please Select Vendor"); slVendor_FB.Focus(); return; }
                else if (OIDCOLOR == ""){   ct.showWarningMessage("Please Select FBColor"); slFBColor_FB.Focus(); return;}
                else if (OIDITEM == ""){    ct.showWarningMessage("Please Select FBCode"); slFBCode_FB.Focus(); return;}
                else if (OIDCOLORFG == ""){ ct.showWarningMessage("Please Select FGColor"); slFGColor_FB.Focus(); return;}
                else
                {
                    OIDCOLOR        = slFBColor_FB.EditValue.ToString();
                    OIDVEND         = slVendor_FB.EditValue.ToString();
                    OIDITEM         = slFBCode_FB.EditValue.ToString();

                    Composition     = ct.getVal_text(txtComposition_FB);    //ct.setNull(Composition,"s");
                    FBWeight        = ct.getVal_text(txtWeightFB_FB);       //ct.setNull(FBWeight,"s");
                    WidthCuttable   = ct.getVal_text(txtWidthCuttable_FB);  //ct.setNull(WidthCuttable,"s");

                    Price           = ct.getVal_num(txtPrice_FB);           //ct.setNull(Price,"i");
                    TotalWidth      = ct.getVal_num(txtTotalWidth_FB);      //ct.setNull(TotalWidth,"i");
                    UsableWidth     = ct.getVal_num(txtUsableWidth_FB);     //ct.setNull(UsableWidth,"i");
                    OIDCURR         = ct.getVal_gl(glCurrency_FB);          //ct.setNull_Gl(OIDCURR, glCurrency_FB);

                    //getChkBox to Array
                    ArrayList rows      = ct.getList_isChecked(gridView3);
                    ArrayList rgPart    = ct.getList_isChecked(gridView11);
                    if (rgPart.Count < 1) { ct.showWarningMessage("Please Select : GarmentPart"); gridView11.Focus(); return; } //ถ้าไม่ยอมเลือก GarmentPart จะไม่ยอมให้ Save นะจ๊ะ
                    if (rows.Count > 0)
                    {
                        try
                        {
                            // Loop list sample
                            for (int i = 0; i < rows.Count; i++)
                            {
                                DataRow row = rows[i] as DataRow;
                        
                                /*in Gridview3*/
                                string OIDSMPL      = row["OIDSMPL"].ToString();
                                string PatternNo    = row["SMPLPatternNo"].ToString();
                                string Color        = row["ColorName"].ToString();
                                string Size         = row["SizeName"].ToString();
                                string Quantity     = row["Quantity"].ToString();
                                string Unit         = row["UnitName"].ToString();
                                string OIDSMPLDT    = row["OIDSMPLDT"].ToString();

                                string sql1 = "Insert Into SMPLRequestFabric( OIDSMPLDT,OIDCOLOR,OIDVEND,OIDITEM,VendFBCode,SMPLotNo,Composition,FBType,FBWeight,WidthCuttable,Price,OIDCURR,TotalWidth,UsableWidth,PathFile )";
                                sql1 += " Values("+ OIDSMPLDT + ", "+ OIDCOLOR + ", "+ OIDVEND + ", "+ OIDITEM + ", N'"+ VendFBCode + "', N'"+ SMPLotNo + "', "+ Composition + ", "+ FBType + ", "+ FBWeight + ", "+ WidthCuttable + ", "+ Price + ", "+ OIDCURR + ", "+ TotalWidth + ", "+ UsableWidth + ", "+ PathFile + ") ";

                                Console.WriteLine(sql1);
                                int si = db.Query(sql1,mainConn);
                                if (si > 0)
                                {
                                    // GetMax FBID
                                    string maxFBID = db.get_oneParameter("Select case When isnull(max(OIDSMPLFB),'') = '' Then 1 else max(OIDSMPLFB) end as maxID From SMPLRequestFabric", mainConn, "maxID");
                                    // Loop GramentPart is Checked
                                    for (int j = 0; j < rgPart.Count; j++)
                                    {
                                        DataRow rgp = rgPart[j] as DataRow;

                                        /*in Gridview11*/
                                        string OIDGParts = rgp["OIDGParts"].ToString();

                                        string sql2 = "Insert Into SMPLRequestFabricParts(OIDSMPLFB,OIDSMPLDT,OIDGParts) Values(" + maxFBID + "," + OIDSMPLDT + "," + OIDGParts + ")";
                                        Console.WriteLine(sql2);
                                        int sj = db.Query(sql2, mainConn);
                                        if (sj > 0)
                                        {
                                            saveStatus = true;
                                        }
                                        else
                                        {
                                            ct.showErrorMessage("Can't Save to SMPLFabricPart!");
                                        }
                                    }//end - for gv11 gpart
                                }
                                else
                                {
                                    ct.showErrorMessage("Can't Save to SMPLFabric!");
                                }
                            }//end - for gv3
                        }
                        catch { }
                    }
                    else
                    {
                        ct.showWarningMessage("Reccord is null : กรุณาเลือกรายการที่ต้องการบันทึก"); gridView3.Focus(); return;
                    }
                }
                if (saveStatus == true)
                {
                    ct.showInfoMessage("Save Success");
                    newFabric();
                    tabbedControlGroup1.SelectedTabPageIndex = 3;
                }
                else
                {
                    ct.showErrorMessage("Can't Save SystemError! Please Contact Administrator.");
                }
            }//end-if
        }

        private void saveMaterials()
        {
            //MessageBox.Show("saveMaterials");
            if (ct.doConfirm("Save Material ?") == true)
            {
                bool saveStatus = false;

                //Check ChkboxRow > 0
                GridView gv1    = gridView6;
                GridView gv2    = gridView7;
                ArrayList rows  = ct.getList_isChecked(gv1);
                if (rows.Count > 0)
                {
                    // Columns Not Null
                    string OIDDEPT = glWorkStation_Mat.EditValue.ToString();
                    string OIDVEND = slVendor_Mat.EditValue.ToString();

                    // Columns Accept Null
                    string OIDITEM      = ct.getVal_sl(slMatCode_Mat);
                    string VendMTCode   = ct.getVal_text(txtVendorMatCode_Mat);
                    string SMPLotNo     = ct.getVal_text(txtSampleLotNo_Mat);
                    string MTColor      = ct.getVal_sl(slMatColor_Mat);
                    string Composition  = ct.getVal_text(txtMatComposition_Mat);
                    string Details      = "null";
                    string Price        = ct.getVal_num(txtPrice_Mat);
                    string OIDCURR      = ct.getVal_gl(glCurrency_Mat);
                    string Situation    = ct.getVal_text(txtSituation_Mat);
                    string Comment      = ct.getVal_text(txtComment_Mat);
                    string Remark       = ct.getVal_text(txtRemark_Mat);
                    string PathFile     = ct.uploadImg(txtPathFile_Mat, "Mat");

                    try
                    {
                        // Loop list sample
                        for (int i = 0; i < rows.Count; i++)
                        {
                            DataRow row = rows[i] as DataRow;

                            /*in gv1*/
                            string OIDSMPL      = row["OIDSMPL"].ToString();
                            string PatternNo    = row["SMPLPatternNo"].ToString();
                            string Color        = row["ColorName"].ToString();
                            //string Size       = row["SizeName"].ToString();
                            string Quantity     = row["Quantity"].ToString();
                            //string Unit       = row["UnitName"].ToString();
                            string OIDSMPLDT    = row["OIDSMPLDT"].ToString();

                            /* Special Custom Var in gv2 */ //get val in rowCellValue
                            string MTSize       = gv2.GetRowCellValue(i, "Size").ToString();
                            string Consumption  = ct.getVal_string(gv2.GetRowCellValue(i, "Consumption").ToString());
                            string OIDUNIT      = gv2.GetRowCellValue(i, "Unit").ToString();

                            //chkDup :: VendorMaterialCode , MaterialColor , MaterialSize
                            string eq = (MTColor == "null") ? "is" : "=";
                            string sql_chkDup = "Select VendMTCode From SMPLRequestMaterial Where (VendMTCode = " + VendMTCode + " and MTColor "+ eq + " " + MTColor + " and MTSize = " + MTSize + ")";
                            Console.WriteLine(sql_chkDup);
                            if (db.get(sql_chkDup, mainConn) == true) { ct.showWarningMessage("MatCode or MatColor or MatSize is Duplicate!");  }
                            else
                            {
                                string sql1 = "Insert Into SMPLRequestMaterial(OIDSMPLDT,OIDITEM,OIDVEND,OIDDEPT,VendMTCode,SMPLotNo,MTColor,MTSize,Consumption,OIDUNIT,Composition,Details,Price,OIDCURR,Situation,Comment,Remark,PathFile)";
                                sql1 += " Values(" + OIDSMPLDT + ", " + OIDITEM + ", " + OIDVEND + ", " + OIDDEPT + ", " + VendMTCode + ", " + SMPLotNo + ", " + MTColor + ", " + MTSize + ", " + Consumption + ", " + OIDUNIT + ", " + Composition + ", " + Details + ", " + Price + ", " + OIDCURR + ", " + Situation + ", " + Comment + ", " + Remark + ", " + PathFile + ")";

                                Console.WriteLine(sql1);
                                int si = db.Query(sql1, mainConn);
                                if (si > 0)
                                {
                                    saveStatus = true;
                                }
                                else
                                {
                                    ct.showErrorMessage("Can't Save to SMPLRequestMaterial!");
                                }
                            }//end-chkDup
                        } //end-for gv3
                    }
                    catch { }
                }
                else
                {
                    ct.showWarningMessage("Please Select Check List"); gv1.Focus(); return;
                }

                //--------------------------------------------------

                if (saveStatus == true)
                {
                    ct.showInfoMessage("Save Success");
                    newMaterials();
                }
                else
                {
                    ct.showErrorMessage("Can't Save Please Contact Administrator.");
                }
            } //end-if-saveMaterial

        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            switch (currenTab)
            {
                /* List of , Main , Fabric , Material */
                case "Main"     : saveMain(); break;
                case "Fabric"   : saveFabric(); break;
                case "Material" : saveMaterials(); break;
                default         : break;
            }

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

        private void gvGarment_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            //
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

        private void bbiExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //if (gridView1.RowCount > 0)
            //{
            //    string pathFile = new ObjSet.Folder(@"C:\MDS\Export\").GetPath() + "PaymentTermList_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
            //    gridView1.ExportToXlsx(pathFile);
            //    Process.Start(pathFile);
            //}

            // Check Part is Exist
            string root = @"C:/__MDS/Export/";
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }

            if (tabbedControlGroup1.SelectedTabPageIndex == 0)
            {
                //List of Sample
                if (gridView1.RowCount > 0)
                {
                    if (ct.doConfirm("Export Excel ?") == true)
                    {
                        string filePath = root + "SMPL-"+DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".xlsx";
                        gridView1.ExportToXlsx(filePath);
                        Process.Start(filePath);
                    }
                }
            }
            if (tabbedControlGroup1.SelectedTabPageIndex == 1)
            {
                //Main
                if (gridView2.RowCount > 0)
                {
                    if (ct.doConfirm("Export Excel ?") == true)
                    {
                        string filePath = root + "SMPLMain-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".xlsx";
                        gridView2.ExportToXlsx(filePath);
                        Process.Start(filePath);
                    }
                }
            }
            if (tabbedControlGroup1.SelectedTabPageIndex == 2)
            {
                //Fabric
                if (gridView4.RowCount > 0)
                {
                    if (ct.doConfirm("Export Excel ?") == true)
                    {
                        string filePath = root + "SMPLFB-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".xlsx";
                        gridView4.ExportToXlsx(filePath);
                        Process.Start(filePath);
                    }
                }
            }
            if (tabbedControlGroup1.SelectedTabPageIndex == 3)
            {
                //Material
                if (gridView8.RowCount > 0)
                {
                    if (ct.doConfirm("Export Excel ?") == true)
                    {
                        string filePath = root + "SMPLMaterial-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".xlsx";
                        gridView8.ExportToXlsx(filePath);
                        Process.Start(filePath);
                    }
                }
            }
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
            if (tabbedControlGroup1.SelectedTabPageIndex == 0)
            {
                gridView1.ShowPrintPreview();
            }
            if (tabbedControlGroup1.SelectedTabPageIndex == 1)
            {
                gridView2.ShowPrintPreview();
            }
            if (tabbedControlGroup1.SelectedTabPageIndex == 2)
            {
                gridView4.ShowPrintPreview();
            }
            if (tabbedControlGroup1.SelectedTabPageIndex == 3)
            {
                gridView8.ShowPrintPreview();
            }
        }

        private void bbiPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //gcPTerm.Print();
            if (tabbedControlGroup1.SelectedTabPageIndex == 0)
            {
                if (ct.doConfirm("Print Page?")==true) { gridView1.Print(); }
            }
            if (tabbedControlGroup1.SelectedTabPageIndex == 1)
            {
                if (ct.doConfirm("Print Page?") == true)
                {
                    gridView2.Print();
                }
            }
            if (tabbedControlGroup1.SelectedTabPageIndex == 2)
            {
                if (ct.doConfirm("Print Page?") == true)
                {
                    gridView4.Print();
                }
            }
            if (tabbedControlGroup1.SelectedTabPageIndex == 3)
            {
                if (ct.doConfirm("Print Page?") == true)
                {
                    gridView8.Print();
                }
            }
        }

         private void simpleButton2_Click(object sender, EventArgs e)
        {
            var frm = new DEV01_M04();
            frm.ShowDialog(this);
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            var frm = new DEV01_M06();
            frm.ShowDialog(this);
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            var frm = new DEV01_M11();
            frm.ShowDialog(this);
        }

        private void tabbedControlGroup1_SelectedPageChanged(object sender, DevExpress.XtraLayout.LayoutTabPageChangedEventArgs e)
        {
            /* Page is : List of Sample , Main , Fabric , Material */

            string tabName = e.Page.Text;
            currenTab = tabName;

            //Console.WriteLine("Page is : " + currenTab);

            btnGenSMPLNo.Enabled = false;

            // List of Sample
            if (tabName == "List of Sample")
            {
                bbiEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                bbiRefresh.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                db.getGrid_SMPL(gridControl1);
            }
            
            // Main
            if (tabName == "Main")
            {
                bbiRefresh.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                bbiNew.Enabled = true;
                string SaleSection = glSaleSection_Main.Text.ToString();
                //string ReferenceNo = txtReferenceNo_Main.Text.ToString();
                string Season = glSeason_Main.Text.ToString();

                if (SaleSection != "" /*&& ReferenceNo != ""*/ && Season != "")
                {
                    btnGenSMPLNo.Enabled = true;
                }
            }

            // Fabric
            if (tabName == "Fabric")
            {
                bbiEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                //bbiNew.Enabled = false;
                bbiSave.Enabled = false;
                if (dosetOIDSMPL != "" && PageFBVal == false)
                {
                    bbiSave.Enabled = false;
                    gridControl3.Enabled = false;
                    btngetListFB_FB.Enabled = false;
                    //bbiSave.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    //bbiEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                    db.getGrid_FBListSample(gridControl3, " And smplQR.OIDSMPL = " + dosetOIDSMPL + " ");
                    db.getDgv("Select OIDGParts,GarmentParts From GarmentParts", gcPart_Fabric, mainConn);
                    txtSampleID_FB.Text         = dosetOIDSMPL;
                    txtFabricRacordID_FB.Text   = db.get_oneParameter("Select case When ISNULL( MAX(OIDSMPLFB),'') = '' Then 1 Else MAX(OIDSMPLFB) End as maxFB From SMPLRequestFabric", mainConn, "maxFB");

                    //get List of Fabric
                    db.getListofFabric(gcList_Fabric, dosetOIDSMPL);
                }
                else
                {
                    //db.getGrid_FBListSample(gridControl3,"");
                }
            }

            // Material
            if (tabName == "Material")
            {
                bbiEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                bbiRefresh.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                if (dosetOIDSMPL != "")
                {
                    bbiNew.Enabled  = true;
                    bbiSave.Enabled = true;
                    txtSampleID_Mat.Text = dosetOIDSMPL;
                    txtMatRecordID_Mat.Text = db.get_newOIDMat();
                    db.getGrid_FBListSample(gridControl6, " And smplQR.OIDSMPL = " + dosetOIDSMPL + " ");
                    db.getGl("Select OIDDEPT,b.Name as Branch,d.Name as Department From Departments d inner join Branchs b on b.OIDBranch = d.OIDBRANCH", mainConn, glWorkStation_Mat, "OIDDEPT", "Department");
                    db.getSl("Select OIDVEND,Code,Name From Vendor", mainConn, slVendor_Mat, "OIDVEND", "Code");
                    db.getSl("Select OIDCOLOR,ColorName From ProductColor", mainConn, slMatColor_Mat, "OIDCOLOR", "ColorName");
                    db.getSl("Select OIDITEM,Code From Items Where MaterialType in(2,3)", mainConn, slMatCode_Mat, "OIDITEM", "Code");
                    db.getGl("Select OIDCURR,Currency From Currency", mainConn, glCurrency_Mat, "OIDCURR", "Currency");

                    //Lood Grid
                    gridControl7.DataSource = db.dsMat();

                    //Set rep_gl
                    db.get_repGl("Select OIDSIZE,SizeName From ProductSize", mainConn,rep_glSize_Mat, "OIDSIZE", "SizeName");
                    db.get_repGl("Select OIDUNIT,UnitName From Unit", mainConn,rep_glUnit_Mat, "OIDUNIT", "UnitName");
                    db.get_repGl("Select OIDCOLOR,ColorName From ProductColor", mainConn,rep_glColor_Mat, "OIDCOLOR", "ColorName");

                    // Test :: getListofMaterial
                    db.getListofMaterial(gridControl8,dosetOIDSMPL);
                }
                else { bbiSave.Enabled = false; }
            }
        }

        private void btnGenSMPLNo_Click(object sender, EventArgs e)
        {
            if (ct.doConfirm("Confirm Clone SampleRequest ? ") == true)
            {
                /*TextEdit*/
                string ReferenceNo          = txtReferenceNo_Main.EditValue.ToString().Trim().Replace("'", "''");
                string ContactName          = txtContactName_Main.EditValue.ToString().Trim().Replace("'", "''");
                string SMPLItem             = txtSMPLItemNo_Main.EditValue.ToString().Trim().Replace("'", "''");
                string ModelName            = txtModelName_Main.EditValue.ToString().Trim().Replace("'", "''");
                string SMPLPatternNo        = txtSMPLPatternNo_Main.EditValue.ToString().Trim().Replace("'", "''");
                string Situation            = txtSituation_Main.EditValue.ToString().Trim().Replace("'", "''");
                string StateArrangements    = txtStateArrangments_Main.EditValue.ToString().Trim().Replace("'", "''");
                string PictureFile          = txtPictureFile_Main.EditValue.ToString().Trim().Replace("'", "''");
                int CreatedBy               = 0;
                string CreatedDate          = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                int UpdatedBy               = 0;
                string UpdatedDate          = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                ///*GridLookup*/
                string OIDBranch    = glBranch_Main.EditValue.ToString();
                string SaleSection  = glSaleSection_Main.EditValue.ToString();
                string Season       = glSeason_Main.Text.ToString().Trim().Replace("'", "''");
                string OIDCATEGORY  = glCategoryDivision_Main.EditValue.ToString();

                ///*SearchLookup*/
                string OIDCUST  = slCustomer_Main.EditValue.ToString();
                string OIDSTYLE = slStyleName_Main.EditValue.ToString();

                ///*RadioGroup*/
                int SpecificationSize   = radioGroup1.SelectedIndex;
                int UseFor              = radioGroup2.SelectedIndex;
                int PatternSizeZone     = radioGroup3.SelectedIndex;
                int CustApproved        = radioGroup4.SelectedIndex;
                int ACPurRecBy          = radioGroup5.SelectedIndex;
                int FBPurRecBy          = radioGroup6.SelectedIndex;

                ///*DateTime*/
                string RequestDate      = Convert.ToDateTime(dtRequestDate_Main.Text.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                string DeliveryRequest  = Convert.ToDateTime(dtDeliveryRequest_Main.EditValue.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                string CustApprovedDate = Convert.ToDateTime(dtCustomerApproved_Main.EditValue.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                string ACPurRecDate     = Convert.ToDateTime(dtACPRBy_Main.EditValue.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                string FBPurRecDate     = Convert.ToDateTime(dtFBPRBy_Main.EditValue.ToString()).ToString("yyyy-MM-dd HH:mm:ss");

                // Check is null or Empty Data
                if (Season == "") {         ct.showWarningMessage("กรุณาเลือก Season!"); glSeason_Main.Focus(); return; }
                if (SaleSection == "") {    ct.showWarningMessage("กรุณาเลือก SaleSection!"); glSaleSection_Main.Focus(); return; }
                if (OIDCUST == "") {        ct.showWarningMessage("กรุณาเลือก Customer!"); slCustomer_Main.Focus(); return; }
                if (OIDCATEGORY == "") {    ct.showWarningMessage("กรุณาเลือก Category!"); glCategoryDivision_Main.Focus(); return; }
                if (OIDSTYLE == "") {       ct.showWarningMessage("กรุณาเลือก Style!"); slStyleName_Main.Focus(); return; }
                if (OIDBranch == "") {      ct.showWarningMessage("กรุณาเลือก Branch!"); glBranch_Main.Focus(); return; }
                if (ContactName == "") {    ct.showWarningMessage("กรุณาใส่ข้อมูล ContactName!"); txtContactName_Main.Focus(); return; }
                if (SMPLItem == "") {       ct.showWarningMessage("กรุณาใส่ข้อมูล SMPLItemNo!"); txtSMPLItemNo_Main.Focus(); return; }
                if (ModelName == "") {      ct.showWarningMessage("กรุณาใส่ข้อมูล ModelName!"); txtModelName_Main.Focus(); return; }
                if (SMPLPatternNo == "") {  ct.showWarningMessage("กรุณาใส่ข้อมูล SMPLPatternNo!"); txtSMPLPatternNo_Main.Focus(); return; }

                string oldSMPLNo    = txtSMPLNo.Text.ToString();
                string oldRevise    = db.get_oneParameter("Select SMPLRevise From SMPLRequest Where SMPLNo = '" + oldSMPLNo + "' ",mainConn, "SMPLRevise");
                int newRevise       = Convert.ToInt32(oldRevise) +1;
                string SMPLNo       = ct.genSMPLNo_Clone(oldSMPLNo,oldRevise);

                /*Upload Image*/
                string imgName      = txtPictureFile_Main.Text.ToString().Trim().Replace("'", "''");
                string newFileName  = string.Empty;
                if (imgName != "")
                {
                    try
                    {
                        string path         = "\\\\172.16.0.190\\MDS_Project\\MDS\\Pictures\\";
                        string filename     = imgName;
                        string extension    = Path.GetExtension(filename);
                        Random generator    = new Random();
                        string r = generator.Next(0, 999999).ToString("D4");
                        newFileName = SMPLNo + "-FG-" + DateTime.Now.ToString("yyyyMMdd") + "-" + r + extension;
                        File.Copy(filename, path + Path.GetFileName(newFileName));
                        //MessageBox.Show("Upload Files is Successfull.", "Upload Status");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Uplaod ไม่ได้ เนื่องจากมีไฟล์นี้ใน Directory ปัจจุบันแล้วจ้า!", "Upload Status");
                    }
                }

                sql = "Insert Into SMPLRequest (SMPLNo,SMPLRevise,Status,ReferenceNo,RequestDate,SMPLItem,ModelName,OIDCUST,OIDCATEGORY,OIDSTYLE,OIDBranch,OIDDEPT,SMPLPatternNo,PatternSizeZone,Season,SpecificationSize,ContactName,DeliveryRequest,UseFor,Situation,StateArrangements,PictureFile,CustApproved,CustApprovedDate,ACPurRecBy,ACPurRecDate,FBPurRecBy,FBPurRecDate,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate)";
                sql += " Values('" + SMPLNo + "', "+ newRevise + " , 0 , '" + ReferenceNo + "', '" + RequestDate + "', '" + SMPLItem + "', '" + ModelName + "', " + OIDCUST + ", " + OIDCATEGORY + ", " + OIDSTYLE + ", " + OIDBranch + ", " + SaleSection + ", '" + SMPLPatternNo + "', " + PatternSizeZone + ", '" + Season + "', " + SpecificationSize + ", '" + ContactName + "', '" + DeliveryRequest + "', " + UseFor + ", '" + Situation + "', '" + StateArrangements + "', '" + newFileName + "', " + CustApproved + ", '" + CustApprovedDate + "', " + ACPurRecBy + ", '" + ACPurRecDate + "', " + FBPurRecBy + ", '" + FBPurRecDate + "', " + CreatedBy + ", '" + CreatedDate + "', " + UpdatedBy + ", '" + UpdatedDate + "')"; 

                Console.WriteLine(sql);
                int i = db.Query(sql, mainConn);
                if (i > 0)
                {
                    ct.showInfoMessage("Clone Data is Successfull.");
                    db.getGrid_SMPL(gridControl1);
                    newMain();
                }
            }//end-if
        }

        private void getcell(string cellName)
        {
            gridView1.GetFocusedRowCellValue(cellName).ToString();
        }
        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            DXMouseEventArgs ea = e as DXMouseEventArgs;
            GridView view       = sender as GridView;
            GridHitInfo info = view.CalcHitInfo(ea.Location);
            if (info.InRow || info.InRowCell)
            {
                bbiSave.Enabled = false;
                PageFBVal       = false;
                bbiEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always; /*.Never = hide , .Always = show*/

                /*Lock Field*/
                glSaleSection_Main.Enabled      = false;
                txtReferenceNo_Main.Enabled     = false;
                glSeason_Main.Enabled           = false;
                slCustomer_Main.Enabled         = false;
                slStyleName_Main.Enabled        = false;
                txtSMPLPatternNo_Main.Enabled   = false;
                radioGroup3.Enabled             = false;

                GridView gv = gridView1;
                string OID = gv.GetFocusedRowCellValue("OIDSMPL").ToString();
                string status = "Select (case Status when 0 then 'New' when 1 then 'Wait Approved' when 2 then 'Customer Approved' end) as Status ";
                status += " From SMPLRequest Where OIDSMPL = '" + OID + "'";

                /*TextEdit*/
                txtSMPLNo.EditValue                 = ct.getcell(gv,"SMPLNo"); ;
                txtStatus.EditValue                 = db.get_oneParameter(status, mainConn,"Status");
                txtReferenceNo_Main.EditValue       = db.getDataFrom_SMPL("ReferenceNo", OID);
                txtContactName_Main.EditValue       = db.getDataFrom_SMPL("ContactName", OID);
                txtSMPLItemNo_Main.EditValue        = gv.GetFocusedRowCellValue("SMPLItem").ToString();
                txtModelName_Main.EditValue         = db.getDataFrom_SMPL("ModelName", OID);
                txtSMPLPatternNo_Main.EditValue     = gv.GetFocusedRowCellValue("SMPLPatternNo").ToString();
                txtSituation_Main.EditValue         = db.getDataFrom_SMPL("Situation", OID);
                txtStateArrangments_Main.EditValue  = db.getDataFrom_SMPL("StateArrangements", OID);

                /*GridLookup*/
                glBranch_Main.EditValue             = Convert.ToInt32(db.getDataFrom_SMPL("OIDBranch", OID));
                glSaleSection_Main.EditValue        = Convert.ToInt32(db.getDataFrom_SMPL("OIDDEPT", OID));
                glSeason_Main.EditValue             = gv.GetFocusedRowCellValue("Season").ToString();
                glCategoryDivision_Main.EditValue   = Convert.ToInt32(db.getDataFrom_SMPL("OIDCATEGORY", OID));

                /*SearchLookup*/
                slCustomer_Main.EditValue           = Convert.ToInt32(db.getDataFrom_SMPL("OIDCUST", OID));
                slStyleName_Main.EditValue          = Convert.ToInt32(db.getDataFrom_SMPL("OIDSTYLE", OID));

                /*RadioGroup*/
                radioGroup1.SelectedIndex           = Convert.ToInt32(gv.GetFocusedRowCellValue("SpecificationSize"));
                radioGroup2.SelectedIndex           = Convert.ToInt32(gv.GetFocusedRowCellValue("UseFor"));
                radioGroup3.SelectedIndex           = Convert.ToInt32(gv.GetFocusedRowCellValue("PatternSizeZone"));
                radioGroup4.SelectedIndex           = Convert.ToInt32(db.getDataFrom_SMPL("CustApproved", OID));
                radioGroup5.SelectedIndex           = Convert.ToInt32(db.getDataFrom_SMPL("ACPurRecBy", OID));
                radioGroup6.SelectedIndex           = Convert.ToInt32(db.getDataFrom_SMPL("FBPurRecBy", OID));
                
                /*DateTime*/
                dtRequestDate_Main.EditValue        = db.getDataFrom_SMPL("RequestDate", OID);
                dtDeliveryRequest_Main.EditValue    = db.getDataFrom_SMPL("DeliveryRequest", OID);
                dtCustomerApproved_Main.EditValue   = db.getDataFrom_SMPL("CustApprovedDate", OID);
                dtACPRBy_Main.EditValue             = db.getDataFrom_SMPL("ACPurRecDate", OID);
                dtFBPRBy_Main.EditValue             = db.getDataFrom_SMPL("FBPurRecDate", OID);

                /*Picture*/
                //string filePath = "\\\\172.16.0.190\\MDS_Project\\MDS\\Pictures\\";
                string picName  = db.getDataFrom_SMPL("PictureFile", OID);
                string fullPart = picPart + picName;
                if (picName != "")
                {
                    try {
                        txtPictureFile_Main.Text = fullPart;
                        picMain.Image = Image.FromFile(fullPart);
                    }
                    catch (Exception ex)
                    {
                        //ct.showErrorMessage("Files Image is not Found! or Files was Rename to New Files!");
                    }
                }
                else
                {
                    txtPictureFile_Main.Text = "";
                    picMain.Image = null;
                }

                /*Show Data to GridView2*/
                db.getDgv("Select ROW_NUMBER() OVER(ORDER BY OIDSMPLDT ASC) AS No,OIDCOLOR as Color,OIDSIZE as Size,Quantity,OIDUnit as Unit From SMPLQuantityRequired Where OIDSMPL = "+ OID +" ", gridControl2,mainConn);
                //db.getGrid_FBListSample(gridControl3, " And smplQR.OIDSMPL = " + OID + " ");
                dosetOIDSMPL = OID;
                gridControl2.Enabled = false;

                //Next to TabPage Main
                tabbedControlGroup1.SelectedTabPageIndex = 1;
                btnGenSMPLNo.Enabled = true;
            }
        }

        private void glSeason_Main_EditValueChanged(object sender, EventArgs e)
        {
            // Function Auto Save :: ไม่เอาจ้าาา
            /*ถ้ามีการเลือก Season ให้ทำการเช็คค่า 1.SaleSection, 2.ReferenceNo, 3.Season | ถ้ามีข้อมูลใน 3 รายการนี้ และ สถารนะเป็น New_Main ให้ทำการ AutoSave เป็น OID ใหม่ในตาราง*/
            //string Season = glSeason_Main.EditValue.ToString();
            //MessageBox.Show(val);

            string SaleSection  = glSaleSection_Main.Text.ToString();
            string ReferenceNo  = txtReferenceNo_Main.Text.ToString();
            string Season       = glSeason_Main.EditValue.ToString();

            if (SaleSection != "" && ReferenceNo != "" && Season != "")
            {
                //btnGenSMPLNo.Enabled = true;
                //MessageBox.Show("Auto Save Success!");
            }
        }

        private void btnOpenImg_Main_Click(object sender, EventArgs e)
        {
            ct.openFile_Image(xtraOpenFileDialog1,txtPictureFile_Main,picMain);
        }

        private void updateMain()
        {
            if (ct.doConfirm("Update SampleRequest ?") == true)
            {
                //chk Quantity Required
                //GridView gv = gridView2;
                //int chk_i = 0;
                //if (gv.RowCount <= 0)
                //{
                //    ct.showWarningMessage("Please Push Data to List Quantity Required"); gv.Focus(); return;
                //}
                //else
                //{
                //    for (int i = 0; i < gv.RowCount; i++)
                //    {
                //        if (gv.GetRowCellValue(i, "Color").ToString() == "") { MessageBox.Show("เลือก Color ด้วยสิคร๊าบ! ขอร้องหละ"); return; } 
                //        if (gv.GetRowCellValue(i, "Size").ToString() == "") { MessageBox.Show("เลือก Size ด้วยสิคร๊าบ! ขอร้องหละ"); return; } 
                //        if (gv.GetRowCellValue(i, "Unit").ToString() == "") { MessageBox.Show("เลือก Unit ด้วยสิคร๊าบ! ขอร้องหละ"); return; } 
                //        if (gv.GetRowCellValue(i, "Quantity").ToString() == "") { MessageBox.Show("เลือก Quantity ด้วยสิคร๊าบ! ขอร้องหละ"); return; }
                //        if (Convert.ToInt32(gv.GetRowCellValue(i, "Quantity").ToString()) < 1)
                //        {
                //            MessageBox.Show("เลือก Quantity ตั้งแต่ 1 ขึ้นไป ขอร้องหละ"); return;
                //        }
                //        else
                //        {
                //            string No       = gv.GetRowCellValue(i, "No").ToString();
                //            string Color    = gv.GetRowCellValue(i, "Color").ToString();
                //            string Size     = gv.GetRowCellValue(i, "Size").ToString();
                //            string Quantity = gv.GetRowCellValue(i, "Quantity").ToString();
                //            string Unit     = gv.GetRowCellValue(i, "Unit").ToString();

                //            chk_i++;
                //            Console.WriteLine(No + "," + Color + "," + Size + "," + Quantity + "," + Unit);
                //        }
                //    }
                //}
                //if (chk_i == 0)
                //{
                //    ct.showWarningMessage("Please Push Data to List Quantity Required"); gv.Focus(); return;
                //}

                /*TextEdit*/
                string SMPLNo               = txtSMPLNo.Text.ToString().Trim().Replace("'","''");
                string ReferenceNo          = txtReferenceNo_Main.EditValue.ToString().Trim().Replace("'", "''");
                string ContactName          = txtContactName_Main.EditValue.ToString().Trim().Replace("'", "''");
                string SMPLItem             = txtSMPLItemNo_Main.EditValue.ToString().Trim().Replace("'", "''");
                string ModelName            = txtModelName_Main.EditValue.ToString().Trim().Replace("'", "''");
                string SMPLPatternNo        = txtSMPLPatternNo_Main.EditValue.ToString().Trim().Replace("'", "''");
                string Situation            = txtSituation_Main.EditValue.ToString().Trim().Replace("'", "''");
                string StateArrangements    = txtStateArrangments_Main.EditValue.ToString().Trim().Replace("'", "''");
                string PictureFile          = txtPictureFile_Main.Text.ToString().Trim().Replace("'", "''");
                int UpdatedBy               = 0; //UpdateBy User Login
                string UpdatedDate          = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                ///*GridLookup*/
                string OIDBranch    = glBranch_Main.EditValue.ToString();
                string SaleSection  = glSaleSection_Main.EditValue.ToString();
                string Season       = glSeason_Main.Text.ToString().Trim().Replace("'", "''");
                string OIDCATEGORY  = glCategoryDivision_Main.EditValue.ToString();

                ///*SearchLookup*/
                string OIDCUST  = slCustomer_Main.EditValue.ToString();
                string OIDSTYLE = slStyleName_Main.EditValue.ToString();

                ///*RadioGroup*/
                int SpecificationSize   = radioGroup1.SelectedIndex;
                int UseFor              = radioGroup2.SelectedIndex;
                int PatternSizeZone     = radioGroup3.SelectedIndex;
                int CustApproved        = radioGroup4.SelectedIndex;
                int ACPurRecBy          = radioGroup5.SelectedIndex;
                int FBPurRecBy          = radioGroup6.SelectedIndex;

                ///*DateTime*/
                string RequestDate      = Convert.ToDateTime(dtRequestDate_Main.Text.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                string DeliveryRequest  = Convert.ToDateTime(dtDeliveryRequest_Main.EditValue.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                string CustApprovedDate = Convert.ToDateTime(dtCustomerApproved_Main.EditValue.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                string ACPurRecDate     = Convert.ToDateTime(dtACPRBy_Main.EditValue.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                string FBPurRecDate     = Convert.ToDateTime(dtFBPRBy_Main.EditValue.ToString()).ToString("yyyy-MM-dd HH:mm:ss");

                // Check is null or Empty Data :: Not Null
                if (Season == "") {         ct.showWarningMessage("กรุณาเลือก Season!"); glSeason_Main.Focus(); return; }
                if (SaleSection == "") {    ct.showWarningMessage("กรุณาเลือก SaleSection!"); glSaleSection_Main.Focus(); return; }
                if (OIDCUST == "") {        ct.showWarningMessage("กรุณาเลือก Customer!"); slCustomer_Main.Focus(); return; }
                if (OIDCATEGORY == "") {    ct.showWarningMessage("กรุณาเลือก Category!"); glCategoryDivision_Main.Focus(); return; }
                if (OIDSTYLE == "") {       ct.showWarningMessage("กรุณาเลือก Style!"); slStyleName_Main.Focus(); return; }
                if (OIDBranch == "") {      ct.showWarningMessage("กรุณาเลือก Branch!"); glBranch_Main.Focus(); return; }
                if (ContactName == "") {    ct.showWarningMessage("กรุณาใส่ข้อมูล ContactName!"); txtContactName_Main.Focus(); return; }
                if (SMPLItem == "") {       ct.showWarningMessage("กรุณาใส่ข้อมูล SMPLItemNo!"); txtSMPLItemNo_Main.Focus(); return; }
                if (ModelName == "") {      ct.showWarningMessage("กรุณาใส่ข้อมูล ModelName!"); txtModelName_Main.Focus(); return; }
                if (SMPLPatternNo == "") {  ct.showWarningMessage("กรุณาใส่ข้อมูล SMPLPatternNo!"); txtSMPLPatternNo_Main.Focus(); return; }

                string newFileName = ct.uploadImg(txtPictureFile_Main, "FG");

                sql = "Update SMPLRequest Set OIDBranch="+ OIDBranch + ",RequestDate='" + RequestDate + "',SpecificationSize=" + SpecificationSize + ",ContactName='" + ContactName + "',DeliveryRequest='" + DeliveryRequest + "',UseFor=" + UseFor + ",SMPLItem='" + SMPLItem + "',ModelName='" + ModelName + "',OIDCATEGORY=" + OIDCATEGORY + ",Situation='" + Situation + "',StateArrangements='" + StateArrangements + "',CustApproved=" + CustApproved + ",CustApprovedDate='" + CustApprovedDate + "',ACPurRecBy=" + ACPurRecBy + ",ACPurRecDate='" + ACPurRecDate + "',FBPurRecBy=" + FBPurRecBy + ",FBPurRecDate='" + FBPurRecDate + "' /*,PictureFile=" + newFileName + "*/ WHERE SMPLNo = '"+SMPLNo+"' ";

                Console.WriteLine(sql);
                int i = db.Query(sql, mainConn);
                if (i > 0)
                {
                    ct.showInfoMessage("Update is Success.");
                    db.getGrid_SMPL(gridControl1);
                    newMain();
                }
            }
        }

        private void updateFabric()
        {
            bool isUpdate = false;

            if (ct.doConfirm("Updat Fabric?") == true && txtSampleID_FB.Text.ToString() != "" && txtFabricRacordID_FB.Text.ToString() != "")
            {
                string fbid = txtFabricRacordID_FB.Text.ToString();
                string QDT  = db.get_oneParameter("Select OIDSMPLDT From SMPLRequestFabric Where OIDSMPLFB = "+ fbid + " ",mainConn, "OIDSMPLDT");

                //Not null
                string VendFBCode   = ct.getVal_text(txtVendorFBCode_FB); //return null
                string SampleLotNo  = ct.getVal_text(txtSampleLotNo_FB);
                string Vendor       = ct.getVal_sl(slVendor_FB);
                string FBColor      = ct.getVal_sl(slFBColor_FB);
                string FBCode       = ct.getVal_sl(slFBCode_FB);
                string FGColor      = ct.getVal_sl(slFGColor_FB);

                //Accept Null
                string Composition  = ct.getVal_text(txtComposition_FB);
                string weight       = ct.getVal_text(txtWeightFB_FB);
                string widthCut     = ct.getVal_text(txtWidthCuttable_FB);
                string price        = ct.getVal_num(txtPrice_FB);
                string Currency     = ct.getVal_gl(glCurrency_FB);
                string TotalWidth   = ct.getVal_num(txtTotalWidth_FB);
                string UsableWidth  = ct.getVal_num(txtUsableWidth_FB);

                //chkNull
                if (VendFBCode == "null") {         ct.showWarningMessage("Please Key Vendor Fabric Code!"); txtVendorFBCode_FB.Focus(); return; }
                else if (SampleLotNo == "null") {   ct.showWarningMessage("Please Key SampleLotNo!"); txtSampleLotNo_FB.Focus(); return; }
                else if (Vendor == "null") {        ct.showWarningMessage("Please Select Vendor!"); slVendor_FB.Focus(); return; }
                else if (FBColor == "null") {       ct.showWarningMessage("Please Select FBColor!"); slFBColor_FB.Focus(); return; }
                else if (FBCode == "null") {        ct.showWarningMessage("Please Select FBCode!"); slFBCode_FB.Focus(); return; }
                else if (FGColor == "null") {       ct.showWarningMessage("Please Select FGColor!"); slFGColor_FB.Focus(); return; }
                else
                {
                    string sql = "Update SMPLRequestFabric set VendFBCode = "+ VendFBCode + ", SMPLotNo = "+ SampleLotNo + ", OIDVEND = "+ Vendor + ",OIDCOLOR = "+ FBColor + ",OIDITEM = "+ FBCode + " /*FGColor ต้องไป Join SMPLQuantity*/ ,Composition = "+ Composition + ", FBWeight = "+ weight + ",WidthCuttable = "+ widthCut + ",Price = "+ price + ",TotalWidth = "+ TotalWidth + ",UsableWidth = "+ UsableWidth + ",OIDCURR = "+ Currency + " ";
                    sql += " Where OIDSMPLFB = "+ fbid + " ";
                    Console.WriteLine(sql);
                    db.Query(sql,mainConn);

                    string sql2 = "Delete SMPLRequestFabricParts Where OIDSMPLFB = "+ fbid + " ";
                    Console.WriteLine(sql2);
                    db.Query(sql2,mainConn);

                    ArrayList arow = ct.getList_isChecked(gridView11);
                    if (arow.Count > 0)
                    {
                        try
                        {
                            for (int i = 0; i < arow.Count; i++)
                            {
                                DataRow r = arow[i] as DataRow;
                                string OIDGpart = r["OIDGParts"].ToString();
                                string sql3 = "Insert Into SMPLRequestFabricParts(OIDSMPLFB,OIDSMPLDT,OIDGParts) Values("+ fbid + ","+ QDT + ","+ OIDGpart + ")";
                                Console.WriteLine(sql3);
                                int qi = db.Query(sql3,mainConn);
                                if (qi > 0)
                                {
                                    isUpdate = true;
                                }
                            }
                        }
                        catch { }
                    }
                }
                refreshFabric();
                db.getListofFabric(gcList_Fabric,dosetOIDSMPL);

                if (isUpdate == true)
                {
                    ct.showInfoMessage("Update Success.");
                }
                else
                {
                    ct.showErrorMessage("Can't Update. Please Contact Administrator!");
                }
            }
        }

        private void updateMaterials()
        {
            bool statusUpdate = false;

            if (ct.doConfirm("Update Material?")==true)
            {
                GridView g = gridView7;
                //Not null :
                if (glWorkStation_Mat.Text.ToString() != "" || slVendor_Mat.Text.ToString() != "")
                {
                    //Var Set Update
                    string MatID        = txtMatRecordID_Mat.Text.ToString();
                    string WorkStation  = glWorkStation_Mat.EditValue.ToString();
                    string Vendor       = slVendor_Mat.EditValue.ToString();
                    string vendMatCode  = ct.getVal_text(txtVendorMatCode_Mat);
                    string Lotno        = ct.getVal_text(txtSampleLotNo_Mat);
                    string Composition  = ct.getVal_text(txtMatComposition_Mat);
                    //string matColor     = ct.getVal_sl(slMatColor_Mat);
                    string matCode      = ct.getVal_sl(slMatCode_Mat);
                    string price        = ct.getVal_num(txtPrice_Mat);
                    string currency     = ct.getVal_gl(glCurrency_Mat);
                    string situation    = ct.getVal_text(txtSituation_Mat);
                    string Comment      = ct.getVal_text(txtComment_Mat);
                    string Remark       = ct.getVal_text(txtRemark_Mat);
                    string pathFile     = ct.getVal_text(txtPathFile_Mat); /*ยังไม่ Update อันนี้นะจ๊ะ*/

                    // Special Var in Gridview
                    string matColor     = (g.GetRowCellValue(0,"Color").ToString() == "") ? "null" : g.GetRowCellValue(0, "Color").ToString();
                    string Consumption  = (g.GetRowCellValue(0, "Consumption").ToString() == "") ? "null" : g.GetRowCellValue(0, "Consumption").ToString();
                    string Size         = g.GetRowCellValue(0, "Size").ToString(); //จะต้องไม่เป็นค่า Null แน่นอน เพราะดึงมาจาก Master

                    // SqlUpdate
                    string sql = "Update SMPLRequestMaterial Set OIDDEPT = " + WorkStation + ",OIDVEND = " + Vendor + ",VendMTCode=" + vendMatCode + ",SMPLotNo=" + Lotno + ",Composition=" + Composition + ",MTColor=" + matColor + ",Consumption=" + Consumption + ",OIDITEM=" + matCode + ",Price=" + price + ",OIDCURR=" + currency + ",Situation=" + situation + ",Comment=" + Comment + ",Remark=" + Remark + " ";
                    sql += " Where OIDSMPLMT = " + txtMatRecordID_Mat.Text.ToString() + " ";

                    // CheckUpdate รับค่าจาก Form มาเช็คใน Database 3 ตัว ถ้าตรงกันหมด = ไม่มีอะไรเปลี่ยนแปลง >> แล้วถ้ามีอันไหนไม่ตรงกัน = ให้ chkDup ก่อน Update
                    string db_vendMatCode   = db.get_oneParameter("Select VendMTCode From SMPLRequestMaterial Where OIDSMPLMT = "+ MatID + " ",mainConn, "VendMTCode");
                    db_vendMatCode          = (db_vendMatCode == "") ? "null" : "N'"+db_vendMatCode+"'";
                    string db_matColor      = db.get_oneParameter("Select MTColor From SMPLRequestMaterial Where OIDSMPLMT = " + MatID + " ",mainConn, "MTColor");
                    db_matColor             = (db_matColor == "") ? "null" : db_matColor;
                    string db_matSize       = db.get_oneParameter("Select MTSize From SMPLRequestMaterial Where OIDSMPLMT = " + MatID + " ",mainConn, "MTSize");
                    Console.WriteLine(db_vendMatCode+","+ vendMatCode+"\n" +db_matColor+","+ matColor+"\n" +db_matSize+","+ Size);

                    if (db_vendMatCode == vendMatCode && db_matColor == matColor && db_matSize == Size)
                    {
                        //ct.showInfoMessage("Math and Normal Update");
                        Console.WriteLine(sql);
                        int i = db.Query(sql, mainConn);
                        if (i > 0)
                        {
                            statusUpdate = true;
                        }
                    }
                    else
                    {
                        //ct.showInfoMessage("Not Math Some Field is Changed! > chkDuplicate");
                        // chkDup :: VenMatCode , MatColor , MatSize
                        string eq = (matColor == "null") ? "is" : "=";
                        string sql_chkDup = "Select VendMTCode From SMPLRequestMaterial Where (VendMTCode = " + vendMatCode + " and MTColor " + eq + " " + matColor + " and MTSize = " + Size + ")";
                        Console.WriteLine(sql_chkDup);
                        if (db.get(sql_chkDup, mainConn) == true) { ct.showWarningMessage("MatCode or MatColor or MatSize is Duplicate!"); return; }
                        else
                        {
                            Console.WriteLine(sql);
                            int i = db.Query(sql, mainConn);
                            if (i > 0)
                            {
                                statusUpdate = true;
                            }
                        }
                    }
                }
                else
                {
                    ct.showWarningMessage("Please Select WorkStation and Vendor");
                }

                // Check Update is Successfull
                if (statusUpdate == true)
                {
                    ct.showInfoMessage("Update Completed");
                    newMaterials();
                }
                else
                {
                    ct.showErrorMessage("Update is Failed. Please Contact Administrator!");
                }
            }
        }

        private void bbiEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            switch (currenTab)
            {
                /* List of Sample , Main , Fabric , Material */
                case "Main"     : updateMain(); break;
                case "Fabric"   : updateFabric(); break;
                case "Material" : updateMaterials(); break;
                default         : updateMain(); break;
            }
        }

        private void gridView3_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            ArrayList rows = ct.getList_isChecked(gridView3);

            if (rows.Count > 0)
            {
                PageFBVal = true;

                try
                {
                    // Create DataTable
                    DataTable dt = new DataTable();

                    // Add Columns Header into DataTable
                    dt.Columns.Add("No", typeof(string));
                    dt.Columns.Add("VendFBCode", typeof(string));
                    dt.Columns.Add("Composition", typeof(string));
                    dt.Columns.Add("FBWeight", typeof(string));
                    dt.Columns.Add("ColorName", typeof(string));
                    dt.Columns.Add("SMPLotNo", typeof(string));
                    dt.Columns.Add("Supplier", typeof(string));
                    dt.Columns.Add("NAVCode", typeof(string));

                    int listfbNo = 1;
                    string sqlcmd = string.Empty;
                    for (int i = 0; i < rows.Count; i++)
                    {
                        DataRow row = rows[i] as DataRow;
                        string PatternNo = row["SMPLPatternNo"].ToString();
                        string Color = row["ColorName"].ToString();
                        string Size = row["SizeName"].ToString();
                        string Quantity = row["Quantity"].ToString();

                        /*Set size*/
                        sqlcmd += "Select '" + Size + "' as SizeName Union ";

                        /* Add to List Fabric*/
                        dt.Rows.Add(new object[] {
                            listfbNo++
                            ,txtVendorFBCode_FB.Text.Trim().ToString().Replace("'","''")
                            ,txtComposition_FB.Text.Trim().ToString().Replace("'","''")
                            ,txtWeightFB_FB.Text.Trim().ToString().Replace("'","''")
                            ,slFBColor_FB.Text.ToString()
                            ,txtSampleLotNo_FB.Text.Trim().ToString().Replace("'","''")
                            ,slVendor_FB.Text.ToString()
                            ,slFBCode_FB.Text.ToString()
                        });
                    }
                    gcList_Fabric.DataSource = dt;

                    int length = sqlcmd.Length;
                    sqlcmd = sqlcmd.Substring(0, length - 6);
                    Console.WriteLine(sqlcmd);
                    db.getDgv(sqlcmd, gcSize_Fabric, mainConn);
                }
                catch { }
            }
            else
            {
                PageFBVal = false;
                gcSize_Fabric.DataSource = null;
                gcList_Fabric.DataSource = null;
            }
        }

        private void btngetListFB_FB_Click(object sender, EventArgs e)
        {
            gcList_Fabric.DataSource = null;

            ArrayList rows = ct.getList_isChecked(gridView3);

            if (rows.Count > 0)
            {
                try
                {
                    // Create DataTable
                    DataTable dt = new DataTable();

                    // Add Columns Header into DataTable
                    dt.Columns.Add("No", typeof(string));
                    dt.Columns.Add("VendFBCode", typeof(string));
                    dt.Columns.Add("Composition", typeof(string));
                    dt.Columns.Add("FBWeight", typeof(string));
                    dt.Columns.Add("ColorName", typeof(string));
                    dt.Columns.Add("SMPLotNo", typeof(string));
                    dt.Columns.Add("Supplier", typeof(string));
                    dt.Columns.Add("NAVCode", typeof(string));

                    int listfbNo = 1;
                    string sqlcmd = string.Empty;
                    for (int i = 0; i < rows.Count; i++)
                    {
                        DataRow row = rows[i] as DataRow;
                        string PatternNo = row["SMPLPatternNo"].ToString();
                        string Color = row["ColorName"].ToString();
                        string Size = row["SizeName"].ToString();
                        string Quantity = row["Quantity"].ToString();

                        /*Set size*/
                        sqlcmd += "Select '" + Size + "' as SizeName Union ";

                        /* Add to List Fabric*/
                        dt.Rows.Add(new object[] {
                            listfbNo++
                            ,txtVendorFBCode_FB.Text.Trim().ToString().Replace("'","''")
                            ,txtComposition_FB.Text.Trim().ToString().Replace("'","''")
                            ,txtWeightFB_FB.Text.Trim().ToString().Replace("'","''")
                            ,slFBColor_FB.Text.ToString()
                            ,txtSampleLotNo_FB.Text.Trim().ToString().Replace("'","''")
                            ,slVendor_FB.Text.ToString()
                            ,slFBCode_FB.Text.ToString()
                        });
                    }
                    gcList_Fabric.DataSource = dt;

                    int length = sqlcmd.Length;
                    sqlcmd = sqlcmd.Substring(0, length - 6);
                    Console.WriteLine(sqlcmd);
                    db.getDgv(sqlcmd, gcSize_Fabric, mainConn);
                }
                catch { }
            }
            else
            {
                gcSize_Fabric.DataSource = null;
                gcList_Fabric.DataSource = null;
            }
        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            ct.openFile_Image(xtraOpenFileDialog1,txtImgUpload_FB,picUpload_FB);
        }

        private void gridView6_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            ArrayList row = ct.getList_isChecked(gridView6);
            if (row.Count > 0)
            {
                // Check Form Condition
                // Not Null
                string OIDDEPT = glWorkStation_Mat.Text.ToString();
                string OIDVEND = slVendor_Mat.Text.ToString();

                //check not null
                if (OIDDEPT == "") {        ct.showWarningMessage("Please Select : Work Station"); glWorkStation_Mat.Focus(); gridView6.ClearSelection(); return; }
                else if (OIDVEND == "") {   ct.showWarningMessage("Please Select : Vendor"); slVendor_Mat.Focus(); gridView6.ClearSelection(); return; }
                else
                {
                    //MTColor = ct.setNull_Sl(MTColor,slMatColor_Mat);
                    string MTColor = (slMatColor_Mat.Text.ToString() == "") ? "" : slMatColor_Mat.Text.ToString();

                    try
                    {
                        // Create DataTable
                        DataTable dt = new DataTable();

                        // Add Columns Header into DataTable
                        dt.Columns.Add("No", typeof(string));
                        dt.Columns.Add("Color", typeof(string));
                        dt.Columns.Add("Size", typeof(string));
                        dt.Columns.Add("Consumption", typeof(string));
                        dt.Columns.Add("Unit", typeof(string));
                        dt.Columns.Add("OIDSMPL", typeof(string));

                        int listMNo = 1;

                        for (int i = 0; i < row.Count; i++)
                        {
                            DataRow r = row[i] as DataRow;

                            /* Add to List Mat*/
                            dt.Rows.Add(new object[] {
                                listMNo++
                                ,MTColor
                                ,r["OIDSIZE"].ToString()
                                ,r["Consumption"].ToString()
                                ,r["OIDUNIT"].ToString()
                                ,dosetOIDSMPL
                            });
                        }

                        gridControl7.DataSource = dt;
                    }
                    catch { }
                }
            }
            else
            {
                gridControl7.DataSource = null;
            }
        }

        private void slVendor_Mat_EditValueChanged(object sender, EventArgs e)
        {
            GridView gv = gridView6;
            ArrayList row = ct.getList_isChecked(gv);

            if (slVendor_Mat.Text.ToString() != "")
            {
                /*lVendor_Mat.EditValue = slVendor_Mat.EditValue.ToString();*/
                txtVendName_Mat.Text = db.get_oneParameter("Select top 1 Name From Vendor Where Code = '"+ slVendor_Mat.Text.ToString() + "'", mainConn, "Name");//slVendor_Mat.Text.ToString();
            }
            else
            { 
                txtVendName_Mat.Text = "";
                //UnSelect All
                if (row.Count > 0)
                {
                    for (int i = 0; i < row.Count; i++)
                    {
                        gv.UnselectRow(i);
                    }
                }
            }
        }

        private void btnUploadMat_Click(object sender, EventArgs e)
        {
            ct.openFile_Image(xtraOpenFileDialog1,txtPathFile_Mat,picMat);
        }

        private void btnGettoLlist_Mat_Click(object sender, EventArgs e)
        {
            /*
            1. check ว่ามีการเลือก chkbox หรือยัง
            2. ถ้าเลือกแล้วค่อยทำงาน โดยการดึงข้อมูลจาก ChkBox มาแสดงในรายการตางรางที่ 2
            */

            ArrayList row = ct.getList_isChecked(gridView6);
            if (row.Count > 0)
            {
                //string MTColor = slMatColor_Mat.Text.ToString();
                string MTColor = (slMatColor_Mat.Text.ToString() == "") ? "" : slMatColor_Mat.EditValue.ToString(); //slMatColor_Mat.EditValue.ToString();
                try
                {
                    // Create DataTable
                    DataTable dt = new DataTable();

                    // Add Columns Header into DataTable
                    dt.Columns.Add("No", typeof(string));
                    dt.Columns.Add("Color", typeof(string));
                    dt.Columns.Add("Size", typeof(string));
                    dt.Columns.Add("Consumption", typeof(string));
                    dt.Columns.Add("Unit", typeof(string));
                    dt.Columns.Add("OIDSMPL", typeof(string));

                    int listMNo = 1;
                    for (int i = 0; i < row.Count; i++)
                    {
                        DataRow r = row[i] as DataRow;

                        /* Add to List Mat*/
                        dt.Rows.Add(new object[] {
                                listMNo++
                                ,MTColor
                                ,r["OIDSIZE"].ToString()
                                ,r["Consumption"].ToString()
                                ,r["OIDUNIT"].ToString()
                                ,dosetOIDSMPL
                            });
                    }
                    gridControl7.DataSource = dt;
                }
                catch { }
            }
            else
            {
                ct.showWarningMessage("Please Select Checkbox List"); gridView6.Focus(); return;
            }
        }

        private void gridControl7_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            GridControl gridControl = (GridControl)sender;
            GridView currentView = (GridView)gridControl.FocusedView;
            if (e.KeyCode == Keys.Delete) { 
                currentView.DeleteRow(currentView.FocusedRowHandle);
            }
        }

        private void gridView8_DoubleClick(object sender, EventArgs e)
        {
            if (gridView8.RowCount > 0)
            {
                status_Mat = "update";
                bbiSave.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;   //hide
                bbiEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;  //show
                gridControl6.Enabled        = false;
                btnGettoLlist_Mat.Enabled   = false;
                slMatColor_Mat.Enabled      = false;

                // Set Update FormDetails
                var s = sender;
                string SampleID = ct.getCellVal(s, "SampleID");
                string MatID    = ct.getCellVal(s, "MatID");
                string sql = "Select ROW_NUMBER() Over(Order by OIDSMPLMT) as No, c.OIDCOLOR as Color,s.OIDSIZE as Size, Consumption as Consumption, u.OIDUNIT as Unit , q.OIDSMPL as OIDSMPL,";
                sql += " m.OIDSMPLMT as MatID,q.OIDSMPL as SampleID,d.Name as WorkStation,VendMTCode,SMPLotNo,v.Name as Vendor,c.ColorName as MatColor,s.SizeName as MatSize,m.Composition,Details,Price,cr.Currency as Currency,i.Code as NAVCode,m.Situation,Comment,Remark,m.PathFile,Consumption/*,m.OIDUNIT *//*Special OIDValue*/,d.OIDDEPT,v.OIDVEND,c.OIDCOLOR,i.OIDITEM,cr.OIDCURR From SMPLRequestMaterial m inner join SMPLQuantityRequired q on q.OIDSMPLDT = m.OIDSMPLDT inner join Departments d on d.OIDDEPT = m.OIDDEPT inner join Vendor v on v.OIDVEND = m.OIDVEND left join ProductColor c on c.OIDCOLOR = m.MTColor inner join ProductSize s on s.OIDSIZE = m.MTSize left join Currency cr on cr.OIDCURR = m.OIDCURR left join Items i on i.OIDITEM = m.OIDITEM inner join Unit u on u.OIDUNIT = m.OIDUNIT Where q.OIDSMPL = " + SampleID + " And m.OIDSMPLMT = "+ MatID + " ";

                txtSampleID_Mat.EditValue       = SampleID;
                txtMatRecordID_Mat.EditValue    = MatID;
                glWorkStation_Mat.EditValue     = db.get_oneParameter(sql,mainConn, "OIDDEPT");
                slVendor_Mat.EditValue          = db.get_oneParameter(sql, mainConn, "OIDVEND");
                slMatColor_Mat.EditValue        = db.get_oneParameter(sql,mainConn, "OIDCOLOR");
                slMatCode_Mat.EditValue         = db.get_oneParameter(sql,mainConn, "OIDITEM");
                glCurrency_Mat.EditValue        = db.get_oneParameter(sql,mainConn, "OIDCURR");

                txtVendorMatCode_Mat.EditValue  = ct.getCellVal(s, "VendMTCode");
                txtSampleLotNo_Mat.EditValue    = ct.getCellVal(s, "SMPLotNo");
                txtMatComposition_Mat.EditValue = ct.getCellVal(s, "Composition");
                txtPrice_Mat.EditValue          = ct.getCellVal(s, "Price");
                txtSituation_Mat.EditValue      = ct.getCellVal(s, "Situation");
                txtComment_Mat.EditValue        = ct.getCellVal(s, "Comment");
                txtRemark_Mat.EditValue         = ct.getCellVal(s, "Remark");
                txtPathFile_Mat.EditValue       = (ct.getCellVal(s, "PathFile") == "") ? "" : picPart+ct.getCellVal(s, "PathFile");
                //picMat.Image                    = (ct.getCellVal(s, "PathFile") == "") ? null : Image.FromFile(picPart + ct.getCellVal(s, "PathFile"));
                
                if (ct.getCellVal(s, "PathFile") != "")
                {
                    try
                    {
                        picMat.Image = Image.FromFile(picPart + ct.getCellVal(s, "PathFile"));
                    }
                    catch { ct.showInfoMessage("Can't Find Image File Destination!"); }
                }
                else
                {
                    picMat.Image = null;
                }

                // Set Grid
                db.getDgv(sql,gridControl7,mainConn);
            }
        }

        private void gridView4_DoubleClick(object sender, EventArgs e)
        {
            if (gridView4.RowCount > 0)
            {
                bbiEdit.Enabled     = true;
                bbiSave.Visibility  = DevExpress.XtraBars.BarItemVisibility.Never;
                bbiEdit.Visibility  = DevExpress.XtraBars.BarItemVisibility.Always;

                var         s = sender;
                string FBID = ct.getCellVal(s, "No");

                string sql = "Select fb.OIDSMPLFB as No,fb.OIDVEND,fb.OIDCOLOR as FBColor,q.OIDCOLOR as FGColor,fb.OIDCURR,fb.OIDITEM"; 
                sql += " ,fb.PathFile,Price,TotalWidth,UsableWidth,WidthCuttable , VendFBCode,fb.Composition,FBWeight,c.ColorName as ColorName,SMPLotNo,v.Name as Supplier,i.Code as NAVCode From SMPLRequestFabric fb inner join SMPLQuantityRequired q on q.OIDSMPLDT = fb.OIDSMPLDT inner join SMPLRequest smpl on smpl.OIDSMPL = q.OIDSMPL inner join ProductColor c on c.OIDCOLOR = fb.OIDCOLOR inner join Items i on i.OIDITEM = fb.OIDITEM inner join Vendor v on v.OIDVEND = fb.OIDVEND Where smpl.OIDSMPL = " + dosetOIDSMPL+" And fb.OIDSMPLFB = "+FBID+" ";

                Console.WriteLine(sql);

                // Set Update Fabric
                txtFabricRacordID_FB.EditValue = FBID;
                txtVendorFBCode_FB.EditValue = ct.getCellVal(s, "VendFBCode");
                txtSampleLotNo_FB.EditValue = ct.getCellVal(s, "SMPLotNo");
                slVendor_FB.EditValue = db.get_oneParameter(sql, mainConn, "OIDVEND");
                slFBColor_FB.EditValue = db.get_oneParameter(sql, mainConn, "FBColor");
                slFBCode_FB.EditValue = db.get_oneParameter(sql, mainConn, "OIDITEM");
                slFGColor_FB.EditValue = db.get_oneParameter(sql, mainConn, "FGColor");
                glCurrency_FB.EditValue = db.get_oneParameter(sql, mainConn, "OIDCURR");

                txtComposition_FB.EditValue = ct.getCellVal(s, "Composition");
                txtWeightFB_FB.EditValue = ct.getCellVal(s, "FBWeight");
                txtWidthCuttable_FB.EditValue = db.get_oneParameter(sql,mainConn, "WidthCuttable");
                txtPrice_FB.EditValue = db.get_oneParameter(sql, mainConn, "Price");
                txtTotalWidth_FB.EditValue = db.get_oneParameter(sql, mainConn, "TotalWidth");
                txtUsableWidth_FB.EditValue = db.get_oneParameter(sql, mainConn, "UsableWidth");

                string imgName = db.get_oneParameter(sql, mainConn, "PathFile");
                txtImgUpload_FB.EditValue = (imgName == "") ? "" : picPart + imgName;
                if (imgName != "")
                {
                    try
                    {
                        picUpload_FB.Image = Image.FromFile(picPart + imgName);
                    }
                    catch
                    {
                        ct.showInfoMessage("Can't Find Image File Destination!");
                    }
                }
                else
                {
                    picUpload_FB.Image = null;
                }

                // Clear checkBox
                db.getDgv("Select OIDGParts,GarmentParts From GarmentParts", gcPart_Fabric, mainConn);

                //>> Pull dbTable to dataTable
                var dt = new DataTable();
                using (var da = new SqlDataAdapter("Select OIDGParts From SMPLRequestFabricParts Where OIDSMPLFB = " + FBID + " ", mainConn))
                {
                    da.Fill(dt);
                }

                // Loop GPart
                GridView gv = gridView11;
                for (int i = 0; i < gv.RowCount; i++)
                {
                    //>> Read Data in Datatable
                    foreach (DataRow row in dt.Rows)
                    {
                        string OIDGParts1 = gv.GetRowCellValue(i, "OIDGParts").ToString();
                        string OIDGParts2 = row["OIDGParts"].ToString();
                        if (OIDGParts1 == OIDGParts2)
                        {
                            gv.SelectRow(i);
                        }
                    }
                }
            }
        }

        public void refreshFabric()
        {
            bbiSave.Enabled         = false;
            bbiEdit.Enabled         = false;
            gridControl3.Enabled    = false;
            btngetListFB_FB.Enabled = false;
            //bbiSave.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            //bbiEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

            db.getGrid_FBListSample(gridControl3, " And smplQR.OIDSMPL = " + dosetOIDSMPL + " ");
            db.getDgv("Select OIDGParts,GarmentParts From GarmentParts", gcPart_Fabric, mainConn);
            txtSampleID_FB.Text = dosetOIDSMPL;
            txtFabricRacordID_FB.Text = db.get_oneParameter("Select case When ISNULL( MAX(OIDSMPLFB),'') = '' Then 1 Else MAX(OIDSMPLFB) End as maxFB From SMPLRequestFabric", mainConn, "maxFB");

            //get List of Fabric
            db.getListofFabric(gcList_Fabric, dosetOIDSMPL);

            //Set New OIDFB
            txtFabricRacordID_FB.EditValue = null;//db.get_newOIDFB();

            //Clear Form
            txtVendorFBCode_FB.EditValue = null;
            txtSampleLotNo_FB.EditValue = null;
            slVendor_FB.EditValue = null;
            slFBColor_FB.EditValue = null;
            slFBCode_FB.EditValue = null;
            slFGColor_FB.EditValue = null;
            txtComposition_FB.EditValue = null;
            txtWeightFB_FB.EditValue = null;
            txtWidthCuttable_FB.EditValue = null;
            txtPrice_FB.EditValue = null;
            glCurrency_FB.EditValue = null;
            txtTotalWidth_FB.EditValue = null;
            txtUsableWidth_FB.EditValue = null;
            txtImgUpload_FB.EditValue = null;
            picUpload_FB.Image = null;
        }

        private void bbiRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            switch (currenTab)
            {
                /* List of , Main , Fabric , Material */
                //case "Main": refreshMain(); break;
                case "Fabric": refreshFabric(); break;
                //case "Material": refreshMaterials(); break;
                default: break;
            }
        }

        private void gridView1_PrintInitialize(object sender, DevExpress.XtraGrid.Views.Base.PrintInitializeEventArgs e)
        {
            PrintingSystemBase pb = e.PrintingSystem as PrintingSystemBase;
            pb.PageSettings.Landscape = true;
        }

        private void gridView8_PrintInitialize(object sender, DevExpress.XtraGrid.Views.Base.PrintInitializeEventArgs e)
        {
            PrintingSystemBase pb = e.PrintingSystem as PrintingSystemBase;
            pb.PageSettings.Landscape = true;
        }
    }
}