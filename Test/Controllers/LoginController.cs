using System;
using System.Web.Mvc;
using System.Configuration;
using System.Data.SqlClient;

namespace Test.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string UserName, string Password)
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT Id FROM U_User WHERE UserName = @UserName AND Password = @Password";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserName", UserName);
                cmd.Parameters.AddWithValue("@Password", Password);

                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    int userId = Convert.ToInt32(result);
                    Session["Id"] = userId;
                    Session["UserName"] = UserName;
                    Session["token"] = Guid.NewGuid().ToString();

                    return RedirectToAction("Dashboard", "Home");
                }
                else
                {
                    ViewBag.Error = "Error Occurred Due To Mismatch Of Username and Password";
                    return View();
                }
            }
        }
    }
}
