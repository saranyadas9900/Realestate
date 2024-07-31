using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class AdminController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

        // GET: Admin
        public ActionResult Index()
        {
            if (Session["Admin"] != null)
            {
                var admin = (Admin)Session["Admin"];
                return View(admin);
            }
            return RedirectToAction("Login");
        }

        // GET: Admin/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Admin/Login
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            Admin admin = ValidateAdmin(username, password);
            if (admin != null)
            {
                Session["Admin"] = admin;
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid username or password";
                return View();
            }
        }

        private Admin ValidateAdmin(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_AdminLogin", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Admin found
                        return new Admin
                        {
                            AdminId = (int)reader["AdminId"],
                            Username = reader["Username"].ToString()
                        };
                    }
                    else
                    {
                        // Admin not found
                        return null;
                    }
                }
            }
        }

        public ActionResult CreatePropertyListing()
        {
            return View();
        }

        public ActionResult ViewPropertyListings()
        {
            return View();
        }

        public ActionResult ViewUserDetails()
        {
            return View();
        }

        public ActionResult AddNewAgent()
        {
            return View();
        }

        public ActionResult ChangeAdminPassword()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Session["Admin"] = null;
            return RedirectToAction("Index", "Home");
        }
    }
}
