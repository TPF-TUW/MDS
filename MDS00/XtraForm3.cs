using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using DevExpress.XtraBars.Navigation;
using DevExpress.LookAndFeel;
using TheepClass;

namespace MDS00
{
    public partial class XtraForm3 : DevExpress.XtraEditors.XtraForm
    {
        public XtraForm3()
        {
            InitializeComponent();
            var skinName = cUtility.LoadRegistry(@"Software\MDS", "SkinName");
            var skinPalette = cUtility.LoadRegistry(@"Software\MDS", "SkinPalette");
            UserLookAndFeel.Default.SetSkinStyle(skinName == null ? "Basic" : skinName.ToString(), skinPalette == null ? "Default" : skinPalette.ToString());
            accordionControl1.ElementClick += AccordionControl1_ElementClick;
        }

        private const int SW_MAXIMIZE = 3;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        private void CreateSplashScreen(string processName)
        {
            DevExpress.XtraSplashScreen.SplashScreenManager.ShowSkinSplashScreen(
                logoImage: null,
                title: "MDS",
                subtitle: "Merchandise and Development System",
                footer: "Copyright © 2020-2021 IT Integration Team",
                loading: "Starting..." + processName,
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
        private void RunProcess(string processName)
        {
            foreach (DevExpress.XtraBars.Ribbon.RibbonForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == processName)
                {
                    frmActive.Activate();
                    return;
                }
            }
            FileInfo f = new FileInfo(processName + ".dll");
            var a = Assembly.LoadFrom(f.FullName);
            var t = a.GetType(processName+"."+processName);
            DevExpress.XtraBars.Ribbon.RibbonForm frm = (DevExpress.XtraBars.Ribbon.RibbonForm)Activator.CreateInstance(t);
            frm.MdiParent = this;
            frm.WindowState = FormWindowState.Maximized;
            frm.Show();
        }

        private void AccordionControl1_ElementClick(object sender, ElementClickEventArgs e)
        {
            if (e.Element.Style == ElementStyle.Group) return;
            CreateSplashScreen(e.Element.Hint);
            try
            {
                RunProcess(e.Element.Hint);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            CloseSplashScreen();
        }

    }

    


}