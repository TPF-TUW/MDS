﻿using System;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraGrid.Views.Grid;
using TheepClass;

namespace MPS07
{
    public partial class MPS07 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public MPS07()
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

        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void bbiExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
        }

        private void bbiPrintPreview_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void bbiPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

  
    }
}