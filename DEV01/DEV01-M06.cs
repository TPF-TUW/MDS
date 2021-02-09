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
    public partial class DEV01_M06 : DevExpress.XtraEditors.XtraForm
    {
        classConn db = new classConn();
        classTools ct = new classTools();
        string sql = string.Empty;
        SqlConnection mainConn = new classConn().MDS();

        public DEV01_M06()
        {
            InitializeComponent();
        }

        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            string CategoryName = txtCategoryName.Text.ToString().Trim().Replace("'","''");
            
            //chkNull or Empty
            if (CategoryName == "")
            {
                ct.showWarningMessage("Please Key CategoryName!"); txtCategoryName.Focus(); return;
            }
            else
            {
                //chkDup
                if (db.get("Select CategoryName From GarmentCategory Where CategoryName = '" + CategoryName + "' ", mainConn) == true)
                {
                    ct.showWarningMessage("CategroryName is Duplicate!"); txtCategoryName.Focus(); return;
                }
                else
                {
                    //Confirm Save
                    if (ct.doConfirm("Save CategoryName ? ")==true)
                    {
                        sql = "Insert Into GarmentCategory (CategoryName) Values('"+ CategoryName + "')";
                        Console.WriteLine(sql);
                        int i = db.Query(sql,mainConn);
                        if (i > 0)
                        {
                            DEV01 frmDev = Application.OpenForms.OfType<DEV01>().First();
                            db.getGl("Select OIDGCATEGORY,CategoryName from GarmentCategory", mainConn, frmDev.glCategoryDivision_Main, "OIDGCATEGORY", "CategoryName");

                            ct.showInfoMessage("Save CategoryName is Successufull.");
                            this.Close();
                        }
                    }
                }
            }
        }
    }
}