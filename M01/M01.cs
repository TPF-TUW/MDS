using System;
using System.Text;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Extensions;
using DBConnection;
using MDS00;
using System.Drawing;
using System.Data;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;

namespace M01
{
    public partial class M01 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        //private Functionality.Function FUNC = new Functionality.Function();
        public M01()
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
                logoImage:null,
                title:"MDS",
                subtitle:"Merchandise and Development System",
                footer: "Copyright © 2020-2021 IT Integration Team",
                loading:"Starting...",
                parentForm:this,
                useFadeIn:true,
                useFadeOut:true,
                throwExceptionIfAlreadyOpened:true,
                startPos: DevExpress.XtraSplashScreen.SplashFormStartPosition.Default,
                location:default
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
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("SELECT FL.FunctionNo, FL.FunctionName, VH.VersionNo ");
            sbSQL.Append("FROM VersionHistory AS VH INNER JOIN ");
            sbSQL.Append("     FunctionList AS FL ON VH.OIDFunction = FL.OIDFunction ");
            sbSQL.Append("WHERE (VH.ActiveType = 1) ");
            sbSQL.Append("ORDER BY FL.FunctionNo ");
            new ObjDevEx.setGridControl(gcAbout, gvAbout, sbSQL).getData(false, false, true, true);
            CloseSplashScreen();
        }
    }
}