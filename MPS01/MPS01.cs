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
using System.Globalization;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Data;

namespace MPS01
{
    public partial class MPS01 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private Functionality.Function FUNC = new Functionality.Function();

        StringBuilder sbSTATUS = new StringBuilder();
        public MPS01()
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
            sbSTATUS.Clear();
            sbSTATUS.Append("SELECT '1' AS ID, 'Ph1 - New Order' AS Status ");
            sbSTATUS.Append("UNION ALL ");
            sbSTATUS.Append("SELECT '2' AS ID, 'Ph2 - Repeat Order' AS Status ");
            sbSTATUS.Append("UNION ALL ");
            sbSTATUS.Append("SELECT '3' AS ID, 'Ph3 - Urgent, (Adjust) Revise(+/-)' AS Status ");
            sbSTATUS.Append("UNION ALL ");
            sbSTATUS.Append("SELECT '4' AS ID, 'Cancel' AS Status ");
            sbSTATUS.Append("UNION ALL ");
            sbSTATUS.Append("SELECT '5' AS ID, 'End Order' AS Status ");

            glueUnit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            glueUnit.Properties.AcceptEditorTextAsNewValue = DevExpress.Utils.DefaultBoolean.True;

            glueLogisticsType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            glueLogisticsType.Properties.AcceptEditorTextAsNewValue = DevExpress.Utils.DefaultBoolean.True;

