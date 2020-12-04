using System;
using System.Text;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Extensions;
using DBConnection;
using MDS00;
using System.Data;
using DevExpress.XtraEditors.Controls;
using System.CodeDom;
using DevExpress.XtraGrid.Views.Grid;
using System.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.XtraEditors;

namespace M07
{
    public partial class M07 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private Functionality.Function FUNC = new Functionality.Function();
        private const string imgPathFile = @"\\172.16.0.190\MDS_Project\MDS\Pictures\";
        private DataTable dtVendor = new DataTable();
        private string selCode = "";
        public M07()
        {
            InitializeComponent();
            UserLookAndFeel.Default.StyleChanged += MyStyleChanged;
            iniConfig = new IniFile("Config.ini");
            UserLookAndFeel.Default.SetSkinStyle(iniConfig.Read("SkinName", "DevExpress"), iniConfig.Read("SkinPalette", "DevExpress"));
            CreateSplashScreen();
        }

        private IniFile iniConfig;

        private void CreateSplashScreen()
        {
            DevExpress.XtraSplashScreen.SplashScreenManager.ShowSkinSplashScreen(
                logoImage: null,
                title: "MDS",
                subtitle: "Merchandise and Development System",
                footer: "Copyright © 2020-2021 IT Integration Team",
                loading: "Starting...",
                parentForm: this,
                useFadeIn: true,
                useFadeOut: true,
                throwExceptionIfAlreadyOpened: true,
                startPos: DevExpress.XtraSplashScreen.SplashFormStartPosition.Default,
                location: default
                );
        }
        private void CloseSplashScreen()
        {
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
        }

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
            tabbedControlGroup1.SelectedTabPage = layoutControlGroup1; //เลือกแท็บ Main
            rgMaterial.SelectedIndex = 0;

            glueCode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            glueCode.Properties.AcceptEditorTextAsNewValue = DevExpress.Utils.DefaultBoolean.True;

