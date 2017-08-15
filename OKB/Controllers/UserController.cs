using OKB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OKB.EF;
using System.Data;
using System.Data.OleDb;
using System.Xml;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.IO;
using ExpertXls.ExcelLib;

namespace OKB.Controllers
{
    public class UserController : Controller
    {
        OKBEntities1 db = new OKBEntities1();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();

        }
        [HttpPost]
        public ActionResult Login(ULogin login)
        {
            using (var db = new OKBEntities1())
            {

                List<Login> lg = (from p in db.Logins where p.UserEmail == login.UserEmail && p.Password == login.Password select p).ToList();
                foreach (var professor in lg)
                {
                    List<Role> role = (from R in db.Roles
                                       where R.RoleId == professor.RoleId select R).ToList();
                    Session["RoleName"] = role.FirstOrDefault().RoleName;
                    Session["RoleName"] = Session["RoleName"] + "," + role.FirstOrDefault().RoleName;
                }
                if ((lg.ToList().Count > 0))
                {
                    Session["User"] = lg.FirstOrDefault().UserEmail;
                    
   Session["User"] = Session["User"] + "," + lg.FirstOrDefault().RoleId;
                    Session["FirstName"] = lg.FirstOrDefault().FirstName;
                    Session["FirstName"] = Session["FirstName"] + "," + lg.FirstOrDefault().FirstName;
                    return RedirectToAction("Admin", "User");
                }
            }
            return View();

        }

        public ActionResult Admin()
        {
            return View();

        }
        public ActionResult DashBoard()
        {

            ViewBag.GetData = db.Registrations.ToList();
            return View();

        }

        [HttpPost]
        public ActionResult Search(Search searchdata)
        {
        //List<Search> loginUsr=db.Logins.Where(u=>u.FirstName==searchdata.Value)
                   List<Login> role = (from R in db.Logins
                                      where R.FirstName == searchdata.Value
                                      select R).ToList();
            return RedirectToAction("Admin", "User");
        }
        public ActionResult SignIn()
        {

            return View();
        }
        [HttpPost]
        public ActionResult SignIn(Registration reg)
        {
            var data=db.Registrations.Add(reg);
            db.SaveChanges();

            return RedirectToAction("Login", "User");
        }
        [HttpPost]
        public ActionResult GetAllDetails()
        {
            ViewBag.GetData = db.Registrations.ToList();
            return View();

        }

