using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
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

        // POST: Admin/CreatePropertyListing
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePropertyListing(PropertyListing model)
        {
            if (ModelState.IsValid)
            {
                if (model.Photo != null && model.Photo.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(model.Photo.InputStream))
                    {
                        byte[] fileBytes = binaryReader.ReadBytes(model.Photo.ContentLength);
                        model.PhotoBase64 = Convert.ToBase64String(fileBytes);
                    }
                }

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        using (SqlCommand command = new SqlCommand("sp_CreatePropertyListing", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@Title", model.Title);
                            command.Parameters.AddWithValue("@Description", model.Description);
                            command.Parameters.AddWithValue("@Address", model.Address);
                            command.Parameters.AddWithValue("@City", model.City);
                            command.Parameters.AddWithValue("@State", model.State);
                            command.Parameters.AddWithValue("@ZipCode", model.ZipCode);
                            command.Parameters.AddWithValue("@Price", model.Price);
                            command.Parameters.AddWithValue("@Bedrooms", model.Bedrooms);
                            command.Parameters.AddWithValue("@Bathrooms", model.Bathrooms);
                            command.Parameters.AddWithValue("@ListingDate", model.ListingDate);
                            command.Parameters.AddWithValue("@PhotoBase64", model.PhotoBase64);

                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                    }
                    ViewBag.Message = "Property listing created successfully!";
                    return RedirectToAction("CreatePropertyListingSuccess");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = $"An error occurred: {ex.Message}";
                }
            }
            return View(model);
        }

        // GET: Admin/CreatePropertyListingSuccess
        public ActionResult CreatePropertyListingSuccess()
        {
            return View();
        }


        public ActionResult ViewPropertyListings()
        {
            List<PropertyListing> propertyListings = new List<PropertyListing>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_GetPropertyListings", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var listing = new PropertyListing
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Description = reader.GetString(reader.GetOrdinal("Description")),
                                    Address = reader.GetString(reader.GetOrdinal("Address")),
                                    City = reader.GetString(reader.GetOrdinal("City")),
                                    State = reader.GetString(reader.GetOrdinal("State")),
                                    ZipCode = reader.GetString(reader.GetOrdinal("ZipCode")),
                                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                    Bedrooms = reader.GetInt32(reader.GetOrdinal("Bedrooms")),
                                    Bathrooms = reader.GetInt32(reader.GetOrdinal("Bathrooms")),
                                    ListingDate = reader.GetDateTime(reader.GetOrdinal("ListingDate")),
                                    PhotoBase64 = reader.GetString(reader.GetOrdinal("PhotoBase64"))
                                };
                                propertyListings.Add(listing);
                            }
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"An error occurred: {ex.Message}";
            }

            // Debugging output
            System.Diagnostics.Debug.WriteLine($"PropertyListings count: {propertyListings.Count}");

            return View(propertyListings);
        }


        public ActionResult Edit(int id)
        {
            PropertyListing listing = GetPropertyListingById(id);
            if (listing == null)
            {
                return HttpNotFound();
            }
            return View(listing);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PropertyListing model, HttpPostedFileBase Photo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Handle file upload
                    string photoBase64 = null;
                    if (Photo != null && Photo.ContentLength > 0)
                    {
                        using (var binaryReader = new BinaryReader(Photo.InputStream))
                        {
                            byte[] imageBytes = binaryReader.ReadBytes(Photo.ContentLength);
                            photoBase64 = Convert.ToBase64String(imageBytes);
                        }
                    }
                    else
                    {
                        photoBase64 = model.PhotoBase64; // Retain old photo if no new photo is uploaded
                    }

                    // Update the model with the new photo base64 value
                    model.PhotoBase64 = photoBase64;

                    // Update database
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        using (SqlCommand command = new SqlCommand("sp_UpdatePropertyListing", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@Id", model.Id);
                            command.Parameters.AddWithValue("@Title", model.Title ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Description", model.Description ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Address", model.Address ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@City", model.City ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@State", model.State ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@ZipCode", model.ZipCode ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Price", model.Price);
                            command.Parameters.AddWithValue("@Bedrooms", model.Bedrooms);
                            command.Parameters.AddWithValue("@Bathrooms", model.Bathrooms);
                            command.Parameters.AddWithValue("@ListingDate", model.ListingDate);
                            command.Parameters.AddWithValue("@PhotoBase64", model.PhotoBase64 ?? (object)DBNull.Value);

                            connection.Open();
                            int rowsAffected = command.ExecuteNonQuery();
                            connection.Close();

                            if (rowsAffected > 0)
                            {
                                ViewBag.Message = "Update successful.";
                                return RedirectToAction("ViewPropertyListings");
                            }
                            else
                            {
                                ViewBag.Message = "No rows affected. Update failed.";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = $"An error occurred: {ex.Message}";
                }
            }
            else
            {
                ViewBag.Message = "Model state is invalid.";
            }

            return View(model);
        }



        // POST: Admin/DeleteConfirmed
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_DeletePropertyListing", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Id", id);

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                return RedirectToAction("ViewPropertyListings");
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"An error occurred: {ex.Message}";
                return View();
            }
        }


        private PropertyListing GetPropertyListingById(int id)
        {
            PropertyListing listing = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_GetPropertyListingById", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Id", id);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                listing = new PropertyListing
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Description = reader.GetString(reader.GetOrdinal("Description")),
                                    Address = reader.GetString(reader.GetOrdinal("Address")),
                                    City = reader.GetString(reader.GetOrdinal("City")),
                                    State = reader.GetString(reader.GetOrdinal("State")),
                                    ZipCode = reader.GetString(reader.GetOrdinal("ZipCode")),
                                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                    Bedrooms = reader.GetInt32(reader.GetOrdinal("Bedrooms")),
                                    Bathrooms = reader.GetInt32(reader.GetOrdinal("Bathrooms")),
                                    ListingDate = reader.GetDateTime(reader.GetOrdinal("ListingDate")),
                                    PhotoBase64 = reader.GetString(reader.GetOrdinal("PhotoBase64"))
                                };
                            }
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"An error occurred: {ex.Message}";
            }
            return listing;
        }
        // GET: Admin/Delete/5
        public ActionResult Delete(int id)
        {
            PropertyListing listing = GetPropertyListingById(id);
            if (listing == null)
            {
                return HttpNotFound();
            }
            return View(listing);
        }

        // GET: Admin/DeleteUser/5
        public ActionResult DeleteUser(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Invalid user ID.";
                return View();
            }

            User user = GetUserById(id);
            if (user == null)
            {
                ViewBag.Message = "User not found.";
                return View();
            }

            return View(user);
        }

        // POST: Admin/DeleteUser/5
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUserConfirmed(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_DeleteUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", id);
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        connection.Close();
                        if (rowsAffected > 0)
                        {
                            ViewBag.Message = "User deleted successfully.";
                        }
                        else
                        {
                            ViewBag.Message = "User deletion failed.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("ViewUserDetails");
        }

        private User GetUserById(int id)
        {
            User user = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_GetUserById", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", id);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                user = new User
                                {
                                    UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    EmailAddress = reader.GetString(reader.GetOrdinal("EmailAddress")),
                                    Address = reader.GetString(reader.GetOrdinal("Address")),
                                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                    City = reader.GetString(reader.GetOrdinal("City")),
                                    State = reader.GetString(reader.GetOrdinal("State")),
                                    DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                                    Gender = reader.GetString(reader.GetOrdinal("Gender"))
                                };
                            }
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching user: {ex.Message}");
                ViewBag.Message = $"An error occurred: {ex.Message}";
            }
            return user;
        }





        public ActionResult ViewUserDetails()
        {
            List<User> users = new List<User>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_GetAllUsers", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var user = new User
                                {
                                    UserID=reader.GetInt32(reader.GetOrdinal("UserID")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    EmailAddress = reader.GetString(reader.GetOrdinal("EmailAddress")),
                                    Address = reader.GetString(reader.GetOrdinal("Address")),
                                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                    City = reader.GetString(reader.GetOrdinal("City")),
                                    State = reader.GetString(reader.GetOrdinal("State")),
                                    DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                                    Gender = reader.GetString(reader.GetOrdinal("Gender"))
                                };
                                users.Add(user);
                            }
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"An error occurred: {ex.Message}";
            }

            // Debugging output
            System.Diagnostics.Debug.WriteLine($"Users count: {users.Count}");

            return View(users);
        }
        public ActionResult EditUser(int id)
        {
            User user = GetUserById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(User model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        using (SqlCommand command = new SqlCommand("sp_UpdateUser", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@UserID", model.UserID);
                            command.Parameters.AddWithValue("@FirstName", model.FirstName);
                            command.Parameters.AddWithValue("@LastName", model.LastName);
                            command.Parameters.AddWithValue("@EmailAddress", model.EmailAddress);
                            command.Parameters.AddWithValue("@Address", model.Address);
                            command.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
                            command.Parameters.AddWithValue("@City", model.City);
                            command.Parameters.AddWithValue("@State", model.State);
                            command.Parameters.AddWithValue("@DateOfBirth", model.DateOfBirth);
                            command.Parameters.AddWithValue("@Gender", model.Gender);
                            connection.Open();
                            int rowsAffected = command.ExecuteNonQuery();
                            connection.Close();
                            if (rowsAffected > 0)
                            {
                                ViewBag.Message = "User updated successfully!";
                                return RedirectToAction("ViewUserDetails");
                            }
                            else
                            {
                                ViewBag.Message = "User update failed.";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error updating user: {ex.Message}");
                    ViewBag.Message = "An error occurred while updating the user.";
                }
            }
            else
            {
                foreach (var modelState in ViewData.ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Model Error: {error.ErrorMessage}");
                    }
                }
            }
            return View(model);
        }
        public ActionResult AddNewAgent()
        {
            return View();
        }


        // POST: AddNewAgent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNewAgent(AgentViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_AddNewAgent", con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Username", model.Username);
                        cmd.Parameters.AddWithValue("@Password", model.Password);
                        cmd.Parameters.AddWithValue("@Name", model.Name);
                        cmd.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
                        cmd.Parameters.AddWithValue("@Email", model.Email);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }

                return RedirectToAction("ViewAgentDetails"); // Redirect to a relevant page after successful addition
            }

            // If model state is invalid, return the same view with the validation messages
            return View(model);
        }

        // GET: ViewAgentDetails/5
        public ActionResult ViewAgentDetails()
        {
            List<AgentViewModel> agents = new List<AgentViewModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_GetAllAgents", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var agent = new AgentViewModel
                                {
                                    AgentID = reader.GetInt32(reader.GetOrdinal("AgentID")),
                                    Username = reader.GetString(reader.GetOrdinal("Username")),

                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                    Email = reader.GetString(reader.GetOrdinal("Email"))
                                };
                                agents.Add(agent);
                            }
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"An error occurred: {ex.Message}";
            }

            // Debugging output
            System.Diagnostics.Debug.WriteLine($"Agents count: {agents.Count}");

            return View(agents);
        }

        public AgentViewModel GetAgentById(int id)
        {
            AgentViewModel agent = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_GetAgentById", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@AgentID", id);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                agent = new AgentViewModel
                                {
                                    AgentID = reader.GetInt32(reader.GetOrdinal("AgentID")),
                                    Username = reader.GetString(reader.GetOrdinal("Username")),
                                    Password = reader.GetString(reader.GetOrdinal("Password")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                    Email = reader.GetString(reader.GetOrdinal("Email"))
                                };
                            }
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions or log errors as necessary
                System.Diagnostics.Debug.WriteLine($"An error occurred: {ex.Message}");
            }

            return agent;
        }


        public ActionResult EditAgent(int id)
        {
            AgentViewModel agent = GetAgentById(id);
            if (agent == null)
            {
                return HttpNotFound();
            }
            return View(agent);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAgent(AgentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_UpdateAgent", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@AgentID", model.AgentID);
                            cmd.Parameters.AddWithValue("@Username", model.Username);
                            cmd.Parameters.AddWithValue("@Password", model.Password);
                            cmd.Parameters.AddWithValue("@Name", model.Name);
                            cmd.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
                            cmd.Parameters.AddWithValue("@Email", model.Email);

                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }

                    ViewBag.Message = "Agent updated successfully!";
                    return RedirectToAction("ViewAgentDetails");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = $"An error occurred: {ex.Message}";
                }
            }
            return View(model);
        }
        public ActionResult DeleteAgent(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_DeleteAgent", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@AgentID", id);

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("ViewAgentDetails");
        }

        [HttpGet]
        public ActionResult ChangeAdminPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeAdminPassword(ChangeAdminPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["Admin"] != null)
                    {
                        var admin = (Admin)Session["Admin"];
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            using (SqlCommand command = new SqlCommand("sp_ChangeAdminPassword", connection))
                            {
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@Username", admin.Username);
                                command.Parameters.AddWithValue("@OldPassword", model.OldPassword);
                                command.Parameters.AddWithValue("@NewPassword", model.NewPassword);
                                int rowsAffected = command.ExecuteNonQuery();
                                if (rowsAffected > 0)
                                {
                                    ViewBag.Message = "Password changed successfully!";
                                    return View(model);
                                }
                                else
                                {
                                    ViewBag.Message = "Password change failed. Please ensure the old password is correct.";
                                }
                            }
                        }
                    }
                    else
                    {
                        ViewBag.Message = "User is not authenticated.";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = $"An error occurred: {ex.Message}";
                }
            }
            return View(model);
        }


        public ActionResult Logout()
        {
            Session["Admin"] = null;
            return RedirectToAction("Index", "Home");
        }
    }
}