            NewData();
            LoadData();
            if (rgMaterial.SelectedIndex > -1)
            {
                LoadDataMeterial();
            }
            CloseSplashScreen();
        }

        private void NewData()
        {
            //rgMaterial.SelectedIndex = 0;
            txeID.Text = "";
            if (rgMaterial.SelectedIndex > -1)
            { 
                string Material = rgMaterial.Properties.Items[rgMaterial.SelectedIndex].Value.ToString();
                txeID.Text = new DBQuery("SELECT CASE WHEN ISNULL(MAX(OIDITEM), '') = '' THEN 1 ELSE MAX(OIDITEM) + 1 END AS NewNo FROM Items WHERE (MaterialType = '" + Material + "') ").getString();
            }
            txeID.Text = "";
            glueCode.EditValue = "";
            txeDescription.Text = "";
            txeComposition.Text = "";
            txeWeight.Text = "";
            txeModelNo.Text = "";
            txeModelName.Text = "";
            slueCategory.EditValue = "";
            slueStyle.EditValue = "";
            slueColor.EditValue = "";
            slueSize.EditValue = "";
            slueCustomer.EditValue = "";
            txeBusinessUnit.Text = "";
            cbeSeason.EditValue = "";
            cbeClass.EditValue = "";
            glueBranch.EditValue = "";
            txeCostSheet.Text = "";
            txeStdPrice.Text = "";

            slueFirstVendor.EditValue = "";
            txeMatDetails.Text = "";
            txeMatCode.Text = "";
            txeSMPLLotNo.Text = "";
            txePrice.Text = "";
            txeCurrency.Text = "";
            rgPurchase.SelectedIndex = 0;
            rgTax.SelectedIndex = -1;
            txePurchaseLoss.Text = "";
            dteFirstReceiptDate.EditValue = DateTime.Now;
            slueDefaultVendor.EditValue = "";

            txeSMPLNo.Text = "";
            dteRequestDate.EditValue = DateTime.Now;
            txeSMPLItem.Text = "";
            txeSMPLPatternNo.Text = "";
            rgZone.SelectedIndex = 0;

            txeMinStock.Text = "";
            txeMaxStock.Text = "";
            txeStockSheifLife.Text = "";
            txeStdCost.Text = "";
            slueDefaultUnit.EditValue = "";
            glueUnit.EditValue = "";

            picImg.EditValue = "";
            txePath.Text = "";

            txeLabTestNo.Text = "";
            dteApprovedLabDate.EditValue = DateTime.Now;
            txeQCInspection.Text = "";
            clbQC.Items.Clear();

            slueVendorCode.EditValue = "";
            txeVendorName.Text = "";
            txeLotSize.Text = "";
            txeProductionLead.Text = "";
            txeDeliveryLead.Text = "";
            txeArrivalLead.Text = "";
            txePOCancelPeriod.Text = "";

            txeLots1.Text = "";
            txeLots2.Text = "";
            txeLots3.Text = "";

            txeRemark.Text = "";
            lblIDVENDItem.Text = "";

            txeCREATE.Text = "0";
            txeCDATE.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            txeUPDATE.Text = "0";
            txeUDATE.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            //**************************************
            slueFirstVendor.ReadOnly = false;
            txeMatDetails.ReadOnly = false;
            txeMatCode.ReadOnly = false;
            txeSMPLLotNo.ReadOnly = false;
            txePrice.ReadOnly = false;
            txeCurrency.ReadOnly = false;
            rgPurchase.ReadOnly = false;
            rgTax.ReadOnly = false;
            txePurchaseLoss.ReadOnly = false;
            dteFirstReceiptDate.ReadOnly = false;
            //**************************************
            selCode = "";

            string Materialx = rgMaterial.Properties.Items[rgMaterial.SelectedIndex].Value.ToString();
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("SELECT OIDCONDQC, ConditionName ");
            sbSQL.Append("FROM ConditionQC ");
            sbSQL.Append("WHERE (ItemType = '" + Materialx + "') ");
            sbSQL.Append("ORDER BY ConditionName ");
            DataTable drMaterial = new DBQuery(sbSQL).getDataTable();
            clbQC.ValueMember = "OIDCONDQC";
            clbQC.DisplayMember = "ConditionName";
            clbQC.DataSource = drMaterial;

            dtVendor.Rows.Clear();
            tabbedControlGroup1.SelectedTabPage = layoutControlGroup1; //เลือกแท็บ Main
        }

        private void LoadDataMeterial()
        {
            string Material = rgMaterial.Properties.Items[rgMaterial.SelectedIndex].Value.ToString();

            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("SELECT Code, Description ");
            sbSQL.Append("FROM  Items ");
            sbSQL.Append("WHERE (MaterialType = '" + Material + "')");
            sbSQL.Append("ORDER BY Code ");
            new ObjDevEx.setGridLookUpEdit(glueCode, sbSQL, "Code", "Code").getData(true);

            sbSQL.Clear();
            sbSQL.Append("SELECT OIDCONDQC, ConditionName ");
            sbSQL.Append("FROM ConditionQC ");
            sbSQL.Append("WHERE (ItemType = '" + Material + "') ");
            sbSQL.Append("ORDER BY ConditionName ");
            DataTable drMaterial = new DBQuery(sbSQL).getDataTable();
            clbQC.ValueMember = "OIDCONDQC";
            clbQC.DisplayMember = "ConditionName";
            clbQC.DataSource = drMaterial;

            sbSQL.Clear();
            sbSQL.Append("SELECT OIDVENDItem, OIDVEND, Code AS Vendor, Name AS VendorName, OIDITEM, LotSize, ProductionLead, DeliveryLead, ArrivalLead, POCancelPeriod, PurchaseLots1, PurchaseLots2, PurchaseLots3, Remark ");
            sbSQL.Append("FROM ItemVendor AS IVD ");
            sbSQL.Append("     CROSS APPLY(SELECT Code, Name FROM Vendor WHERE OIDVEND = IVD.OIDVEND) AS VD ");
            sbSQL.Append("WHERE (OIDITEM = '" + txeID.Text.Trim() + "') ");
            sbSQL.Append("ORDER BY OIDVENDItem ");
            new ObjDevEx.setGridControl(gcVendor, gvVendor, sbSQL).getData(false, false, false, true);
            dtVendor = new DBQuery(sbSQL).getDataTable();
            if (gvVendor.Columns.Count > 0)
            {
                //เปลี่ยนชื่อ Column ใน DataTable ให้ตรงกับ DataGridView
                for (int ii = 0; ii < gvVendor.Columns.Count; ii++)
                {
                    try
                    {
                        dtVendor.Columns[ii].ColumnName = gvVendor.Columns[ii].FieldName;
                        dtVendor.Columns[ii].AllowDBNull = true;
                    }
                    catch (Exception) { }
                    ii++;
                }
            }
            gvVendor.Columns[0].Visible = false;
            gvVendor.Columns[1].Visible = false;
            gvVendor.Columns[4].Visible = false;
        }

        private void LoadData()
        {

            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("SELECT OIDGCATEGORY AS ID, CategoryName ");
            sbSQL.Append("FROM  GarmentCategory ");
            sbSQL.Append("ORDER BY CategoryName ");
            new ObjDevEx.setSearchLookUpEdit(slueCategory, sbSQL, "CategoryName", "ID").getData(true);

            sbSQL.Clear();
            sbSQL.Append("SELECT OIDCOLOR AS ID, ColorNo, ColorName ");
            sbSQL.Append("FROM  ProductColor ");
            sbSQL.Append("ORDER BY ColorNo ");
            new ObjDevEx.setSearchLookUpEdit(slueColor, sbSQL, "ColorName", "ID").getData(true);

            sbSQL.Clear();
            sbSQL.Append("SELECT OIDSIZE AS ID, SizeNo, SizeName ");
            sbSQL.Append("FROM  ProductSize ");
            sbSQL.Append("ORDER BY SizeNo ");
            new ObjDevEx.setSearchLookUpEdit(slueSize, sbSQL, "SizeName", "ID").getData(true);

            sbSQL.Clear();
            sbSQL.Append("SELECT OIDCUST AS ID, Code, Name, ShortName ");
            sbSQL.Append("FROM  Customer ");
            sbSQL.Append("ORDER BY Name ");
            new ObjDevEx.setSearchLookUpEdit(slueCustomer, sbSQL, "ShortName", "ID").getData(true);

            sbSQL.Clear();
            sbSQL.Append("SELECT DISTINCT Season ");
            sbSQL.Append("FROM  Items ");
            sbSQL.Append("ORDER BY Season ");
            new ObjDevEx.setComboboxEdit(cbeSeason, sbSQL).getDataRange();

            sbSQL.Clear();
            sbSQL.Append("SELECT DISTINCT ClassType ");
            sbSQL.Append("FROM  Items ");
            sbSQL.Append("ORDER BY ClassType ");
            new ObjDevEx.setComboboxEdit(cbeClass, sbSQL).getDataRange();

            sbSQL.Clear();
            sbSQL.Append("SELECT OIDBranch AS ID, Name AS Branch ");
            sbSQL.Append("FROM  Branchs ");
            sbSQL.Append("ORDER BY OIDBranch ");
            new ObjDevEx.setGridLookUpEdit(glueBranch, sbSQL, "Branch", "ID").getData(true);

            sbSQL.Clear();
            sbSQL.Append("SELECT OIDUNIT AS ID, UnitName ");
            sbSQL.Append("FROM  Unit ");
            sbSQL.Append("ORDER BY UnitName ");
            new ObjDevEx.setSearchLookUpEdit(slueDefaultUnit, sbSQL, "UnitName", "ID").getData(true);

            sbSQL.Clear();
            sbSQL.Append("SELECT OIDVEND AS ID, Code, Name ");
            sbSQL.Append("FROM  Vendor ");
            sbSQL.Append("ORDER BY Name ");
            new ObjDevEx.setSearchLookUpEdit(slueFirstVendor, sbSQL, "Name", "ID").getData(true);
            new ObjDevEx.setSearchLookUpEdit(slueDefaultVendor, sbSQL, "Name", "ID").getData(true);
            new ObjDevEx.setSearchLookUpEdit(slueVendorCode, sbSQL, "Code", "ID").getData(true);

            
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NewData();
            LoadData();
            if (rgMaterial.SelectedIndex > -1)
            {
                LoadDataMeterial();
            }
        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (glueCode.Text.Trim() == "")
            {
                FUNC.msgWarning("Please input items code.");
                glueCode.Focus();
            }
            else if (txeDescription.Text.Trim() == "")
            {
                FUNC.msgWarning("Please input description.");
                txeDescription.Focus();
            }
            else if (glueBranch.Text.Trim() == "")
            {
                FUNC.msgWarning("Please input branch.");
                glueBranch.Focus();
            }
            else if (txeStdPrice.Text.Trim() == "")
            {
                FUNC.msgWarning("Please input standard price.");
                txeStdPrice.Focus();
            }
            else if (slueFirstVendor.Text.Trim() == "")
            {
                FUNC.msgWarning("Please select supplier (start raw mat.).");
                slueFirstVendor.Focus();
            }
            else if (slueDefaultVendor.Text.Trim() == "")
            {
                FUNC.msgWarning("Please select default supplier.");
                slueDefaultVendor.Focus();
            }
            else if (txeQCInspection.Text.Trim() == "")
            {
                FUNC.msgWarning("Please input quality inspection %.");
                txeQCInspection.Focus();
            }
            else
            {
                if (FUNC.msgQuiz("Confirm save ?") == true)
                { 
                    bool chkPASS = true;
                    //Check Vendor
                    if (slueFirstVendor.Text.Trim() != "")
                    {
                        bool chkVendor = false;
                        foreach (DataRow dr in dtVendor.Rows) // search whole table
                        {
                            if (dr["OIDVEND"].ToString() == slueFirstVendor.EditValue.ToString())
                            {
                                chkVendor = true;
                                break;
                            }
                        }

                        if (chkVendor == false)
                        {
                            FUNC.msgWarning("Please add vendor:'" + slueFirstVendor.Text.Trim() + "' to vendor details.");
                            tabbedControlGroup1.SelectedTabPage = layoutControlGroup2;
                            slueVendorCode.EditValue = slueFirstVendor.EditValue;
                            slueVendorCode.Focus();
                            chkPASS = false;
                        }
                        else
                        {
                            if (slueFirstVendor.EditValue.ToString() != slueDefaultVendor.EditValue.ToString())
                            {
                                chkVendor = false;
                                foreach (DataRow dr in dtVendor.Rows) // search whole table
                                {
                                    if (dr["OIDVEND"].ToString() == slueDefaultVendor.EditValue.ToString())
                                    {
                                        chkVendor = true;
                                        break;
                                    }
                                }

                                if (chkVendor == false)
                                {
                                    FUNC.msgWarning("Please add vendor:'" + slueDefaultVendor.Text.Trim() + "' to vendor details.");
                                    tabbedControlGroup1.SelectedTabPage = layoutControlGroup2;
                                    slueVendorCode.EditValue = slueDefaultVendor.EditValue;
                                    slueVendorCode.Focus();
                                    chkPASS = true;
                                }
                            }
                        }
                    }

                    if (chkPASS == true)
                    {
                        StringBuilder sbSQL = new StringBuilder();

                        string MaterialType = rgMaterial.Properties.Items[rgMaterial.SelectedIndex].Value.ToString();
                        string PurchaseType = rgPurchase.Properties.Items[rgPurchase.SelectedIndex].Value.ToString();
                        string TaxBenefits = "0";
                        if (rgTax.SelectedIndex != -1)
                        {
                            TaxBenefits = rgTax.Properties.Items[rgTax.SelectedIndex].Value.ToString();
                        }
                        string Zone = rgZone.Properties.Items[rgZone.SelectedIndex].Value.ToString();

                        string newFileName = "";
                        //CopyFile
                        if (txePath.Text.Trim() != "")
                        {
                            System.IO.FileInfo fi = new System.IO.FileInfo(txePath.Text);
                            string extn = fi.Extension;
                            newFileName = glueCode.Text.ToUpper().Trim() + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extn;
                            string newPathFileName = imgPathFile + newFileName;
                            //MessageBox.Show(newFileName);
                            System.IO.File.Copy(txePath.Text, newPathFileName);
                        }

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

                        //******** save Items table ************
                        sbSQL.Append("IF NOT EXISTS(SELECT OIDITEM FROM Items WHERE MaterialType = '" + MaterialType + "' AND Code = N'" + glueCode.Text.Trim().Replace("'", "''") + "') ");
                        sbSQL.Append(" BEGIN ");
                        sbSQL.Append("  INSERT INTO Items(MaterialType, Code, Description, Composition, WeightOrMoreDetail, ModelNo, ModelName, OIDCATEGORY, OIDSTYLE, OIDCOLOR, OIDSIZE, OIDCUST, BusinessUnit, Season, ClassType, Branch,  ");
                        sbSQL.Append("       CostSheetNo, StdPrice, FirstVendor, PurchaseType, PurchaseLoss, TaxBenefits, FirstReceiptDate, DefaultVendor, MinStock, MaxStock, StockShelfLife, StdCost, DefaultUnit, PathFile, LabTestNo, ApprovedLabDate, QCInspection, ");
                        sbSQL.Append("       CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) ");
                        sbSQL.Append("  VALUES('" + MaterialType + "', N'" + glueCode.Text.Trim().Replace("'", "''") + "', N'" + txeDescription.Text.Trim().Replace("'", "''") + "', N'" + txeComposition.Text.Trim().Replace("'", "''") + "', N'" + txeWeight.Text.Trim().Replace("'", "''") + "', N'" + txeModelNo.Text.Trim().Replace("'", "''") + "', N'" + txeModelName.Text.Trim().Replace("'", "''") + "', '" + slueCategory.EditValue.ToString() + "', '" + slueStyle.EditValue.ToString() + "', '" + slueColor.EditValue.ToString() + "', '" + slueSize.EditValue.ToString() + "', '" + slueCustomer.EditValue.ToString() + "',  ");
                        sbSQL.Append("         N'" + txeBusinessUnit.Text.Trim().Replace("'", "''") + "', N'" + cbeSeason.Text.Trim() + "', N'" + cbeClass.Text.Trim().Replace("'", "''") + "', '" + glueBranch.EditValue.ToString() + "', N'" + txeCostSheet.Text.Trim().Replace("'", "''") + "', '" + txeStdPrice.Text.Trim() + "', '" + slueFirstVendor.EditValue.ToString() + "', '" + PurchaseType + "', '" + txePurchaseLoss.Text.Trim() + "', '" + TaxBenefits + "', '" + Convert.ToDateTime(dteFirstReceiptDate.Text).ToString("yyyy-MM-dd") + "', '" + slueDefaultVendor.EditValue.ToString() + "', ");
                        sbSQL.Append("         '" + txeMinStock.Text.Trim() + "', '" + txeMaxStock.Text.Trim() + "', '" + txeStockSheifLife.Text.Trim() + "', '" + txeStdCost.Text.Trim() + "', '" + slueDefaultUnit.EditValue.ToString() + "', N'" + newFileName + "', N'" + txeLabTestNo.Text.Trim().Replace("'", "''") + "', '" + Convert.ToDateTime(dteApprovedLabDate.Text).ToString("yyyy-MM-dd") + "', '" + txeQCInspection.Text.Trim() + "', ");
                        sbSQL.Append("         '" + strCREATE + "', GETDATE(), '" + strUPDATE + "', GETDATE()) ");
                        sbSQL.Append(" END ");
                        sbSQL.Append("ELSE ");
                        sbSQL.Append(" BEGIN ");
                        sbSQL.Append("  UPDATE Items SET ");
                        sbSQL.Append("      MaterialType = '" + MaterialType + "', Code = N'" + glueCode.Text.Trim().Replace("'", "''") + "', Description = N'" + txeDescription.Text.Trim().Replace("'", "''") + "', Composition = N'" + txeComposition.Text.Trim().Replace("'", "''") + "', WeightOrMoreDetail = N'" + txeWeight.Text.Trim().Replace("'", "''") + "',  ");
                        sbSQL.Append("      ModelNo = N'" + txeModelNo.Text.Trim().Replace("'", "''") + "', ModelName = N'" + txeModelName.Text.Trim().Replace("'", "''") + "', OIDCATEGORY = '" + slueCategory.EditValue.ToString() + "', OIDSTYLE = '" + slueStyle.EditValue.ToString() + "', OIDCOLOR = '" + slueColor.EditValue.ToString() + "', ");
                        sbSQL.Append("      OIDSIZE = '" + slueSize.EditValue.ToString() + "', OIDCUST = '" + slueCustomer.EditValue.ToString() + "', BusinessUnit = N'" + txeBusinessUnit.Text.Trim().Replace("'", "''") + "', Season = N'" + cbeSeason.Text.Trim() + "', ClassType = N'" + cbeClass.Text.Trim().Replace("'", "''") + "', ");
                        sbSQL.Append("      Branch = '" + glueBranch.EditValue.ToString() + "', CostSheetNo = N'" + txeCostSheet.Text.Trim().Replace("'", "''") + "', StdPrice = '" + txeStdPrice.Text.Trim() + "', FirstVendor = '" + slueFirstVendor.EditValue.ToString() + "', PurchaseType = '" + PurchaseType + "', PurchaseLoss = '" + txePurchaseLoss.Text.Trim() + "', ");
                        sbSQL.Append("      TaxBenefits = '" + TaxBenefits + "', FirstReceiptDate = '" + Convert.ToDateTime(dteFirstReceiptDate.Text).ToString("yyyy-MM-dd") + "', DefaultVendor = '" + slueDefaultVendor.EditValue.ToString() + "', MinStock = '" + txeMinStock.Text.Trim() + "', MaxStock = '" + txeMaxStock.Text.Trim() + "', ");
                        sbSQL.Append("      StockShelfLife = '" + txeStockSheifLife.Text.Trim() + "', StdCost = '" + txeStdCost.Text.Trim() + "', DefaultUnit = '" + slueDefaultUnit.EditValue.ToString() + "', PathFile = N'" + newFileName + "', LabTestNo = N'" + txeLabTestNo.Text.Trim().Replace("'", "''") + "', ");
                        sbSQL.Append("      ApprovedLabDate = '" + Convert.ToDateTime(dteApprovedLabDate.Text).ToString("yyyy-MM-dd") + "', QCInspection = '" + txeQCInspection.Text.Trim() + "', UpdatedBy = '" + strUPDATE + "', UpdatedDate = GETDATE() ");
                        sbSQL.Append("  WHERE(OIDITEM = '" + txeID.Text.Trim() + "') ");
                        sbSQL.Append(" END  ");

                        bool chkSAVE = false;
                        try
                        {
                            chkSAVE = new DBQuery(sbSQL).runSQL();
                        }
                        catch (Exception)
                        { }

                        if (chkSAVE == true)
                        {

                            sbSQL.Clear();
                            sbSQL.Append("SELECT OIDITEM FROM Items WHERE MaterialType = '" + MaterialType + "' AND Code = N'" + glueCode.Text.Trim().Replace("'", "''") + "'");
                            string OIDITEM = new DBQuery(sbSQL).getString();
                            //******** save ItemInspection table ********
                            sbSQL.Clear();
                            string strCONDQC = "";
                            int iCQC = 0;
                            foreach (DataRowView item in clbQC.CheckedItems)
                            {
                                if (iCQC != 0)
                                {
                                    strCONDQC += ", ";
                                }
                                strCONDQC += "'" + item["OIDCONDQC"].ToString() + "'";
                                sbSQL.Append("IF NOT EXISTS(SELECT OIDITEMINSP FROM ItemInspection WHERE OIDITEM = '" + OIDITEM + "' AND OIDCONDQC = '" + item["OIDCONDQC"].ToString() + "') ");
                                sbSQL.Append(" BEGIN ");
                                sbSQL.Append("  INSERT INTO ItemInspection(OIDITEM, OIDCONDQC, CreatedBy, CreatedDate) ");
                                sbSQL.Append("  VALUES('" + OIDITEM + "', '" + item["OIDCONDQC"].ToString() + "', '" + strCREATE + "', GETDATE()) ");
                                sbSQL.Append(" END ");
                                iCQC++;
                            }

                            if (strCONDQC == "")
                            {
                                sbSQL.Append("DELETE FROM ItemInspection WHERE (OIDITEM = '" + OIDITEM + "')  ");
                            }
                            else
                            {
                                sbSQL.Append("DELETE FROM ItemInspection WHERE (OIDITEM = '" + OIDITEM + "') AND (OIDCONDQC NOT IN (" + strCONDQC + "))  ");
                            }

                            //******** save ItemVendor table ************

                            if (dtVendor.Rows.Count > 0)
                            {
                                foreach (DataRow dr in dtVendor.Rows) // search whole table
                                {
                                    if (dr["OIDVENDItem"].ToString() == "") //Insert
                                    {
                                        sbSQL.Append("INSERT INTO ItemVendor(OIDVEND, OIDITEM, ");
                                        if (dr["OIDVEND"].ToString() == slueFirstVendor.EditValue.ToString())
                                        {
                                            sbSQL.Append("MatDetails, MatCode, SMPLLotNo, Price, Currency, ");
                                        }
                                        sbSQL.Append("  LotSize, ProductionLead, DeliveryLead, ArrivalLead, POCancelPeriod, PurchaseLots1, PurchaseLots2, PurchaseLots3, Remark) ");
                                        sbSQL.Append(" VALUES('" + dr["OIDVEND"].ToString() + "', '" + OIDITEM + "',  ");
                                        if (dr["OIDVEND"].ToString() == slueFirstVendor.EditValue.ToString())
                                        {
                                            sbSQL.Append("N'" + txeMatDetails.Text.Trim().Replace("'", "''") + "', N'" + txeMatCode.Text.Trim().Replace("'", "''") + "', N'" + txeSMPLLotNo.Text.Trim().Replace("'", "''") + "', '" + txePrice.Text.Trim() + "', N'" + txeCurrency.Text.Trim().Replace("'", "''") + "',  ");
                                        }
                                        sbSQL.Append("'" + dr["LotSize"].ToString() + "', '" + dr["ProductionLead"].ToString() + "', '" + dr["DeliveryLead"].ToString() + "', '" + dr["ArrivalLead"].ToString() + "', '" + dr["POCancelPeriod"].ToString() + "', '" + dr["PurchaseLots1"].ToString() + "', '" + dr["PurchaseLots2"].ToString() + "', '" + dr["PurchaseLots3"].ToString() + "', N'" + dr["Remark"].ToString().Replace("'", "''") + "')  ");
                                    }
                                    else //Update
                                    {
                                        sbSQL.Append("UPDATE ItemVendor SET ");
                                        sbSQL.Append("  OIDVEND = '" + dr["OIDVEND"].ToString() + "', OIDITEM = '" + OIDITEM + "', ");
                                        if (dr["OIDVEND"].ToString() == slueFirstVendor.EditValue.ToString())
                                        {
                                            sbSQL.Append("MatDetails = N'" + txeMatDetails.Text.Trim().Replace("'", "''") + "', MatCode = N'" + txeMatCode.Text.Trim().Replace("'", "''") + "', SMPLLotNo = N'" + txeSMPLLotNo.Text.Trim().Replace("'", "''") + "', Price = '" + txePrice.Text.Trim() + "', Currency = N'" + txeCurrency.Text.Trim().Replace("'", "''") + "',  ");
                                        }
                                        sbSQL.Append("LotSize = '" + dr["LotSize"].ToString() + "', ProductionLead = '" + dr["ProductionLead"].ToString() + "', DeliveryLead = '" + dr["DeliveryLead"].ToString() + "', ArrivalLead = '" + dr["ArrivalLead"].ToString() + "', POCancelPeriod = '" + dr["POCancelPeriod"].ToString() + "', PurchaseLots1 = '" + dr["PurchaseLots1"].ToString() + "', PurchaseLots2 = '" + dr["PurchaseLots2"].ToString() + "', PurchaseLots3 = '" + dr["PurchaseLots3"].ToString() + "', Remark = N'" + dr["Remark"].ToString().Replace("'", "''") + "'  ");
                                        sbSQL.Append("WHERE (OIDVENDItem = '" + dr["OIDVENDItem"].ToString() + "') ");
                                    }
                                }
                            }

                            if (sbSQL.Length > 0)
                            {
                                try
                                {
                                    chkSAVE = new DBQuery(sbSQL).runSQL();
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
        }

        private void selectMaterial(int value)
        {
            switch (value)
            {
                case 0:
                    rgMaterial.SelectedIndex = 0;
                    break;
                case 1:
                    rgMaterial.SelectedIndex = 1;
                    break;
                case 2:
                    rgMaterial.SelectedIndex = 2;
                    break;
                case 3:
                    rgMaterial.SelectedIndex = 3;
                    break;
                case 4:
                    rgMaterial.SelectedIndex = 4;
                    break;
                case 9:
                    rgMaterial.SelectedIndex = 5;
                    break;
                default:
                    rgMaterial.SelectedIndex = -1;
                    break;
            }
        }

        private void selectPurchase(int value)
        {
            switch (value)
            {
                case 0:
                    rgPurchase.SelectedIndex = 0;
                    break;
                case 1:
                    rgPurchase.SelectedIndex = 1;
                    break;
                case 9:
                    rgPurchase.SelectedIndex = 2;
                    break;
                default:
                    rgPurchase.SelectedIndex = -1;
                    break;
            }
        }

        private void selectTax(int value)
        {
            switch (value)
            {
                case 1:
                    rgTax.SelectedIndex = 0;
                    break;
                case 2:
                    rgTax.SelectedIndex = 1;
                    break;
                case 9:
                    rgTax.SelectedIndex = 2;
                    break;
                default:
                    rgTax.SelectedIndex = -1;
                    break;
            }
        }

        private void selectZone(int value)
        {
            switch (value)
            {
                case 0:
                    rgZone.SelectedIndex = 0;
                    break;
                case 1:
                    rgZone.SelectedIndex = 1;
                    break;
                case 2:
                    rgZone.SelectedIndex = 2;
                    break;
                default:
                    rgZone.SelectedIndex = -1;
                    break;
            }
        }

        private void rgMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            NewData();
            LoadDataMeterial();
            glueCode.Focus();
        }

        private void gvVendor_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if (sender is GridView)
            {
                GridView gView = (GridView)sender;
                if (!gView.IsValidRowHandle(e.RowHandle)) return;
                int parent = gView.GetParentRowHandle(e.RowHandle);
                if (gView.IsGroupRow(parent))
                {
                    for (int i = 0; i < gView.GetChildRowCount(parent); i++)
                    {
                        if (gView.GetChildRowHandle(parent, i) == e.RowHandle)
                        {
                            e.Appearance.BackColor = i % 2 == 0 ? Color.AliceBlue : Color.White;
                        }
                    }
                }
                else
                {
                    e.Appearance.BackColor = e.RowHandle % 2 == 0 ? Color.AliceBlue : Color.White;
                }
            }
        }

        private void LoadCode(string strCODE)
        {
            strCODE = strCODE.ToUpper().Trim();
            //txeID.Text = "";
            //txeDescription.Text = "";
            //txeComposition.Text = "";
            //txeWeight.Text = "";
            //txeModelNo.Text = "";
            //txeModelName.Text = "";
            //slueCategory.EditValue = "";
            //slueStyle.EditValue = "";
            //slueColor.EditValue = "";
            //slueSize.EditValue = "";
            //slueCustomer.EditValue = "";
            //txeBusinessUnit.Text = "";
            //cbeSeason.EditValue = "";
            //cbeClass.EditValue = "";
            //glueBranch.EditValue = "";
            //txeCostSheet.Text = "";
            //txeStdPrice.Text = "";

            //slueFirstVendor.EditValue = "";
            //txeMatDetails.Text = "";
            //txeMatCode.Text = "";
            //txeSMPLLotNo.Text = "";
            //txePrice.Text = "";
            //txeCurrency.Text = "";
            //rgPurchase.SelectedIndex = 0;
            //rgTax.SelectedIndex = -1;
            //txePurchaseLoss.Text = "";
            //dteFirstReceiptDate.EditValue = DateTime.Now;
            //slueDefaultVendor.EditValue = "";

            //txeSMPLNo.Text = "";
            //dteRequestDate.EditValue = DateTime.Now;
            //txeSMPLItem.Text = "";
            //txeSMPLPatternNo.Text = "";
            //rgZone.SelectedIndex = 0;

            //txeMinStock.Text = "";
            //txeMaxStock.Text = "";
            //txeStockSheifLife.Text = "";
            //txeStdCost.Text = "";
            //slueDefaultUnit.EditValue = "";
            //glueUnit.EditValue = "";

            //picImg.EditValue = "";
            //txePath.Text = "";

            //txeLabTestNo.Text = "";
            //dteApprovedLabDate.EditValue = DateTime.Now;
            //txeQCInspection.Text = "";
            //clbQC.Items.Clear();

            //slueVendorCode.EditValue = "";
            //txeVendorName.Text = "";
            //txeLotSize.Text = "";
            //txeProductionLead.Text = "";
            //txeDeliveryLead.Text = "";
            //txeArrivalLead.Text = "";
            //txePOCancelPeriod.Text = "";

            //txeLots1.Text = "";
            //txeLots2.Text = "";
            //txeLots3.Text = "";

            //txeRemark.Text = "";
            //lblIDVENDItem.Text = "";

            //txeCREATE.Text = "0";
            //txeCDATE.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            //txeUPDATE.Text = "0";
            //txeUDATE.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            ////**************************************
            //slueFirstVendor.ReadOnly = false;
            //txeMatDetails.ReadOnly = false;
            //txeMatCode.ReadOnly = false;
            //txeSMPLLotNo.ReadOnly = false;
            //txePrice.ReadOnly = false;
            //txeCurrency.ReadOnly = false;
            //rgPurchase.ReadOnly = false;
            //rgTax.ReadOnly = false;
            //txePurchaseLoss.ReadOnly = false;
            //dteFirstReceiptDate.ReadOnly = false;
            ////**************************************
            //string Materialx = rgMaterial.Properties.Items[rgMaterial.SelectedIndex].Value.ToString();
            StringBuilder sbSQL = new StringBuilder();
            //sbSQL.Append("SELECT OIDCONDQC, ConditionName ");
            //sbSQL.Append("FROM ConditionQC ");
            //sbSQL.Append("WHERE (ItemType = '" + Materialx + "') ");
            //sbSQL.Append("ORDER BY ConditionName ");
            //DataTable drMaterial = new DBQuery(sbSQL).getDataTable();
            //clbQC.ValueMember = "OIDCONDQC";
            //clbQC.DisplayMember = "ConditionName";
            //clbQC.DataSource = drMaterial;

            //selCode = "";
            sbSQL.Clear();
            //StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("SELECT OIDITEM, MaterialType, Code, Description, Composition, WeightOrMoreDetail, ModelNo, ModelName, OIDCATEGORY, OIDSTYLE, OIDCOLOR, OIDSIZE, OIDCUST, BusinessUnit, Season, ClassType, Branch, CostSheetNo,  ");
            sbSQL.Append("       FORMAT(StdPrice, '###0.####') AS StdPrice, FirstVendor, PurchaseType, TaxBenefits, FORMAT(PurchaseLoss, '###0.##') AS PurchaseLoss, FirstReceiptDate, DefaultVendor, MinStock, MaxStock, StockShelfLife, FORMAT(StdCost, '###0.####') AS StdCost, ");
            sbSQL.Append("       DefaultUnit, PathFile, LabTestNo, ApprovedLabDate, QCInspection, CreatedBy, CreatedDate, ");
            sbSQL.Append("       UpdatedBy, UpdatedDate ");
            sbSQL.Append("FROM Items ");
            sbSQL.Append("WHERE (Code = N'" + strCODE.Replace("'", "''") + "') ");
            string[] arrItem = new DBQuery(sbSQL).getMultipleValue();
            if (arrItem.Length > 0)
            {
                if (FUNC.msgQuiz("The system already has this name. Want to fix it ?") == true)
                {
                    txeID.Text = arrItem[0];
                    txeDescription.Text = arrItem[3];
                    txeComposition.Text = arrItem[4];
                    txeWeight.Text = arrItem[5];
                    txeModelNo.Text = arrItem[6];
                    txeModelName.Text = arrItem[7];
                    slueCategory.EditValue = arrItem[8];
                    slueStyle.EditValue = arrItem[9];
                    slueColor.EditValue = arrItem[10];
                    slueSize.EditValue = arrItem[11];
                    slueCustomer.EditValue = arrItem[12];
                    txeBusinessUnit.Text = arrItem[13];
                    cbeSeason.Text = arrItem[14];
                    cbeClass.Text = arrItem[15];
                    glueBranch.EditValue = arrItem[16];
                    txeCostSheet.Text = arrItem[17];
                    txeStdPrice.Text = arrItem[18];

                    slueFirstVendor.EditValue = arrItem[19];
                    //txeMatDetails.Text = "";
                    //txeMatCode.Text = "";
                    //txeSMPLLotNo.Text = "";
                    //txePrice.Text = "";
                    //txeCurrency.Text = "";
                    selectPurchase(Convert.ToInt32(arrItem[20]));
                    selectTax(Convert.ToInt32(arrItem[21]));

                    txePurchaseLoss.Text = arrItem[22];
                    dteFirstReceiptDate.EditValue = Convert.ToDateTime(arrItem[23]);
                    slueDefaultVendor.EditValue = arrItem[24];

                    txeSMPLNo.Text = "";
                    dteRequestDate.EditValue = DateTime.Now;
                    txeSMPLItem.Text = "";
                    txeSMPLPatternNo.Text = "";
                    rgZone.SelectedIndex = 0;

                    txeMinStock.Text = arrItem[25];
                    txeMaxStock.Text = arrItem[26];
                    txeStockSheifLife.Text = arrItem[27];
                    txeStdCost.Text = arrItem[28];
                    slueDefaultUnit.EditValue = arrItem[29];
                    glueUnit.EditValue = "";

                    txePath.Text = imgPathFile + arrItem[30];
                    picImg.Image = Image.FromFile(txePath.Text);

                    txeLabTestNo.Text = arrItem[31];
                    dteApprovedLabDate.EditValue = Convert.ToDateTime(arrItem[32]);
                    txeQCInspection.Text = arrItem[33];
                    //clbQC.Items.Clear();

                    slueVendorCode.EditValue = "";
                    txeVendorName.Text = "";
                    txeLotSize.Text = "";
                    txeProductionLead.Text = "";
                    txeDeliveryLead.Text = "";
                    txeArrivalLead.Text = "";
                    txePOCancelPeriod.Text = "";

                    txeLots1.Text = "";
                    txeLots2.Text = "";
                    txeLots3.Text = "";

                    txeRemark.Text = "";
                    lblIDVENDItem.Text = "";

                    txeCREATE.Text = arrItem[34];
                    txeCDATE.Text = arrItem[35];
                    txeUPDATE.Text = arrItem[36];
                    txeUDATE.Text = arrItem[37];

                    //**************************************
                    slueFirstVendor.ReadOnly = true;
                    txeMatDetails.ReadOnly = true;
                    txeMatCode.ReadOnly = true;
                    txeSMPLLotNo.ReadOnly = true;
                    txePrice.ReadOnly = true;
                    txeCurrency.ReadOnly = true;
                    rgPurchase.ReadOnly = true;
                    rgTax.ReadOnly = true;
                    txePurchaseLoss.ReadOnly = true;
                    dteFirstReceiptDate.ReadOnly = true;
                    //**************************************
                }
                else
                {
                    txeID.Text = "";
                    if (rgMaterial.SelectedIndex > -1)
                    {
                        string Material = rgMaterial.Properties.Items[rgMaterial.SelectedIndex].Value.ToString();
                        txeID.Text = new DBQuery("SELECT CASE WHEN ISNULL(MAX(OIDITEM), '') = '' THEN 1 ELSE MAX(OIDITEM) + 1 END AS NewNo FROM Items WHERE (MaterialType = '" + Material + "') ").getString();
                    }
                    glueCode.Text = "";
                    glueCode.Focus();

                }
            }
            else
            {
                txeID.Text = new DBQuery("SELECT CASE WHEN ISNULL(MAX(OIDCUST), '') = '' THEN 1 ELSE MAX(OIDCUST) + 1 END AS NewNo FROM Customer").getString();
               
                //bool chkNameDup = chkDuplicateName();
                //if (chkNameDup == false)
                //{
                //    txeDescription.Text = "";
                //}

                txeDescription.Focus();
            }

            selCode = "";


            sbSQL.Clear();
            sbSQL.Append("SELECT OIDCONDQC ");
            sbSQL.Append("FROM ItemInspection ");
            sbSQL.Append("WHERE (OIDITEM = '" + txeID.Text.Trim() + "') ");
            sbSQL.Append("ORDER BY OIDCONDQC ");
            DataTable dtQC = new DBQuery(sbSQL).getDataTable();

            foreach (DataRow row in dtQC.Rows)
            {
                for (int i = 0; i < clbQC.ItemCount; i++)
                {
                    if (row["OIDCONDQC"].ToString() == clbQC.GetItemValue(i).ToString())
                    {
                        clbQC.SetItemCheckState(i, CheckState.Checked);
                        break;
                    }
                }
            }

            sbSQL.Clear();
            sbSQL.Append("SELECT OIDVENDItem, OIDVEND, Code AS Vendor, Name AS VendorName, OIDITEM, LotSize, ProductionLead, DeliveryLead, ArrivalLead, POCancelPeriod, PurchaseLots1, PurchaseLots2, PurchaseLots3, Remark ");
            sbSQL.Append("FROM ItemVendor AS IVD ");
            sbSQL.Append("     CROSS APPLY(SELECT Code, Name FROM Vendor WHERE OIDVEND = IVD.OIDVEND) AS VD ");
            sbSQL.Append("WHERE (OIDITEM = '" + txeID.Text.Trim() + "') ");
            sbSQL.Append("ORDER BY OIDVENDItem ");
            new ObjDevEx.setGridControl(gcVendor, gvVendor, sbSQL).getData(false, false, false, true);
            dtVendor = new DBQuery(sbSQL).getDataTable();
            if (gvVendor.Columns.Count > 0)
            {
                //เปลี่ยนชื่อ Column ใน DataTable ให้ตรงกับ DataGridView
                for (int ii = 0; ii < gvVendor.Columns.Count; ii++)
                {
                    try
                    {
                        dtVendor.Columns[ii].ColumnName = gvVendor.Columns[ii].FieldName;
                        dtVendor.Columns[ii].AllowDBNull = true;
                    }
                    catch (Exception) { }
                    ii++;
                }
            }
            gvVendor.Columns[0].Visible = false;
            gvVendor.Columns[1].Visible = false;
            gvVendor.Columns[4].Visible = false;

            txeDescription.Focus();
        }

        private void glueCode_EditValueChanged(object sender, EventArgs e)
        {
            //txeDescription.Focus();
            //LoadCode(glueCode.Text);

        }

        private void glueCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeDescription.Focus();
            }
        }

        private void glueCode_LostFocus(object sender, EventArgs e)
        {
           

        }

        private void slueCategory_EditValueChanged(object sender, EventArgs e)
        {
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("SELECT OIDSTYLE AS ID, StyleName ");
            sbSQL.Append("FROM  ProductStyle ");
            sbSQL.Append("WHERE (OIDGCATEGORY = '" + slueCategory.EditValue.ToString() + "') ");
            sbSQL.Append("ORDER BY StyleName ");
            new ObjDevEx.setSearchLookUpEdit(slueStyle, sbSQL, "StyleName", "ID").getData(true);
            slueStyle.Focus();
        }

        private void slueFirstVendor_EditValueChanged(object sender, EventArgs e)
        {
            slueDefaultVendor.EditValue = slueFirstVendor.EditValue.ToString();

            txeMatDetails.Text = "";
            txeMatCode.Text = "";
            txeSMPLLotNo.Text = "";
            txePrice.Text = "";
            txeCurrency.Text = "";

            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("SELECT TOP (1) MatDetails, MatCode, SMPLLotNo, FORMAT(Price, '###0.####') AS Price, Currency ");
            sbSQL.Append("FROM   ItemVendor ");
            sbSQL.Append("WHERE(OIDITEM = '" + txeID.Text.Trim() + "') AND(OIDVEND = '" + slueFirstVendor.EditValue.ToString() + "') ");
            string[] arrVEND = new DBQuery(sbSQL).getMultipleValue();
            if (arrVEND.Length > 0)
            {
                txeMatDetails.Text = arrVEND[0];
                txeMatCode.Text = arrVEND[1];
                txeSMPLLotNo.Text = arrVEND[2];
                txePrice.Text = arrVEND[3];
                txeCurrency.Text = arrVEND[4];
            }

            txeMatDetails.Focus();
        }

        private void slueDefaultVendor_EditValueChanged(object sender, EventArgs e)
        {
            slueVendorCode.EditValue = slueDefaultVendor.EditValue.ToString();
            txeMinStock.Focus();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            xtraOpenFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.gif,*.png";
            xtraOpenFileDialog1.FileName = "";
            xtraOpenFileDialog1.Title = "Select Image File";

            if (xtraOpenFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txePath.Text = xtraOpenFileDialog1.FileName;
                
                picImg.Image = Image.FromFile(txePath.Text);
            }

            txeLabTestNo.Focus();
        }

        private void btnNEW_Click(object sender, EventArgs e)
        {
            slueVendorCode.EditValue = "";
            txeVendorName.Text = "";
            txeLotSize.Text = "";
            txeProductionLead.Text = "";
            txeDeliveryLead.Text = "";
            txeArrivalLead.Text = "";
            txePOCancelPeriod.Text = "";

            txeLots1.Text = "";
            txeLots2.Text = "";
            txeLots3.Text = "";

            txeRemark.Text = "";
            lblIDVENDItem.Text = "";
            selCode = "";

        }

        private void slueVendorCode_EditValueChanged(object sender, EventArgs e)
        {
            txeVendorName.Text = "";
            txeLotSize.Text = "";
            txeProductionLead.Text = "";
            txeDeliveryLead.Text = "";
            txeArrivalLead.Text = "";
            txePOCancelPeriod.Text = "";

            txeLots1.Text = "";
            txeLots2.Text = "";
            txeLots3.Text = "";

            txeRemark.Text = "";
            lblIDVENDItem.Text = "";
            //selCode = "";

            if (slueVendorCode.Text.Trim() != "")
            {
                StringBuilder sbSQL = new StringBuilder();
                sbSQL.Append("SELECT IVD.OIDVENDItem, VD.OIDVEND, VD.Code AS Vendor, VD.Name AS VendorName, IVD.OIDITEM, IVD.LotSize, IVD.ProductionLead, IVD.DeliveryLead,  ");
                sbSQL.Append("       IVD.ArrivalLead, IVD.POCancelPeriod, IVD.PurchaseLots1, IVD.PurchaseLots2, IVD.PurchaseLots3, IVD.Remark ");
                sbSQL.Append("FROM   Vendor AS VD LEFT OUTER JOIN ");
                sbSQL.Append("       ItemVendor AS IVD ON IVD.OIDVEND = VD.OIDVEND ");
                sbSQL.Append("WHERE(VD.OIDVEND = '" + slueVendorCode.EditValue.ToString() + "') ");
                string[] arrVendor = new DBQuery(sbSQL).getMultipleValue();
                if (arrVendor.Length > 0)
                {
                    txeVendorName.Text = arrVendor[3];
                    txeLotSize.Text = arrVendor[5];
                    txeProductionLead.Text = arrVendor[6];
                    txeDeliveryLead.Text = arrVendor[7];
                    txeArrivalLead.Text = arrVendor[8];
                    txePOCancelPeriod.Text = arrVendor[9];

                    txeLots1.Text = arrVendor[10];
                    txeLots2.Text = arrVendor[11];
                    txeLots3.Text = arrVendor[12];

                    txeRemark.Text = arrVendor[13];
                    lblIDVENDItem.Text = arrVendor[0];
                }
            }
            //MessageBox.Show(slueVendorCode.EditValue.ToString());
            txeVendorName.Focus();
        }

        private void btnADD_Click(object sender, EventArgs e)
        {
            if (slueVendorCode.Text.Trim() == "")
            {
                FUNC.msgWarning("Please select vendor code.");
                slueVendorCode.Focus();
            }
            else
            {
                bool chkVendor = true;
                foreach (DataRow dr in dtVendor.Rows) // search whole table
                {
                    if (dr["OIDVEND"].ToString() == slueVendorCode.EditValue.ToString())
                    {
                        dr["LotSize"] = txeLotSize.Text.Trim();

                        dr["ProductionLead"] = txeProductionLead.Text.Trim();
                        dr["DeliveryLead"] = txeDeliveryLead.Text.Trim();
                        dr["ArrivalLead"] = txeArrivalLead.Text.Trim();
                        dr["POCancelPeriod"] = txePOCancelPeriod.Text.Trim();

                        dr["PurchaseLots1"] = txeLots1.Text.Trim();
                        dr["PurchaseLots2"] = txeLots2.Text.Trim();
                        dr["PurchaseLots3"] = txeLots3.Text.Trim();
                        dr["Remark"] = txeRemark.Text.Trim();

                        chkVendor = false;
                        break;
                    }
                }

                if (chkVendor == true)
                {
                    dtVendor.Rows.Add(new Object[]{
                    "",
                    slueVendorCode.EditValue.ToString(),
                    slueVendorCode.Text,
                    txeVendorName.Text,
                    txeID.Text.Trim(),
                    txeLotSize.Text.Trim(),
                    txeProductionLead.Text.Trim(),
                    txeDeliveryLead.Text.Trim(),
                    txeArrivalLead.Text.Trim(),
                    txePOCancelPeriod.Text.Trim(),
                    txeLots1.Text.Trim(),
                    txeLots2.Text.Trim(),
                    txeLots3.Text.Trim(),
                    txeRemark.Text.Trim()
                });
                }

                gcVendor.DataSource = dtVendor;
                gcVendor.EndUpdate();
                gcVendor.ResumeLayout();
                gvVendor.ClearSelection();
                gvVendor.OptionsView.ColumnAutoWidth = true;
                gvVendor.BestFitColumns();
                gvVendor.ClearSelection();

                btnNEW.PerformClick();
            }
        }

        private void gvVendor_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            slueVendorCode.EditValue = gvVendor.GetFocusedRowCellValue("OIDVEND").ToString();
            txeVendorName.Text = gvVendor.GetFocusedRowCellValue("VendorName").ToString();
            txeLotSize.Text = gvVendor.GetFocusedRowCellValue("LotSize").ToString();
            txeProductionLead.Text = gvVendor.GetFocusedRowCellValue("ProductionLead").ToString();
            txeDeliveryLead.Text = gvVendor.GetFocusedRowCellValue("DeliveryLead").ToString();
            txeArrivalLead.Text = gvVendor.GetFocusedRowCellValue("ArrivalLead").ToString();
            txePOCancelPeriod.Text = gvVendor.GetFocusedRowCellValue("POCancelPeriod").ToString();

            txeLots1.Text = gvVendor.GetFocusedRowCellValue("PurchaseLots1").ToString();
            txeLots2.Text = gvVendor.GetFocusedRowCellValue("PurchaseLots2").ToString();
            txeLots3.Text = gvVendor.GetFocusedRowCellValue("PurchaseLots3").ToString();

            txeRemark.Text = gvVendor.GetFocusedRowCellValue("Remark").ToString();
            lblIDVENDItem.Text = gvVendor.GetFocusedRowCellValue("OIDVENDItem").ToString();
        }

        private void bbiExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
           
        }

        private void picImg_Click(object sender, EventArgs e)
        {
            if (txePath.Text.Trim() != "")
            {
                ShowImage frmIMG = new ShowImage(txePath.Text.Trim());
                frmIMG.Show();
            }
        }

        private void glueCode_Closed(object sender, ClosedEventArgs e)
        {
            glueCode.Focus();
            txeDescription.Focus();
        }

        private void glueCode_ProcessNewValue(object sender, ProcessNewValueEventArgs e)
        {
            GridLookUpEdit gridLookup = sender as GridLookUpEdit;
            if (e.DisplayValue == null) return;
            string newValue = e.DisplayValue.ToString();
            if (newValue == String.Empty) return;
        }

        private void glueCode_Leave(object sender, EventArgs e)
        {
            if (glueCode.Text.Trim() != "" && glueCode.Text.ToUpper().Trim() != selCode)
            {
                glueCode.Text = glueCode.Text.ToUpper().Trim();
                selCode = glueCode.Text;
                LoadCode(glueCode.Text);
            }
        }

        private void txeDescription_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeComposition.Focus();
            }
        }

        private void txeDescription_Leave(object sender, EventArgs e)
        {

        }

        private void txeComposition_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeWeight.Focus();
            }
        }

        private void txeWeight_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeModelNo.Focus();
            }
        }

        private void txeModelNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeModelName.Focus();
            }
        }

        private void txeModelName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                slueCategory.Focus();
            }
        }

        private void slueStyle_EditValueChanged(object sender, EventArgs e)
        {
            slueColor.Focus();
        }

        private void slueColor_EditValueChanged(object sender, EventArgs e)
        {
            slueSize.Focus();
        }

        private void slueSize_EditValueChanged(object sender, EventArgs e)
        {
            slueCustomer.Focus();
        }

        private void slueCustomer_EditValueChanged(object sender, EventArgs e)
        {
            txeBusinessUnit.Focus();
        }

        private void txeBusinessUnit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                cbeSeason.Focus();
            }
        }

        private void cbeSeason_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbeClass.Focus();
        }

        private void cbeClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            glueBranch.Focus();
        }

        private void glueBranch_EditValueChanged(object sender, EventArgs e)
        {
            txeCostSheet.Focus();
        }

        private void txeCostSheet_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeStdPrice.Focus();
            }
        }

        private void txeStdPrice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                slueFirstVendor.Focus();
            }
        }

        private void txeMatDetails_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeMatCode.Focus();
            }
        }

        private void txeMatCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeSMPLLotNo.Focus();
            }
        }

        private void txeSMPLLotNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txePrice.Focus();
            }
        }

        private void txePrice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeCurrency.Focus();
            }
        }

        private void txeCurrency_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                rgPurchase.Focus();
            }
        }

        private void txePurchaseLoss_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                dteFirstReceiptDate.Focus();
            }
        }

        private void dteFirstReceiptDate_EditValueChanged(object sender, EventArgs e)
        {
            slueDefaultVendor.Focus();
        }

        private void txeMinStock_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeMaxStock.Focus();
            }
        }

        private void txeMaxStock_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeStockSheifLife.Focus();
            }
        }

        private void txeStockSheifLife_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Enter)
            {
                txeStdCost.Focus();
            }
        }

        private void txeStdCost_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                slueDefaultUnit.Focus();
            }
        }

        private void slueDefaultUnit_EditValueChanged(object sender, EventArgs e)
        {
            glueUnit.Focus();
        }

        private void glueUnit_EditValueChanged(object sender, EventArgs e)
        {
            btnSelect.Focus();
        }

        private void txePath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeLabTestNo.Focus();
            }
        }

        private void txeLabTestNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                dteApprovedLabDate.Focus();
            }
        }

        private void dteApprovedLabDate_EditValueChanged(object sender, EventArgs e)
        {
            txeQCInspection.Focus();
        }

        private void txeQCInspection_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                clbQC.Focus();
            }
        }

        private void txeSMPLNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                dteRequestDate.Focus();
            }
        }

        private void dteRequestDate_EditValueChanged(object sender, EventArgs e)
        {
            txeSMPLItem.Focus();
        }

        private void txeSMPLItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeSMPLPatternNo.Focus();
            }
        }

        private void txeSMPLPatternNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                rgZone.Focus();
            }
        }

        private void txeVendorName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeLotSize.Focus();
            }
        }

        private void txeProductionLead_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void txeLotSize_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeProductionLead.Focus();
            }
        }

        private void txeProductionLead_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeDeliveryLead.Focus();
            }
        }

        private void txeDeliveryLead_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeArrivalLead.Focus();
            }
        }

        private void txeArrivalLead_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txePOCancelPeriod.Focus();
            }
        }

        private void txePOCancelPeriod_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeLots1.Focus();
            }
        }

        private void txeLots1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeLots2.Focus();
            }
        }

        private void txeLots2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeLots3.Focus();
            }
        }

        private void txeLots3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeRemark.Focus();
            }
        }
    }
}