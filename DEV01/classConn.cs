using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using MDS00;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DEV01
{
    public class classConn
    {
        IniFile ini = new IniFile("\\\\172.16.0.190\\MDS_Project\\MDS\\FileConfig\\Configue.ini");

        public string _server;
        public string _dbname;
        public string _user;
        public string _password;

        SqlConnection conn;
        SqlDataAdapter da;
        DataSet ds;
        SqlCommand cmd;
        SqlDataReader dr;
        DataTable dt;
        StringBuilder sql;

        public SqlConnection MDS()
        {
            _server = "172.16.0.30";
            _dbname = "MDS";
            _user = "sa";
            _password = "gik8nv@tpf";

            //string section = "ConnectionString";
            //_server = ini.Read("Server",section);
            //_dbname = ini.Read("Database", section);
            //_user = ini.Read("Uid", section);
            //_password = ini.Read("Pwd", section);

            return new SqlConnection("Data Source=" + _server + ";Initial Catalog=" + _dbname + ";Persist Security Info=True;User ID=" + _user + ";Password=" + _password + "");
        }

        public SqlConnection DellInspiron15()
        {
            _server = "S410717NB0201\\MSSQLSERVER2";
            _dbname = "GSS_Test";
            _user = "sa";
            _password = "gik8nv@tpf";
            return new SqlConnection("Data Source=" + _server + ";Initial Catalog=" + _dbname + ";Persist Security Info=True;User ID=" + _user + ";Password=" + _password + "");
        }

        // getData to GridControl
        public void getGc(StringBuilder sql, GridControl dgvName, SqlConnection conn)
        {
            cmd = new SqlCommand(sql.ToString(), conn);
            conn.Open();
            da = new SqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            conn.Close();
            dgvName.DataSource = dt;
        }

        // getData to GridView
        public void getDgv(string sql, GridControl dgvName, SqlConnection conn)
        {
            cmd = new SqlCommand(sql, conn);
            conn.Open();
            da = new SqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            conn.Close();
            dgvName.DataSource = dt;
        }

        // get Data to LookupEdit
        public void getGl(string sql, SqlConnection conn, GridLookUpEdit glName, string valName, string displayName)
        {
            cmd = new SqlCommand(sql, conn);
            conn.Open();
            da = new SqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            conn.Close();
            glName.Properties.DataSource = dt;
            glName.Properties.DisplayMember = displayName;
            glName.Properties.ValueMember = valName;
        }

        // get Data to Repo_LookupEdit
        public void get_repGl(string sql, SqlConnection conn, RepositoryItemGridLookUpEdit glName, string valName, string displayName)
        {
            cmd = new SqlCommand(sql, conn);
            conn.Open();
            da = new SqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            conn.Close();
            glName.DataSource = dt;
            glName.DisplayMember = displayName;
            glName.ValueMember = valName;
        }

        // get Data to SearchLookupEdit
        public void getSl(string sql, SqlConnection conn, SearchLookUpEdit slName, string valName, string displayName)
        {
            cmd = new SqlCommand(sql, conn);
            conn.Open();
            da = new SqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            conn.Close();
            slName.Properties.DataSource = dt;
            slName.Properties.DisplayMember = displayName;
            slName.Properties.ValueMember = valName;
        }

        // Load Data to ComboBox
        //public void getCbo(string sql, ComboBox cboName, SqlConnection conn)
        //{
        //    cboName.Items.Clear();
        //    //conn = new dbConn().GSSv2_Prod();
        //    cmd = new SqlCommand(sql, conn);
        //    cmd.CommandText = sql;
        //    conn.Open();
        //    dr = cmd.ExecuteReader();
        //    while (dr.Read())
        //    {
        //        cboName.Items.Add(dr[0].ToString());
        //    }
        //    conn.Close();
        //}

        // dbQuery Select : Check True/False
        public bool get(string sql, SqlConnection conn)
        {
            bool b = false;
            cmd = new SqlCommand(sql, conn);
            conn.Open();
            dr = cmd.ExecuteReader();
            if (dr.Read() == true)
            {
                b = true;
            }
            cmd.Dispose();
            conn.Close();
            return b;
        }

        // Select One Columns
        public string get_oneParameter(string sql, SqlConnection conn, string colName)
        {
            string rs = string.Empty;
            cmd = new SqlCommand(sql, conn);
            conn.Open();
            dr = cmd.ExecuteReader();
            if (dr.Read() == true)
            {
                rs = dr[colName].ToString();
            }
            dr.Close();
            cmd.Dispose();
            conn.Close();
            return rs;
        }

        // Select One Columns
        public string getsb_oneParameter(StringBuilder sql, SqlConnection conn, string colName)
        {
            string rs = string.Empty;
            cmd = new SqlCommand(sql.ToString(), conn);
            conn.Open();
            dr = cmd.ExecuteReader();
            if (dr.Read() == true)
            {
                rs = dr[colName].ToString();
            }
            dr.Close();
            cmd.Dispose();
            conn.Close();
            return rs;
        }

        // dbQuery Insert/Update/Delete
        public int Query(string sql, SqlConnection conn)
        {
            int i;
            cmd = new SqlCommand(sql, conn);
            conn.Open();
            cmd.CommandType = CommandType.Text;
            i = cmd.ExecuteNonQuery();
            conn.Close();
            return i;
        }

        /*Main Repo :: SMPLQuantiityRequired*/
        public class FGRequest
        {
            public FGRequest() { }
            public FGRequest(Int32 no, string color, string size, Int32 quantity, string unit)
            {
                No = no; Color = color; Size = size; Quantity = quantity; Unit = unit;
            }
            public Int32 No { get; set; }
            public string Color { get; set; }
            public string Size { get; set; }
            public Int32 Quantity { get; set; }
            public string Unit { get; set; }
        }
        //Create DataSourse
        public BindingList<FGRequest> FGRequestDS()
        {
            BindingList<FGRequest> ds = new BindingList<FGRequest>();
            //ds.Add(new FGRequest(1, "Black", "XL", 10));
            //ds.AllowNew = true;
            return ds;
        }

        /*Mat Repo :: G7*/
        public class MatRequest
        {
            public MatRequest() { }
            public MatRequest(Int32 no, string color, string size, string consumption, string unit, string smplID)
            {
                No = no; Color = color; Size = size; Consumption = consumption; Unit = unit; SmplID = smplID;
            }
            public Int32 No { get; set; }
            public string Color { get; set; }
            public string Size { get; set; }
            public string Consumption { get; set; }
            public string Unit { get; set; }
            public string SmplID { get; set; }
        }
        // ds Mat
        public BindingList<MatRequest> dsMat()
        {
            BindingList<MatRequest> ds = new BindingList<MatRequest>();
            return ds;
        }

        public string getDataFrom_SMPL(string fieldName, string OID)
        {
            string s = string.Empty;
            s = get_oneParameter("Select " + fieldName + " From SMPLRequest Where OIDSMPL = " + OID + " ", MDS(), fieldName);
            return s.Trim();
        }

        /* --------------------------------------------------------------------------------- Special Query This Project Only ------------------------------------------------------------------------- */
        public string get_newOIDMat()
        {
            string sql          = "SELECT CASE WHEN ISNULL(MAX(OIDSMPLMT), '') = '' THEN 1 ELSE MAX(OIDSMPLMT) + 1 END AS newOIDMat FROM SMPLRequestMaterial";
            string newOIDMat    = get_oneParameter(sql, MDS(), "newOIDMat");
            return newOIDMat;
        }

        public void getGrid_SMPL(GridControl glName)
        {
            sql = new StringBuilder();
            sql.Append("SELECT OIDSMPL,smpl.Status, SMPLNo,SMPLRevise/*,ReferenceNo,ContactName,ModelName,Situation,StateArrangements*/ ,b.Name as Branch ,d.Name as SaleSection ,RequestDate, SpecificationSize, Season ,/*c.ShortName as cusShortName,*/c.Name as Customer, UseFor ,g.CategoryName as Category ,p.StyleName as Style,SMPLItem, SMPLPatternNo, PatternSizeZone, CustApproved FROM SMPLRequest smpl left join Branchs b on b.OIDBranch = smpl.OIDBranch left join Departments d on d.OIDDEPT = smpl.OIDDEPT left join Customer c on c.OIDCUST = smpl.OIDCUST left join GarmentCategory g on g.OIDGCATEGORY = smpl.OIDCATEGORY left join ProductStyle p on p.OIDSTYLE = smpl.OIDSTYLE Order By Status");
            getGc(sql, glName,MDS());
        }
        public void getGrid_QuantityReq(GridControl glName)
        {
            /*อันนี้ลบทิ้งได้เลยนะ*/
            sql = new StringBuilder();
            sql.Append("SELECT ROW_NUMBER() OVER(order by OIDSMPLDT asc) as No, SMPLQuantityRequired.OIDSMPLDT, SMPLQuantityRequired.OIDSMPL, SMPLRequest.SMPLNo, SMPLRequest.SMPLRevise, SMPLRequest.SMPLItem, SMPLRequest.SMPLPatternNo, SMPLRequest.PatternSizeZone,ProductColor.ColorNo, ProductColor.ColorName, ProductSize.SizeNo, ProductSize.SizeName, SMPLQuantityRequired.Quantity, Unit.UnitName FROM SMPLQuantityRequired INNER JOIN SMPLRequest ON SMPLQuantityRequired.OIDSMPL = SMPLRequest.OIDSMPL INNER JOIN ProductColor ON SMPLQuantityRequired.OIDCOLOR = ProductColor.OIDCOLOR INNER JOIN ProductSize ON SMPLQuantityRequired.OIDSIZE = ProductSize.OIDSIZE INNER JOIN Unit ON SMPLQuantityRequired.OIDUnit = Unit.OIDUNIT/*Where*/Order by SMPLQuantityRequired.OIDSMPLDT");
            getGc(sql, glName, MDS());
        }
        public void getGrid_FBListSample(GridControl gcName,string Where)
        {
            sql = new StringBuilder();
            sql.Append("SELECT smplQR.OIDSMPL, smpl.SMPLPatternNo,'' as Consumption, c.ColorName, s.SizeName, smplQR.Quantity,u.UnitName,smplQR.OIDSMPLDT,s.OIDSIZE,u.OIDUNIT FROM SMPLRequest smpl INNER JOIN SMPLQuantityRequired smplQR ON smpl.OIDSMPL = smplQR.OIDSMPL INNER JOIN ProductColor c ON smplQR.OIDCOLOR = c.OIDCOLOR INNER JOIN ProductSize s ON smplQR.OIDSIZE = s.OIDSIZE INNER JOIN Unit u ON smplQR.OIDUnit = u.OIDUNIT Where smplQR.OIDSMPL is not null " + Where + " Order By smplQR.OIDSMPL,smpl.SMPLPatternNo,c.ColorName");
            getGc(sql, gcName,MDS());
        }
        public string genSMPLNo()
        {
            string SMPLNo = string.Empty;
            sql = new StringBuilder();
            sql.Append("Select SUBSTRING(Season,1,4)+'S'+cast(OIDDEPT as nvarchar(10))+SUBSTRING( /*string*/'0000'+cast(SUBSTRING(SMPLNo,7,4)+1 as nvarchar(max)) ,/*start*/LEN('0000'+cast(SUBSTRING(SMPLNo,7,4)+1 as nvarchar(max)))-3 ,/*length*/4)+'-0'/*+cast(SUBSTRING(SMPLNo,12,1)+1 as nvarchar(max))*/ as SMPLNo From SMPLRequest Where OIDSMPL =(Select MAX(OIDSMPL) From SMPLRequest)");
            SMPLNo = getsb_oneParameter(sql,MDS(), "SMPLNo");
            return SMPLNo;
        }

        // Tab : Fabric
        public string get_newOIDFB()
        {
            string sql = "SELECT CASE WHEN ISNULL(MAX(OIDSMPLFB), '') = '' THEN 1 ELSE MAX(OIDSMPLFB) + 1 END AS newOIDFB FROM SMPLRequestFabric";
            string newOIDFB = get_oneParameter(sql, MDS(), "newOIDFB");
            return newOIDFB;
        }
        public void getListofFabric(GridControl gc, string OIDSMPL)
        {
            string sqlFB = "Select fb.OIDSMPLFB as No , VendFBCode,fb.Composition,FBWeight,c.ColorName as ColorName,SMPLotNo,v.Name as Supplier,i.Code as NAVCode From SMPLRequestFabric fb inner join SMPLQuantityRequired q on q.OIDSMPLDT = fb.OIDSMPLDT inner join SMPLRequest smpl on smpl.OIDSMPL = q.OIDSMPL inner join ProductColor c on c.OIDCOLOR = fb.OIDCOLOR inner join Items i on i.OIDITEM = fb.OIDITEM inner join Vendor v on v.OIDVEND = fb.OIDVEND Where smpl.OIDSMPL = "+OIDSMPL+" ";
            getDgv(sqlFB,gc,MDS());
        }

        // Tab : Material
        public void getListofMaterial(GridControl gc,string dosetOIDSMPL)
        {
            string sqlMat = "Select m.OIDSMPLMT as MatID,q.OIDSMPL as SampleID,d.Name as WorkStation,VendMTCode,SMPLotNo,v.Name as Vendor,c.ColorName as MatColor,s.SizeName as MatSize,m.Composition,Details,Price,cr.Currency as Currency,i.Code as NAVCode,m.Situation,Comment,Remark,m.PathFile ,Consumption/*,m.OIDUNIT */ From SMPLRequestMaterial m inner join SMPLQuantityRequired q on q.OIDSMPLDT = m.OIDSMPLDT inner join Departments d on d.OIDDEPT = m.OIDDEPT inner join Vendor v on v.OIDVEND = m.OIDVEND left join ProductColor c on c.OIDCOLOR = m.MTColor inner join ProductSize s on s.OIDSIZE = m.MTSize left join Currency cr on cr.OIDCURR = m.OIDCURR left join Items i on i.OIDITEM = m.OIDITEM Where q.OIDSMPL = " + dosetOIDSMPL + " ";
            getDgv(sqlMat, gc, MDS());
        }
    }
}
