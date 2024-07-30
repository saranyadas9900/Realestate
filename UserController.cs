using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class UserController : Controller
    {
        // GET: SignUp
        public ActionResult SignUp()
        {
            return View();
        }

        // POST: SignUp
        [HttpPost]
        public ActionResult SignUp(User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        using (SqlCommand command = new SqlCommand("sp_RegisterUser1", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@FirstName", user.FirstName);
                            command.Parameters.AddWithValue("@LastName", user.LastName);
                            command.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);
                            command.Parameters.AddWithValue("@Gender", user.Gender);
                            command.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                            command.Parameters.AddWithValue("@EmailAddress", user.EmailAddress);
                            command.Parameters.AddWithValue("@Address", user.Address);
                            command.Parameters.AddWithValue("@State", user.State);
                            command.Parameters.AddWithValue("@City", user.City);
                            command.Parameters.AddWithValue("@Username", user.Username);
                            command.Parameters.AddWithValue("@Password", user.Password);

                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                    }
                    ViewBag.Message = "Registration successful!";
                    return RedirectToAction("SignUpSuccess");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = $"An error occurred: {ex.Message}";
                }
            }
            return View(user);
        }

        // GET: SignUpSuccess
        public ActionResult SignUpSuccess()
        {
            return View();
        }

        // GET: SignIn
        public ActionResult SignIn()
        {
            return View();
        }

        // POST: SignIn
        [HttpPost]
        public ActionResult SignIn(SignInModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        using (SqlCommand command = new SqlCommand("sp_ValidateUser", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@Username", model.Username);
                            command.Parameters.AddWithValue("@Password", model.Password);

                            connection.Open();
                            var result = command.ExecuteScalar();

                            if (result != null && (int)result > 0)
                            {
                                // Successful login
                                ViewBag.Message = "Login successful!";
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                // Invalid credentials
                                ViewBag.Message = "Invalid username or password.";
                            }

                            connection.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = $"An error occurred: {ex.Message}";
                }
            }

            // Return the view with ViewBag.Message for error display
            return View(model);
        }
    }
}

