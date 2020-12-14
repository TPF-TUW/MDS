using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Data.SqlClient;

namespace DEV01
{
    public partial class DEV01_M11 : DevExpress.XtraEditors.XtraForm
    {
        //Global Var
        classConn db = new classConn();
        classTools ct = new classTools();
        SqlConnection mainConn = new classConn().MDS();
        string sql = string.Empty;

        public DEV01_M11()
        {
            InitializeComponent();
        }

        private void DEV01_M11_Load(object sender, EventArgs e)
        {
            //get CategoryName to glCategoryName From GamentCategory.CategoryName
            db.getGl("Select OIDGCATEGORY,CategoryName From GarmentCategory",mainConn,glCategoryName, "OIDGCATEGORY", "CategoryName");
        }

        private void btnAddStyle_Click(object sender, EventArgs e)
        {
            string StyleName = txtStyleName.Text.ToString().Trim().Replace("'", "''");
            string CategoryName = glCategoryName.Text.ToString();

            //chkNull or Empty
            if (StyleName == "")
            {
                ct.showWarningMessage("Please Key StyleName!"); txtStyleName.Focus(); return;
            }
            else if (CategoryName == "")
            {
                ct.showWarningMessage("Please Select CategoryName!"); glCategoryName.Focus(); return;
            }
            else
            {
                //chkDup
                if (db.get("Select StyleName From ProductStyle Where StyleName = '" + StyleName + "' ", mainConn) == true)
                {
                    ct.showWarningMessage("StyleName is Duplicate!"); txtStyleName.Focus(); return;
                }
                else
                {
                    //Confirm Save
                    if (ct.doConfirm("Save StyleName ? ") == true)
                    {
                        sql = "Insert Into ProductStyle (StyleName,OIDGCATEGORY) Values('" + StyleName + "',"+ glCategoryName.EditValue.ToString() + ")";
                        Console.WriteLine(sql);
                        int i = db.Query(sql, mainConn);
                        if (i > 0)
                        {
                            ct.showInfoMessage("Save StyleName is Successufull.");
                            this.Close();
                        }
                    }
                }
            }
        }
    }
}