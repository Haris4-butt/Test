using System;
using System.Web.Mvc;
using System.Configuration;
using System.Data.SqlClient;

namespace Test.Controllers
{
    public class LoginController : Controller
    {
        
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string UserName, string Password, string Message , string Status, string USER_IP)
        {
            ViewBag.InputUsername = UserName;
            ViewBag.InputPassword = Password;

            string connStr = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
            try
            {
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
                        Session["Password"] = Password;
                        Session["token"] = Guid.NewGuid().ToString();
                        Status = "A";
                        USER_IP = "192.168.7.16";


                        //Insert Query For Login_logs
                        string insertquery = "INSERT INTO LOGIN_LOGS(SESSION_ID,USER_LOGIN,LOGIN_PASSWORD,STATUS, USER_IP ) Values(@SessionId,@User_Login,@Login_Password,@Status,@User_IP)";

                        using (SqlCommand cmd1 = new SqlCommand(insertquery, conn))
                        {
                            cmd1.Parameters.AddWithValue("@SessionID", Session["token"].ToString());
                            cmd1.Parameters.AddWithValue("@User_Login", UserName);
                            cmd1.Parameters.AddWithValue("@Login_Password", Password);
                            cmd1.Parameters.AddWithValue("@Status", Status);
                            cmd1.Parameters.AddWithValue("@User_IP", USER_IP);

                            cmd1.ExecuteNonQuery();

                        }


                        return View("~/Views/Home/Index.cshtml");


                    }
                    else
                    {
                        ViewBag.Error = "Error Occurred Due To Mismatch Of Username and Password";
                        Status = "F";
                        USER_IP = "192.168";
                        string insertquery = "INSERT INTO LOGIN_LOGS(USER_LOGIN,LOGIN_PASSWORD,STATUS, USER_IP ) Values(@User_Login,@Login_Password,@Status,@User_IP)";

                        using (SqlCommand cmd2 = new SqlCommand(insertquery, conn))
                        {
                            // cmd1.Parameters.AddWithValue("@SessionID", Session["token"].ToString());
                            cmd2.Parameters.AddWithValue("@User_Login", UserName);
                            cmd2.Parameters.AddWithValue("@Login_Password", Password);
                            cmd2.Parameters.AddWithValue("@Status", Status);
                            cmd2.Parameters.AddWithValue("@User_IP", USER_IP);

                            cmd2.ExecuteNonQuery();
                

                        }
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                Message = " Connection Failed: " + ex.Message;
                return View();
            }  

        }

        [HttpGet]
        public ActionResult Logout(string UserName, string Password,  string Status, string USER_IP)
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
            string sessionID = Session["token"]?.ToString();
            Status = "I";
            if (!string.IsNullOrEmpty(sessionID))
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string updateQuery = "INSERT INTO LOGIN_LOGS(SESSION_ID,USER_LOGIN,LOGIN_PASSWORD,STATUS, USER_IP ) Values(@SessionId,@User_Login,@Login_Password,@Status,@User_IP)";
                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionID", sessionID);
                        cmd.Parameters.AddWithValue("@User_Login", UserName);
                        cmd.Parameters.AddWithValue("@Login_Password", Password);
                        cmd.Parameters.AddWithValue("@Status", Status);
                        cmd.Parameters.AddWithValue("@User_IP", USER_IP);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }
    }
}



