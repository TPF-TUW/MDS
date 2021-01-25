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
using DBConnection;

namespace M11
{
    public partial class M11_01 : DevExpress.XtraEditors.XtraForm
    {
        private Functionality.Function FUNC = new Functionality.Function();

        public M11_01()
        {
            InitializeComponent();
        }

        private void M11_01_Load(object sender, EventArgs e)
        {
        }

        private bool chkDuplicate()
        {
            bool chkDup = true;
            txeCategoryName.Text = txeCategoryName.Text.Trim();
            if (txeCategoryName.Text != "")
            {
                string StyleName = txeCategoryName.Text.ToString().Trim().Replace("'", "''");
                StringBuilder sbSQL = new StringBuilder();
                sbSQL.Append("SELECT OIDGCATEGORY FROM GarmentCategory WHERE (CategoryName = N'" + StyleName + "') ");
                if (new DBQuery(sbSQL).getString() != "")
                {
                    chkDup = false;
                }
            }
            return chkDup;
        }

        private void btnAddStyle_Click(object sender, EventArgs e)
        {
            string StyleName = txeCategoryName.Text.ToString().Trim().Replace("'", "''");
            //chkNull or Empty
            if (StyleName == "")
            {
                FUNC.msgWarning("Please input category name.");
                txeCategoryName.Focus();
            }
            else
            {

                bool chkDup = chkDuplicate();
                if (chkDup == true)
                {
                    if (FUNC.msgQuiz("Confirm save data ?") == true)
                    {
                        //string strCREATE = txeCREATE.Text.Trim() != "" ? txeCREATE.Text.Trim() : "0";
                        string strCREATE = "0";

                        StringBuilder sbSQL = new StringBuilder();
                        sbSQL.Append(" INSERT INTO GarmentCategory(CategoryName, CreatedBy, CreatedDate) ");
                        sbSQL.Append("  VALUES(N'" + StyleName + "', '" + strCREATE + "', GETDATE()) ");
                        try
                        {
                            bool chkSAVE = new DBQuery(sbSQL).runSQL();
                            if (chkSAVE == true)
                            {
                                if (Application.OpenForms.OfType<M11>().Count() > 0)
                                {
                                    M11 frmStyle = Application.OpenForms.OfType<M11>().First();
                                    sbSQL.Clear();
                                    sbSQL.Append("SELECT CategoryName, OIDGCATEGORY AS ID ");
                                    sbSQL.Append("FROM GarmentCategory ");
                                    sbSQL.Append("ORDER BY CategoryName ");
                                    new ObjDevEx.setGridLookUpEdit(frmStyle.glueCategory, sbSQL, "CategoryName", "ID").getData(true);
                                }
                                FUNC.msgInfo("Save complete.");
                                this.Close();
                            }
                        }
                        catch (Exception)
                        { }
                    }
                }
                else
                {
                    txeCategoryName.Text = "";
                    txeCategoryName.Focus();
                    FUNC.msgWarning("Duplicate category name. !! Please Change.");
                }
            }
        }
    }
}