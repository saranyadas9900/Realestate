using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;
using WebApplication4.Repositories;
using System.Configuration;

namespace WebApplication4.Controllers
{
    public class AdminController : Controller

    {

        private readonly string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
        private readonly AdminRepository _adminRepository = new AdminRepository();

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
            Admin admin = _adminRepository.ValidateAdmin(username, password);
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
        //get
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
                // Check if Photo is null, if so, set PhotoBase64 to null
                if (model.Photo == null)
                {
                    model.PhotoBase64 = null;
                }
                else if (model.Photo.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(model.Photo.InputStream))
                    {
                        byte[] fileBytes = binaryReader.ReadBytes(model.Photo.ContentLength);
                        model.PhotoBase64 = Convert.ToBase64String(fileBytes);
                    }
                }

                try
                {
                    _adminRepository.CreatePropertyListing(model);
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
        //get method
        public ActionResult ViewPropertyListings()
        {
            try
            {
                List<PropertyListing> propertyListings = _adminRepository.GetPropertyListings();
                return View(propertyListings);
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"An error occurred: {ex.Message}";
                return View(new List<PropertyListing>());
            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                PropertyListing listing = _adminRepository.GetPropertyListingById(id);
                if (listing == null)
                {
                    return HttpNotFound();
                }
                return View(listing);
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"An error occurred: {ex.Message}";
                return View(new PropertyListing());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PropertyListing model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Update the PhotoBase64 property with the new photo data
                    if (model.Photo != null && model.Photo.ContentLength > 0)
                    {
                        using (var binaryReader = new BinaryReader(model.Photo.InputStream))
                        {
                            byte[] fileBytes = binaryReader.ReadBytes(model.Photo.ContentLength);
                            model.PhotoBase64 = Convert.ToBase64String(fileBytes);
                        }
                    }

                    _adminRepository.UpdatePropertyListing(model);
                    return RedirectToAction("ViewPropertyListings");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = $"An error occurred: {ex.Message}";
                }
            }
            return View(model);
        }
        //get
        public ActionResult Delete(int id)
        {
            try
            {
                PropertyListing listing = _adminRepository.GetPropertyListingById(id);
                if (listing == null)
                {
                    return HttpNotFound();
                }
                return View(listing);
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"An error occurred: {ex.Message}";
                return View(new PropertyListing());
            }
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _adminRepository.DeletePropertyListing(id);
                return RedirectToAction("ViewPropertyListings");
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"An error occurred: {ex.Message}";
                return View();
            }
        }


        public ActionResult ViewUserDetails()
        {
            IEnumerable<User> users = _adminRepository.GetAllUsers();
            return View(users);
        }
        public ActionResult DeleteUser(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Invalid user ID.";
                return View();
            }

            User user = _adminRepository.GetUserById(id);
            if (user == null)
            {
                ViewBag.Message = "User not found.";
                return View();
            }

            return View(user);
        }

        [HttpPost]
        public ActionResult DeleteUserConfirmed(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Invalid user ID.";
                return View("DeleteUser");
            }
            bool isDeleted = _adminRepository.DeleteUser(id);
            ViewBag.Message = isDeleted ? "User deleted successfully." : "Failed to delete user.";
            return RedirectToAction("ViewUserDetails");
        }
        public ActionResult EditUser(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Invalid user ID.";
                return View();
            }

            User user = _adminRepository.GetUserById(id);
            if (user == null)
            {
                ViewBag.Message = "User not found.";
                return View();
            }

            return View(user);
        }

        [HttpPost]
        public ActionResult EditUser(User user)
        {
            if (ModelState.IsValid)
            {
                bool isUpdated = _adminRepository.UpdateUser(user);
                ViewBag.Message = isUpdated ? "User updated successfully." : "Failed to update user.";
            }
            else
            {
                ViewBag.Message = "Invalid user data.";
            }

            return RedirectToAction("ViewUserDetails");
        }

        public ActionResult AddNewAgent()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddNewAgent(AgentViewModel agent)
        {
            if (ModelState.IsValid)
            {
                bool isAdded = _adminRepository.AddAgent(agent);
                ViewBag.Message = isAdded ? "Agent added successfully." : "Failed to add agent.";
            }
            else
            {
                ViewBag.Message = "Invalid agent data.";
            }

            return RedirectToAction("ViewAgentDetails");
        }

        public ActionResult ViewAgentDetails()
        {
            List<AgentViewModel> agents = _adminRepository.GetAllAgents();
            return View(agents);
        }

        public ActionResult EditAgent(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Invalid agent ID.";
                return View();
            }

            AgentViewModel agent = _adminRepository.GetAllAgents().FirstOrDefault(a => a.AgentID == id);
            if (agent == null)
            {
                ViewBag.Message = "Agent not found.";
                return View();
            }

            return View(agent);
        }

        [HttpPost]
        public ActionResult EditAgent(AgentViewModel agent)
        {
            if (ModelState.IsValid)
            {
                bool isUpdated = _adminRepository.UpdateAgent(agent);
                ViewBag.Message = isUpdated ? "Agent updated successfully." : "Failed to update agent.";
            }
            else
            {
                ViewBag.Message = "Invalid agent data.";
            }

            return RedirectToAction("ViewAgentDetails");
        }

        public ActionResult DeleteAgent(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Invalid agent ID.";
                return View();
            }

            AgentViewModel agent = _adminRepository.GetAllAgents().FirstOrDefault(a => a.AgentID == id);
            if (agent == null)
            {
                ViewBag.Message = "Agent not found.";
                return View();
            }

            return View(agent);
        }

        [HttpPost]
        public ActionResult DeleteAgentConfirmed(int id)
        {
            if (id <= 0)
            {
                ViewBag.Message = "Invalid agent ID.";
                return View("DeleteAgent");
            }

            bool isDeleted = _adminRepository.DeleteAgent(id);
            ViewBag.Message = isDeleted ? "Agent deleted successfully." : "Failed to delete agent.";

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

    