        public ActionResult GetAllRegisteredUser()
        {
            return View(db.Registrations.ToList());

        }
        [HttpPost]
        public ActionResult ExcelUpload(HttpPostedFileBase file)
        {
            
            DataSet ds = new DataSet();
            DataTable uploaddt = new DataTable();
            DataTable dtUploadProfile = new DataTable();
            String actualFileName = file.FileName;
            
            if (Request.Files["file"].ContentLength > 0)
            {
                string fileExtension =
 System.IO.Path.GetExtension(Request.Files["file"].FileName);

                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {
                    string fileLocation = Server.MapPath("~/Content/") + Request.Files["file"].FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {

                        System.IO.File.Delete(fileLocation);
                    }
                    Request.Files["file"].SaveAs(fileLocation);
                    string excelConnectionString = string.Empty;
                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                    fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    //connection String for xls file format.
                    if (fileExtension == ".xls")
                    {
                        excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    }
                    //connection String for xlsx file format.
                    else if (fileExtension == ".xlsx")
                    {
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }
                    //Create Connection to Excel work book and add oledb namespace
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    excelConnection.Open();
                    DataTable dt = new DataTable();

                    dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dt == null)
                    {
                        return null;
                    }

                    String[] excelSheets = new String[dt.Rows.Count];
                    int t = 0;
                    //excel data saves in temp file here.
                    foreach (DataRow row in dt.Rows)
                    {
                        excelSheets[t] = row["TABLE_NAME"].ToString();
                        t++;
                    }
                    OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                    string query = string.Format("Select * from [{0}]", excelSheets[0]);
                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                    {
                        dataAdapter.Fill(ds);
                    }
                }
                if (fileExtension.ToString().ToLower().Equals(".xml"))
                {
                    string fileLocation = Server.MapPath("~/Content/") + Request.Files["FileUpload"].FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {
                        System.IO.File.Delete(fileLocation);
                    }

                    Request.Files["FileUpload"].SaveAs(fileLocation);
                    XmlTextReader xmlreader = new XmlTextReader(fileLocation);
                    // DataSet ds = new DataSet();
                    ds.ReadXml(xmlreader);
                    xmlreader.Close();
                }
                String filePath = String.Empty;
                string fileName=file.FileName;
                string targetpath = Server.MapPath("");
                file.SaveAs(targetpath + fileName);
                filePath = ConfigurationManager.AppSettings["GetFileFolder"] + "\\" + file.FileName;
                string pathToExcelFile = targetpath + fileName;

                FileStream sourceXlsDataStream = new System.IO.FileStream(filePath, FileMode.Open);
                ExcelWorkbook tempWorkbook = new ExcelWorkbook(sourceXlsDataStream);
                tempWorkbook.LicenseKey = ConfigurationManager.AppSettings["ExportExcelFileKey"];
                ExcelWorksheet tempWorksheet = tempWorkbook.Worksheets[0];
                //StringBuilder errorMSG = new StringBuilder();

                // get the data from the used range of the temporary workbook to a .NET DataTable object
                DataTable exportedDataTable = tempWorksheet.GetDataTable(tempWorksheet.UsedRange, true, false, true);

                if (exportedDataTable.Rows.Count == 0)
                {
                }


                dtUploadProfile = exportedDataTable;

                if (dtUploadProfile.Columns[0].ColumnName != "Name" || dtUploadProfile.Columns[1].ColumnName != "UserName" || dtUploadProfile.Columns[2].ColumnName != "Email")
                {


                }








                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //string conn = ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString;
                    //SqlConnection con = new SqlConnection(conn);
                    SqlConnection con = new SqlConnection("Data Source = PIYUSH1; Initial Catalog = OKB;Integrated security=true");
                    string query = "Insert into Registration(Name,UserName,Email) Values('" +
                    ds.Tables[0].Rows[i][0].ToString() + "','" + ds.Tables[0].Rows[i][1].ToString() +
                    "','" + ds.Tables[0].Rows[i][2].ToString() + "')";
                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            return RedirectToAction("GetAllRegisteredUser");
        }
        //[ChildActionOnly]
        public ActionResult Load()
        {

            return View();
        }
        
        public ActionResult GetDataByID(int id)
        {

            return View();
        }
        private DataTable CreateDTfromExcel(HttpPostedFile file)
        {
            StringBuilder errorMSG = new StringBuilder();
            String fileName = String.Empty;
            String filePath = String.Empty;
            //DataTable excelDataTableWithError = new DataTable(); 



            filePath = ConfigurationManager.AppSettings["GetFileFolder"] + "\\" + file.FileName;
            file.SaveAs(filePath);

            //filePath += fileName;

            FileStream sourceXlsDataStream = new System.IO.FileStream(filePath, FileMode.Open);
            ExcelWorkbook tempWorkbook = new ExcelWorkbook(sourceXlsDataStream);
            tempWorkbook.LicenseKey = ConfigurationManager.AppSettings["ExportExcelFileKey"];
            ExcelWorksheet tempWorksheet = tempWorkbook.Worksheets[0];
            //StringBuilder errorMSG = new StringBuilder();

            // get the data from the used range of the temporary workbook to a .NET DataTable object
            DataTable exportedDataTable = tempWorksheet.GetDataTable(tempWorksheet.UsedRange, true, false, true);

            if (exportedDataTable.Rows.Count == 0)
            {
            }

            tempWorkbook.Close();
            sourceXlsDataStream.Close();
            return exportedDataTable;

        }


    }
}