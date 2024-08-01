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





        public ActionResult ViewUserDetails()
        {
            List<User> users = new List<User>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetAllUsers", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var user = new User
                        {
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            EmailAddress = reader["Email"].ToString(),
                            PhoneNumber = reader["PhoneNumber"].ToString(),
                            City = reader["City"].ToString(),
                            State = reader["State"].ToString(),
                            DateOfBirth = reader["Dob"] != DBNull.Value ? Convert.ToDateTime(reader["Dob"]) : default(DateTime), 
                            Gender = reader["Gender"].ToString()
                        };

                        users.Add(user);
                    }
                    con.Close();
                }
            }

            return View(users);
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