            LoadData();
            NewData();
        }

        private void LoadSummary()
        {
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("SELECT COF.OIDFC AS ID, COF.ProductionPlanID, COF.Season, COF.OIDCUST AS [Customer ID], CUS.Name AS Customer, COF.BusinessUnit, COF.OIDCSITEM, ITC.ItemCode, ITC.ItemName, COF.ModelNo AS MainSampleCode, ");
            sbSQL.Append("       COF.OIDVEND AS SupplierID, VD.Name AS Supplier, COF.SewingDifficulty, COF.ProductionPlanType, COF.Status AS StatusID, STS.Status, COF.LastUpdate AS LastUpdateDate, COF.RequestedWHDate, COF.ContractedDate, ");
            sbSQL.Append("       COF.TransportMethod AS TransportMethodName, COF.LogisticsType AS LogisticsTypeName, COF.OrderQty AS[Order Qty(Pcs)], COF.FabricOrderNO AS[FabricOrderNo.], COF.FabricActualOrderQty AS[FB Actual Order Qty(Pcs)], ");
            sbSQL.Append("       COF.FabricUpdateDate AS[FB Update Date], COF.TrimOrderNO AS[TrimOrderNo.], COF.TrimActualOrderQty AS[TrimOrderActualQty(Pcs)], COF.TrimUpdateDate, COF.POOrderNO AS[PO Order No.], ");
            sbSQL.Append("       COF.POActualOrderQty AS[PO Actual Order Qty(Pcs)], COF.POUpdateDate, COF.ColorOrderNO AS[Color Order No.], COF.ColorActualOrderQty AS[Color Actual Order Qty(Pcs)], COF.ColorUpdateDate, ");
            sbSQL.Append("       COF.OrderQTYOld AS[Order Qty(Old)], COF.BookingFabric, COF.BookingAccessory, COF.FileOrderDate, COF.DataUpdate, COF.CreateBy, COF.CreateDate, COF.UpdateBy, COF.Updatedate ");
            sbSQL.Append("FROM   COForecast AS COF LEFT OUTER JOIN ");
            sbSQL.Append("       Customer AS CUS ON COF.OIDCUST = CUS.OIDCUST LEFT OUTER JOIN ");
            sbSQL.Append("       Vendor AS VD ON COF.OIDVEND = VD.OIDVEND LEFT OUTER JOIN ");
            sbSQL.Append("       ItemCustomer AS ITC ON COF.OIDCSITEM = ITC.OIDCSITEM LEFT OUTER JOIN ");
            sbSQL.Append("       (");
            sbSQL.Append(sbSTATUS);
            sbSQL.Append("       ) AS STS ON COF.Status = STS.ID ");
            sbSQL.Append("ORDER BY COF.ProductionPlanID ");
            new ObjDevEx.setGridControl(gcFO, gvFO, sbSQL).getData(false, false, false, true);
        }

        private void LoadData()
        {
            new ObjDevEx.setGridLookUpEdit(glueStatus, sbSTATUS, "Status", "ID").getData();

            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("SELECT Code, Name AS Supplier, OIDVEND AS ID ");
            sbSQL.Append("FROM Vendor ");
            sbSQL.Append("WHERE (VendorType = 6) ");
            sbSQL.Append("ORDER BY Code ");
            new ObjDevEx.setSearchLookUpEdit(slueSupplier, sbSQL, "Supplier", "ID").getData();

            sbSQL.Clear();
            sbSQL.Append("SELECT Code, Name AS Customer, OIDCUST AS ID ");
            sbSQL.Append("FROM Customer ");
            sbSQL.Append("ORDER BY Code ");
            new ObjDevEx.setSearchLookUpEdit(slueCustomer, sbSQL, "Customer", "ID").getData();

            sbSQL.Clear();
            sbSQL.Append("SELECT ITC.ItemCode, ITC.ItemName, CUS.Name AS Customer, ITC.StyleNo AS [StyleNo.], ITC.Season, ITC.OIDCSITEM AS ID ");
            sbSQL.Append("FROM   ItemCustomer AS ITC LEFT OUTER JOIN ");
            sbSQL.Append("       Customer AS CUS ON ITC.OIDCUST = CUS.OIDCUST ");
            sbSQL.Append("ORDER BY ITC.ItemCode ");
            new ObjDevEx.setSearchLookUpEdit(slueItemCode, sbSQL, "ItemCode", "ItemCode").getData();

            sbSQL.Clear();
            sbSQL.Append("SELECT BusinessUnit ");
            sbSQL.Append("FROM COForecast ");
            sbSQL.Append("ORDER BY BusinessUnit");
            new ObjDevEx.setGridLookUpEdit(glueUnit, sbSQL, "BusinessUnit", "BusinessUnit").getData();

            sbSQL.Clear();
            sbSQL.Append("SELECT SeasonNo AS [Season No.], SeasonName AS [Season Name] ");
            sbSQL.Append("FROM Season ");
            sbSQL.Append("ORDER BY OIDSEASON");
            new ObjDevEx.setGridLookUpEdit(glueSeason, sbSQL, "Season No.", "Season No.").getData();

            sbSQL.Clear();
            sbSQL.Append("SELECT TransportMethod ");
            sbSQL.Append("FROM (SELECT DISTINCT TransportMethod ");
            sbSQL.Append("      FROM (SELECT DISTINCT TransportMethod ");
            sbSQL.Append("            FROM COForecast ");
            sbSQL.Append("            UNION ALL ");
            sbSQL.Append("            SELECT '' AS TransportMethod ");
            sbSQL.Append("            UNION ALL ");
            sbSQL.Append("            SELECT 'Air' AS TransportMethod ");
            sbSQL.Append("            UNION ALL ");
            sbSQL.Append("            SELECT 'Ship' AS TransportMethod ");
            sbSQL.Append("            UNION ALL ");
            sbSQL.Append("            SELECT 'Truck' AS TransportMethod) AS DTM) AS TM ");
            sbSQL.Append("ORDER BY CASE TransportMethod ");
            sbSQL.Append("    WHEN '' THEN '0' ");
            sbSQL.Append("    WHEN 'Air' THEN '1' ");
            sbSQL.Append("    WHEN 'Ship' THEN '2' ");
            sbSQL.Append("    WHEN 'Truck' THEN '3' ");
            sbSQL.Append("    ELSE TransportMethod ");
            sbSQL.Append("END ");
            new ObjDevEx.setGridLookUpEdit(glueTransport, sbSQL, "TransportMethod", "TransportMethod").getData();

            sbSQL.Clear();
            sbSQL.Append("SELECT LogisticsType ");
            sbSQL.Append("FROM (SELECT DISTINCT LogisticsType ");
            sbSQL.Append("      FROM (SELECT DISTINCT LogisticsType ");
            sbSQL.Append("            FROM COForecast ");
            sbSQL.Append("            UNION ALL ");
            sbSQL.Append("            SELECT '' AS LogisticsType ");
            sbSQL.Append("            UNION ALL ");
            sbSQL.Append("            SELECT 'ADC' AS LogisticsType ");
            sbSQL.Append("            UNION ALL ");
            sbSQL.Append("            SELECT 'MDC' AS LogisticsType) AS DTM) AS TM ");
            sbSQL.Append("ORDER BY CASE LogisticsType ");
            sbSQL.Append("    WHEN '' THEN '0' ");
            sbSQL.Append("    WHEN 'ADC' THEN '1' ");
            sbSQL.Append("    WHEN 'MDC' THEN '2' ");
            sbSQL.Append("    ELSE LogisticsType ");
            sbSQL.Append("END ");
            new ObjDevEx.setGridLookUpEdit(glueLogisticsType, sbSQL, "LogisticsType", "LogisticsType").getData();

            //*** SET GRIDCONTROL COLUMN *****
            repositoryItemSearchLookUpEdit1.DataSource = slueCustomer.Properties.DataSource;
            repositoryItemSearchLookUpEdit1.DisplayMember = "Customer";
            repositoryItemSearchLookUpEdit1.ValueMember = "ID";
            repositoryItemSearchLookUpEdit1.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;

            repositoryItemGridLookUpEdit1.DataSource = glueSeason.Properties.DataSource;
            repositoryItemGridLookUpEdit1.DisplayMember = "Season No.";
            repositoryItemGridLookUpEdit1.ValueMember = "Season No.";
            repositoryItemGridLookUpEdit1.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;

            repositoryItemGridLookUpEdit2.DataSource = glueUnit.Properties.DataSource;
            repositoryItemGridLookUpEdit2.DisplayMember = "BusinessUnit";
            repositoryItemGridLookUpEdit2.ValueMember = "BusinessUnit";
            repositoryItemGridLookUpEdit2.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            repositoryItemGridLookUpEdit2.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            repositoryItemGridLookUpEdit2.AcceptEditorTextAsNewValue = DevExpress.Utils.DefaultBoolean.True;

            repositoryItemSearchLookUpEdit2.DataSource = slueItemCode.Properties.DataSource;
            repositoryItemSearchLookUpEdit2.DisplayMember = "ItemCode";
            repositoryItemSearchLookUpEdit2.ValueMember = "ItemCode";
            repositoryItemSearchLookUpEdit2.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;

            DataTable dtINPUT = new DataTable();
            dtINPUT.Columns.Add("ProductionPlanID", typeof(String));
            dtINPUT.Columns.Add("OIDCUST", typeof(String));
            dtINPUT.Columns.Add("FileOrderDate", typeof(DateTime));
            dtINPUT.Columns.Add("Year", typeof(Int32));
            dtINPUT.Columns.Add("Season", typeof(String));
            dtINPUT.Columns.Add("BusinessUnit", typeof(String));
            dtINPUT.Columns.Add("OIDCSITEM", typeof(String));
            dtINPUT.Columns.Add("ItemName", typeof(String));
            dtINPUT.Columns.Add("StyleNo", typeof(String));
            dtINPUT.Columns.Add("ModelNo", typeof(String));

            //dtINPUT.Rows.Add("", "", null);
            gcINPUT.DataSource = dtINPUT;

            LoadSummary();
        }

        private void NewData()
        {
            tabbedControlGroup1.SelectedTabPage = layoutControlGroup1;
            bbiRefresh.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

            speSeason.Value = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
            //lblStatus.Text = "* Add Port";
            //lblStatus.ForeColor = Color.Green;

            //txeID.Text = new DBQuery("SELECT CASE WHEN ISNULL(MAX(OIDPORT), '') = '' THEN 1 ELSE MAX(OIDPORT) + 1 END AS NewNo FROM PortAndCity").getString();
            //txeCode.Text = "";
            //txeName.Text = "";
            //txeCity.Text = "";
            //slueCountry.EditValue = "";
            //txeCREATE.Text = "0";
            //txeDATE.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            //txeCode.Focus();
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
            NewData();
        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gvINPUT.CloseEditor();
            gvINPUT.UpdateCurrentRow();
            //if (txeCode.Text.Trim() == "")
            //{
            //    FUNC.msgWarning("Please input code.");
            //    txeCode.Focus();
            //}
            //else if (txeName.Text.Trim() == "")
            //{
            //    FUNC.msgWarning("Please input name.");
            //    txeName.Focus();
            //}
            //else if (txeCity.Text.Trim() == "")
            //{
            //    FUNC.msgWarning("Please input city.");
            //    txeCity.Focus();
            //}
            //else if (slueCountry.Text.Trim() == "")
            //{
            //    FUNC.msgWarning("Please select country.");
            //    txeCity.Focus();
            //}
            //else
            //{
            //    bool chkGMP = chkDuplicateNo();
            //    if (chkGMP == true)
            //    {
            //        chkGMP = chkDuplicateName();
            //        if (chkGMP == true)
            //        {
            //            if (FUNC.msgQuiz("Confirm save data ?") == true)
            //            {
            //                StringBuilder sbSQL = new StringBuilder();
            //                string strCREATE = "0";
            //                if (txeCREATE.Text.Trim() != "")
            //                {
            //                    strCREATE = txeCREATE.Text.Trim();
            //                }

            //                if (lblStatus.Text == "* Add Port")
            //                {
            //                    sbSQL.Append("  INSERT INTO PortAndCity(PortCode, PortName, City, Country, CreatedBy, CreatedDate) ");
            //                    sbSQL.Append("  VALUES(N'" + txeCode.Text.Trim().Replace("'", "''") + "', N'" + txeName.Text.Trim().Replace("'", "''") + "', N'" + txeCity.Text.Trim().Replace("'", "''") + "', N'" + slueCountry.EditValue.ToString().Trim().Replace("'", "''") + "', '" + strCREATE + "', GETDATE()) ");
            //                }
            //                else if (lblStatus.Text == "* Edit Port")
            //                {
            //                    sbSQL.Append("  UPDATE PortAndCity SET ");
            //                    sbSQL.Append("      PortCode=N'" + txeCode.Text.Trim().Replace("'", "''") + "', ");
            //                    sbSQL.Append("      PortName=N'" + txeName.Text.Trim().Replace("'", "''") + "', ");
            //                    sbSQL.Append("      City=N'" + txeCity.Text.Trim().Replace("'", "''") + "', ");
            //                    sbSQL.Append("      Country=N'" + slueCountry.EditValue.ToString().Trim().Replace("'", "''") + "' ");
            //                    sbSQL.Append("  WHERE(OIDPORT = '" + txeID.Text.Trim() + "') ");
            //                }

            //                //MessageBox.Show(sbSQL.ToString());
            //                if (sbSQL.Length > 0)
            //                {
            //                    try
            //                    {
            //                        bool chkSAVE = new DBQuery(sbSQL).runSQL();
            //                        if (chkSAVE == true)
            //                        {
            //                            FUNC.msgInfo("Save complete.");
            //                            bbiNew.PerformClick();
            //                        }
            //                    }
            //                    catch (Exception)
            //                    { }
            //                }
            //            }
            //        }
            //    }
            //}
        }

        private void bbiExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //string pathFile = new ObjSet.Folder(@"C:\MDS\Export\").GetPath() + "PortAndCityList_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
            //gvPort.ExportToXlsx(pathFile);
            //System.Diagnostics.Process.Start(pathFile);
        }


        private void bbiPrintPreview_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //gcPort.ShowPrintPreview();
        }

        private void bbiPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //gcPort.Print();
        }

        private void txeCode_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //{
            //    txeName.Focus();
            //}
        }

        private void txeCode_Leave(object sender, EventArgs e)
        {
            //if (txeCode.Text.Trim() != "")
            //{
            //    txeCode.Text = txeCode.Text.ToUpper().Trim();
            //    bool chkDup = chkDuplicateNo();
            //    if (chkDup == true)
            //    {
            //        txeName.Focus();
            //    }
            //    else
            //    {
            //        txeCode.Text = "";
            //        txeCode.Focus();
            //        //FUNC.msgWarning("Duplicate code. !! Please Change.");

            //    }
            //}
        }

        private void txeName_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //{
            //    txeCity.Focus();
            //}
        }

        private void txeName_Leave(object sender, EventArgs e)
        {
            //if (txeName.Text.Trim() != "")
            //{
            //    txeName.Text = txeName.Text.ToUpper().Trim();
            //    bool chkDup = chkDuplicateName();
            //    if (chkDup == true)
            //    {
            //        txeCity.Focus();
            //    }
            //    else
            //    {
            //        txeName.Text = "";
            //        txeName.Focus();
            //        //FUNC.msgWarning("Duplicate name. !! Please Change.");

            //    }
            //}
        }

        private void txeCity_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //{
            //    slueCountry.Focus();
            //}
        }

        private void txeCity_Leave(object sender, EventArgs e)
        {

        }

        //private bool chkDuplicateNo()
        //{
        //    bool chkDup = true;
        //    if (txeCode.Text != "")
        //    {
        //        txeCode.Text = txeCode.Text.ToUpper().Trim();
        //        if (txeCode.Text.Trim() != "" && lblStatus.Text == "* Add Port")
        //        {
        //            StringBuilder sbSQL = new StringBuilder();
        //            sbSQL.Append("SELECT TOP(1) PortCode FROM PortAndCity WHERE (PortCode = N'" + txeCode.Text.Trim().Trim().Replace("'", "''") + "') ");
        //            if (new DBQuery(sbSQL).getString() != "")
        //            {
        //                txeCode.Text = "";
        //                txeCode.Focus();
        //                chkDup = false;
        //                FUNC.msgWarning("Duplicate code. !! Please Change.");
        //            }
        //        }
        //        else if (txeCode.Text.Trim() != "" && lblStatus.Text == "* Edit Port")
        //        {
        //            StringBuilder sbSQL = new StringBuilder();
        //            sbSQL.Append("SELECT TOP(1) OIDPORT ");
        //            sbSQL.Append("FROM PortAndCity ");
        //            sbSQL.Append("WHERE (PortCode = N'" + txeCode.Text.Trim().Trim().Replace("'", "''") + "') ");
        //            string strCHK = new DBQuery(sbSQL).getString();
        //            if (strCHK != "" && strCHK != txeID.Text.Trim())
        //            {
        //                txeCode.Text = "";
        //                txeCode.Focus();
        //                chkDup = false;
        //                FUNC.msgWarning("Duplicate code. !! Please Change.");
        //            }
        //        }
        //    }
        //    return chkDup;
        //}

        //private bool chkDuplicateName()
        //{
        //    bool chkDup = true;
        //    if (txeName.Text != "")
        //    {
        //        txeName.Text = txeName.Text.ToUpper().Trim();
        //        if (txeName.Text.Trim() != "" && lblStatus.Text == "* Add Port")
        //        {
        //            StringBuilder sbSQL = new StringBuilder();
        //            sbSQL.Append("SELECT TOP(1) PortName FROM PortAndCity WHERE (PortName = N'" + txeName.Text.Trim().Replace("'", "''") + "') ");
        //            if (new DBQuery(sbSQL).getString() != "")
        //            {
        //                txeName.Text = "";
        //                txeName.Focus();
        //                chkDup = false;
        //                FUNC.msgWarning("Duplicate name. !! Please Change.");
        //            }
        //        }
        //        else if (txeName.Text.Trim() != "" && lblStatus.Text == "* Edit Port")
        //        {
        //            StringBuilder sbSQL = new StringBuilder();
        //            sbSQL.Append("SELECT TOP(1) OIDPORT ");
        //            sbSQL.Append("FROM PortAndCity ");
        //            sbSQL.Append("WHERE (PortName = N'" + txeName.Text.Trim().Replace("'", "''") + "') ");
        //            string strCHK = new DBQuery(sbSQL).getString();
        //            if (strCHK != "" && strCHK != txeID.Text.Trim())
        //            {
        //                txeName.Text = "";
        //                txeName.Focus();
        //                chkDup = false;
        //                FUNC.msgWarning("Duplicate name. !! Please Change.");
        //            }
        //        }
        //    }
        //    return chkDup;
        //}

        //*********** REGION ************
        public class LocalesRetrievalException : Exception
        {
            public LocalesRetrievalException(string message)
                : base(message)
            {
            }
        }

        #region Windows API

        private delegate bool EnumLocalesProcExDelegate(
           [MarshalAs(UnmanagedType.LPWStr)] String lpLocaleString,
           LocaleType dwFlags, int lParam);

        [DllImport(@"kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool EnumSystemLocalesEx(EnumLocalesProcExDelegate pEnumProcEx,
           LocaleType dwFlags, int lParam, IntPtr lpReserved);

        private enum LocaleType : uint
        {
            LocaleAll = 0x00000000,             // Enumerate all named based locales
            LocaleWindows = 0x00000001,         // Shipped locales and/or replacements for them
            LocaleSupplemental = 0x00000002,    // Supplemental locales only
            LocaleAlternateSorts = 0x00000004,  // Alternate sort locales
            LocaleNeutralData = 0x00000010,     // Locales that are "neutral" (language only, region data is default)
            LocaleSpecificData = 0x00000020,    // Locales that contain language and region data
        }

        #endregion

        public enum CultureTypes : uint
        {
            SpecificCultures = LocaleType.LocaleSpecificData,
            NeutralCultures = LocaleType.LocaleNeutralData,
            AllCultures = LocaleType.LocaleWindows
        }

        public static List<CultureInfo> GetCultures(CultureTypes cultureTypes)
        {
            List<CultureInfo> cultures = new List<CultureInfo>();
            EnumLocalesProcExDelegate enumCallback = (locale, flags, lParam) =>
            {
                try
                {
                    cultures.Add(new CultureInfo(locale));
                }
                catch (CultureNotFoundException)
                {
                    // This culture is not supported by .NET (not happened so far)
                    // Must be ignored.
                }
                return true;
            };

            if (EnumSystemLocalesEx(enumCallback, (LocaleType)cultureTypes, 0, (IntPtr)0) == false)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new LocalesRetrievalException("Win32 error " + errorCode + " while trying to get the Windows locales");
            }

            // Add the two neutral cultures that Windows misses 
            // (CultureInfo.GetCultures adds them also):
            if (cultureTypes == CultureTypes.NeutralCultures || cultureTypes == CultureTypes.AllCultures)
            {
                cultures.Add(new CultureInfo("en-US"));
                //cultures.Add(new CultureInfo("zh-CHS"));
                //cultures.Add(new CultureInfo("zh-CHT"));
            }

            return cultures;
        }

        public static List<string> GetCountries()
        {
            List<CultureInfo> cultures = GetCultures(CultureTypes.SpecificCultures);
            List<string> countries = new List<string>();
           
            foreach (CultureInfo culture in cultures)
            {
                RegionInfo region = new RegionInfo(culture.Name);

                if (!(countries.Contains(region.EnglishName)))
                {
                    countries.Add(region.EnglishName);
                }
            }
            countries.Sort();
            return countries;
        }


        //*********** END-REGION ********

        private void slueCountry_Popup(object sender, EventArgs e)
        {
            //(sender as SearchLookUpEdit).Properties.View.Columns["Country"].SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;
        }

        private void gvPort_DoubleClick(object sender, EventArgs e)
        {
        //    GridView view = (GridView)sender;
        //    Point pt = view.GridControl.PointToClient(Control.MousePosition);
        //    DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo info = view.CalcHitInfo(pt);
        //    if (info.InRow || info.InRowCell)
        //    {
        //        DataTable dtCP = (DataTable)gcPort.DataSource;
        //        if (dtCP.Rows.Count > 0)
        //        {
        //            lblStatus.Text = "* Edit Port";
        //            lblStatus.ForeColor = Color.Red;

        //            DataRow drCP = dtCP.Rows[info.RowHandle];
        //            txeID.Text = drCP["OIDPORT"].ToString();
        //            txeCode.Text = drCP["PortCode"].ToString();
        //            txeName.Text = drCP["PortName"].ToString();
        //            txeCity.Text = drCP["City"].ToString();
        //            slueCountry.EditValue = drCP["Country"].ToString();

        //            txeCREATE.Text = drCP["CreatedBy"].ToString();
        //            txeDATE.Text = drCP["CreatedDate"].ToString();
        //        }
        //    }
        }

        private void tabbedControlGroup1_SelectedPageChanged(object sender, DevExpress.XtraLayout.LayoutTabPageChangedEventArgs e)
        {
            if (tabbedControlGroup1.SelectedTabPage == layoutControlGroup1)
            {
                bbiRefresh.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            }
            else
            {
                bbiRefresh.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }
        }

        private void bbiRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadSummary();
        }

        private void slueItemCode_EditValueChanged(object sender, EventArgs e)
        {
            if (slueItemCode.Text.Trim() != "")
            {
                GridView view = slueItemCode.Properties.View;
                txeItemName.Text = view.GetFocusedRowCellValue("ItemName").ToString();
                txeStyle.Text = view.GetFocusedRowCellValue("StyleNo.").ToString();
            }
            else
            {
                txeItemName.Text = "";
                txeStyle.Text = "";
            }
        }

        private void gvINPUT_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "OIDCSITEM")
            {
                if (gvINPUT.GetRowCellValue(e.RowHandle, "OIDCSITEM") != null)
                {
                    string strITEM = gvINPUT.GetRowCellValue(e.RowHandle, "OIDCSITEM").ToString();
                    StringBuilder sbSQL = new StringBuilder();
                    sbSQL.Append("SELECT ITC.ItemCode, ITC.ItemName, CUS.Name AS Customer, ITC.StyleNo AS [StyleNo.], ITC.Season, ITC.OIDCSITEM AS ID ");
                    sbSQL.Append("FROM   ItemCustomer AS ITC LEFT OUTER JOIN ");
                    sbSQL.Append("       Customer AS CUS ON ITC.OIDCUST = CUS.OIDCUST ");
                    sbSQL.Append("WHERE (ITC.ItemCode = '" + strITEM + "') ");
                    string[] arrITEM = new DBQuery(sbSQL).getMultipleValue();
                    if (arrITEM.Length > 0)
                    {
                        gvINPUT.SetRowCellValue(e.RowHandle, "ItemName", arrITEM[1]);
                        gvINPUT.SetRowCellValue(e.RowHandle, "StyleNo", arrITEM[3]);
                    }
                    else
                    {
                        gvINPUT.SetRowCellValue(e.RowHandle, "ItemName", "");
                        gvINPUT.SetRowCellValue(e.RowHandle, "StyleNo", "");
                    }
                }
            }
        }

        private void gvINPUT_ShownEditor(object sender, EventArgs e)
        {
            //GridView view = sender as GridView;
            //if (view.IsNewItemRow(view.FocusedRowHandle))
            //    view.ActiveEditor.IsModified = true;
        }

        private void repositoryItemSearchLookUpEdit2_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void bbiClone_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int FocusIndex = gvINPUT.FocusedRowHandle;
            gvINPUT.AddNewRow();
            gvINPUT.CloseEditor();
            gvINPUT.UpdateCurrentRow();

            int ind = 0;
            foreach (DevExpress.XtraGrid.Columns.GridColumn column in gvINPUT.Columns)
            {
                gvINPUT.SetRowCellValue(gvINPUT.RowCount-2, column, gvINPUT.GetRowCellValue(FocusIndex, column));
                ind++;
            }
        }

        private void gvINPUT_InitNewRow(object sender, InitNewRowEventArgs e)
        {

        }
    }
}