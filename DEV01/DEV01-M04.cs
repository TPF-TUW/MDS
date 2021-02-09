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
    public partial class DEV01_M04 : DevExpress.XtraEditors.XtraForm
    {
        //Global Variable
        classConn db = new classConn();
        classTools ct = new classTools();
        SqlConnection mainConn = new classConn().MDS();
        SqlConnection conn;
        string sql = string.Empty;

        public DEV01_M04()
        {
            InitializeComponent();
        }

        private void chkNull(string Alert, TextEdit txtName)
        {
            ct.showWarningMessage("Please Key : "+Alert+"!"); txtName.Focus(); return;
        }

        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            string CustomerName = txtCustomerName.Text.ToString().Trim().Replace("'","''");
            string CustomerShortName = txtCustomerShortName.Text.ToString().Trim().Replace("'", "''");
            string CustomerCode = txtCustomerCode.Text.ToString().Trim().Replace("'", "''");

            if (CustomerName == "") { chkNull("CustomerName", txtCustomerName); }
            else if (CustomerShortName == "") { chkNull("CustomerShortName", txtCustomerShortName); }
            else if (CustomerCode == "") { chkNull("CustomerCode", txtCustomerCode); }
            else
            {
                //chkDup
                if (db.get("Select ShortName From Customer Where ShortName = '" + CustomerShortName + "' ", mainConn) == true)
                {
                    ct.showWarningMessage("CustomerShortName is Duplicate!"); txtCustomerShortName.Focus(); return;
                }
                else if (db.get("Select Code From Customer Where Code = '" + CustomerCode + "' ", mainConn) == true)
                {
                    ct.showWarningMessage("CustomerCode is Duplicate!"); txtCustomerCode.Focus(); return;
                }
                else
                {
                    if (ct.doConfirm("SAVE Customer?") == true)
                    {
                        sql = "Insert into Customer(Name,/**/ShortName,/**/Code,CustomerType) Values ('" + CustomerName + "','" + CustomerShortName + "','" + CustomerCode + "',9)";
                        Console.WriteLine(sql);
                        int i = db.Query(sql, mainConn);
                        if (i > 0)
                        {
                            DEV01 frmDev = Application.OpenForms.OfType<DEV01>().First();
                            db.getSl("Select OIDCUST,ShortName,Name From Customer", mainConn, frmDev.slCustomer_Main, "OIDCUST", "Name");
                            ct.showInfoMessage("Save Customer is Successfull.");
                            this.Close();
                        }
                    }
                }
            }
        }
    }
}