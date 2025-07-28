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
        public ActionResult Index(string UserName, string Password, string Message)
        {
            ViewBag.InputUsername = UserName;
            ViewBag.InputPassword = Password;

            string connStr = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
            try {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        Message = "Open";
                    }
                    else
                    {
                        Message = "Not Open";
                    }

                    //conn.Open();


                    string query = "Select ID,	USERNAME,	PASSWORD from U_User Where USERNAME = @UserName AND PASSWORD = @Password ";
                    

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserName", UserName);
                    cmd.Parameters.AddWithValue("@Password", Password);

                    
                    
                    string version = conn.ServerVersion;
                    Console.WriteLine(Message);

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        //int userId = Convert.ToInt32(result);
                        //Session["Id"] = userId;
                        Session["UserName"] = UserName;
                        Session["token"] = Guid.NewGuid().ToString();

                     // return RedirectToAction("Index", "Home");
                        return View("~/Views/Home/Home.cshtml");

                    }
                    else
                    {
                        ViewBag.Error = "Error Occurred Due To Mismatch Of Username and Password";
                        return View();
                    }
                }
            }

            catch (Exception ex)
            {
                Message = " Connection Failed: " + ex.Message;
            }

            ViewBag.Status = Message;
            return RedirectToAction("Index","Home");

        }

    }
}



