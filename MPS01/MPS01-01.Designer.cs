namespace MPS01
{
    partial class MPS01_01
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MPS01_01));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.slueCustomer = new DevExpress.XtraEditors.SearchLookUpEdit();
            this.searchLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.glueSeason = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.speSeason = new DevExpress.XtraEditors.SpinEdit();
            this.txeStyleNo = new DevExpress.XtraEditors.TextEdit();
            this.btnAddCustomer = new DevExpress.XtraEditors.SimpleButton();
            this.txeItemName = new DevExpress.XtraEditors.TextEdit();
            this.txeItemCode = new DevExpress.XtraEditors.TextEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.slueCustomer.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.glueSeason.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.speSeason.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txeStyleNo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txeItemName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txeItemCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.slueCustomer);
            this.layoutControl1.Controls.Add(this.glueSeason);
            this.layoutControl1.Controls.Add(this.speSeason);
            this.layoutControl1.Controls.Add(this.txeStyleNo);
            this.layoutControl1.Controls.Add(this.btnAddCustomer);
            this.layoutControl1.Controls.Add(this.txeItemName);
            this.layoutControl1.Controls.Add(this.txeItemCode);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(498, 126);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // slueCustomer
            // 
            this.slueCustomer.Location = new System.Drawing.Point(77, 4);
            this.slueCustomer.Name = "slueCustomer";
            this.slueCustomer.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.slueCustomer.Properties.NullText = "";
            this.slueCustomer.Properties.PopupView = this.searchLookUpEdit1View;
            this.slueCustomer.Size = new System.Drawing.Size(248, 20);
            this.slueCustomer.StyleController = this.layoutControl1;
            this.slueCustomer.TabIndex = 11;
            this.slueCustomer.EditValueChanged += new System.EventHandler(this.slueCustomer_EditValueChanged);
            // 
            // searchLookUpEdit1View
            // 
            this.searchLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.searchLookUpEdit1View.Name = "searchLookUpEdit1View";
            this.searchLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.searchLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // glueSeason
            // 
            this.glueSeason.Location = new System.Drawing.Point(138, 100);
            this.glueSeason.Name = "glueSeason";
            this.glueSeason.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.glueSeason.Properties.NullText = "";
            this.glueSeason.Properties.PopupView = this.gridLookUpEdit1View;
            this.glueSeason.Size = new System.Drawing.Size(52, 20);
            this.glueSeason.StyleController = this.layoutControl1;
            this.glueSeason.TabIndex = 10;
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // speSeason
            // 
            this.speSeason.EditValue = new decimal(new int[] {
            2020,
            0,
            0,
            0});
            this.speSeason.Location = new System.Drawing.Point(77, 100);
            this.speSeason.Name = "speSeason";
            this.speSeason.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.speSeason.Properties.MaxValue = new decimal(new int[] {
            2500,
            0,
            0,
            0});
            this.speSeason.Properties.MinValue = new decimal(new int[] {
            2020,
            0,
            0,
            0});
            this.speSeason.Size = new System.Drawing.Size(57, 20);
            this.speSeason.StyleController = this.layoutControl1;
            this.speSeason.TabIndex = 9;
            // 
            // txeStyleNo
            // 
            this.txeStyleNo.EditValue = "";
            this.txeStyleNo.Location = new System.Drawing.Point(77, 76);
            this.txeStyleNo.Name = "txeStyleNo";
            this.txeStyleNo.Properties.MaxLength = 10;
            this.txeStyleNo.Size = new System.Drawing.Size(113, 20);
            this.txeStyleNo.StyleController = this.layoutControl1;
            this.txeStyleNo.TabIndex = 8;
            this.txeStyleNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txeStyleNo_KeyDown);
            // 
            // btnAddCustomer
            // 
            this.btnAddCustomer.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnAddCustomer.ImageOptions.Image")));
            this.btnAddCustomer.Location = new System.Drawing.Point(454, 4);
            this.btnAddCustomer.Name = "btnAddCustomer";
            this.btnAddCustomer.Size = new System.Drawing.Size(40, 44);
            this.btnAddCustomer.StyleController = this.layoutControl1;
            this.btnAddCustomer.TabIndex = 7;
            this.btnAddCustomer.Click += new System.EventHandler(this.btnAddCustomer_Click);
            // 
            // txeItemName
            // 
            this.txeItemName.Location = new System.Drawing.Point(77, 52);
            this.txeItemName.Name = "txeItemName";
            this.txeItemName.Properties.MaxLength = 100;
            this.txeItemName.Size = new System.Drawing.Size(373, 20);
            this.txeItemName.StyleController = this.layoutControl1;
            this.txeItemName.TabIndex = 5;
            this.txeItemName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txeItemName_KeyDown);
            // 
            // txeItemCode
            // 
            this.txeItemCode.EditValue = "";
            this.txeItemCode.Location = new System.Drawing.Point(77, 28);
            this.txeItemCode.Name = "txeItemCode";
            this.txeItemCode.Properties.MaxLength = 10;
            this.txeItemCode.Size = new System.Drawing.Size(115, 20);
            this.txeItemCode.StyleController = this.layoutControl1;
            this.txeItemCode.TabIndex = 4;
            this.txeItemCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txeItemCode_KeyDown);
            this.txeItemCode.Leave += new System.EventHandler(this.txeItemCode_Leave);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.emptySpaceItem1,
            this.layoutControlItem5,
            this.emptySpaceItem2,
            this.emptySpaceItem3,
            this.layoutControlItem6,
            this.layoutControlItem3,
            this.layoutControlItem7,
            this.layoutControlItem4,
            this.emptySpaceItem4});
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            this.Root.Size = new System.Drawing.Size(498, 126);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.txeItemCode;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(192, 24);
            this.layoutControlItem1.Text = "Item Code";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(61, 14);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.txeItemName;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 48);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(450, 24);
            this.layoutControlItem2.Text = "Item Name";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(61, 14);
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(192, 24);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(258, 24);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.txeStyleNo;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 72);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(190, 24);
            this.layoutControlItem5.Text = "Style No.";
            this.layoutControlItem5.TextSize = new System.Drawing.Size(61, 14);
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(190, 72);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(260, 24);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.Location = new System.Drawing.Point(190, 96);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(260, 26);
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.speSeason;
            this.layoutControlItem6.Location = new System.Drawing.Point(0, 96);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(134, 26);
            this.layoutControlItem6.Text = "Season";
            this.layoutControlItem6.TextSize = new System.Drawing.Size(61, 14);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.glueSeason;
            this.layoutControlItem3.Location = new System.Drawing.Point(134, 96);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(56, 26);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.slueCustomer;
            this.layoutControlItem7.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(325, 24);
            this.layoutControlItem7.Text = "Customer";
            this.layoutControlItem7.TextSize = new System.Drawing.Size(61, 14);
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnAddCustomer;
            this.layoutControlItem4.Location = new System.Drawing.Point(450, 0);
            this.layoutControlItem4.MaxSize = new System.Drawing.Size(44, 48);
            this.layoutControlItem4.MinSize = new System.Drawing.Size(44, 48);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(44, 122);
            this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // emptySpaceItem4
            // 
            this.emptySpaceItem4.AllowHotTrack = false;
            this.emptySpaceItem4.Location = new System.Drawing.Point(325, 0);
            this.emptySpaceItem4.Name = "emptySpaceItem4";
            this.emptySpaceItem4.Size = new System.Drawing.Size(125, 24);
            this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
            // 
            // MPS01_01
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 126);
            this.Controls.Add(this.layoutControl1);
            this.Name = "MPS01_01";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Item Code";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MPS01_01_FormClosed);
            this.Load += new System.EventHandler(this.MPS01_01_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.slueCustomer.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.glueSeason.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.speSeason.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txeStyleNo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txeItemName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txeItemCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.TextEdit txeItemCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.SimpleButton btnAddCustomer;
        private DevExpress.XtraEditors.TextEdit txeItemName;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraEditors.TextEdit txeStyleNo;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraEditors.SpinEdit speSeason;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraEditors.SearchLookUpEdit slueCustomer;
        private DevExpress.XtraGrid.Views.Grid.GridView searchLookUpEdit1View;
        private DevExpress.XtraEditors.GridLookUpEdit glueSeason;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem4;
    }
}