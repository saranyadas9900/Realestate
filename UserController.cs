using System;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Web.Mvc;
using WebApplication4.Models;

using WebApplication4.Repositories;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using System.Net;

namespace WebApplication4.Controllers
{
    public class UserController : Controller
    {
        private readonly UserRepository _userRepository = new UserRepository();

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
                    _userRepository.RegisterUser(user);
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
                    bool isValidUser = _userRepository.ValidateUser(model);
                    if (isValidUser)
                    {
                        // Assume user ID is retrieved and set in session
                        var userId = _userRepository.GetUserIdByUsername(model.Username);
                        Session["UserId"] = userId;
                        ViewBag.Message = "Login successful!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        // Invalid credentials
                        ViewBag.Message = "Invalid username or password.";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = $"An error occurred: {ex.Message}";
                }
            }
            return View(model);
        }
        public ActionResult Index()
        {
            try
            {
                var userId = GetCurrentUserId();
                var userModel = _userRepository.GetUserDetails(userId);
                return View(userModel);
            }
            catch (InvalidOperationException)
            {
                return RedirectToAction("SignIn", "User");
            }
        }
        public ActionResult ViewPropertyListings()
        {
            try
            {
                var properties = _userRepository.GetPropertyListings();
                return View(properties);
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"An error occurred: {ex.Message}";
                return View("Error");
            }
        }

        public ActionResult ScheduleVisit(int id)
        {
            // Store the PropertyId in TempData for later use
            TempData["PropertyId"] = id;
            // Create and pass an empty VisitSchedule model to the view
            return View("ScheduleVisit", new VisitSchedule());
        }
        public ActionResult VisitSuccess()
        {
            return View();
        }




        [HttpPost]
        public ActionResult ScheduleVisit(VisitSchedule model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int propertyId = (int)TempData["PropertyId"];
                    int userId = GetCurrentUserId();

                    // Call the stored procedure to schedule the visit
                    _userRepository.ScheduleVisit(propertyId, userId, model.VisitDate);

                    return RedirectToAction("VisitSuccess");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = $"An error occurred: {ex.Message}";
                }
            }
            return View(model);
        }




        [HttpGet]
        public ActionResult SearchProperties()
        {
            return View();
        }


        [HttpPost]
        public ActionResult SearchProperties(string state, string city, int? minBedrooms, int? minBathrooms, decimal? minPrice, decimal? maxPrice)
        {
            var properties = _userRepository.SearchProperties(state, city, minBedrooms, minBathrooms, minPrice, maxPrice);
            return View("SearchResults", properties);
        }

        [HttpPost]
        public ActionResult AddToFavorites(int id)
        {
            try
            {
                int userId = GetCurrentUserId(); // Ensure this method retrieves the current user ID correctly.
                _userRepository.AddToFavorites(userId, id); // Ensure this method adds the property to the user's favorites list.

                return RedirectToAction("ViewPropertyListings"); // Redirect after adding to favorites
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                ViewBag.Message = $"An error occurred: {ex.Message}";
                return View("Error"); // Return to an error view or handle the error appropriately
            }
        }


        public ActionResult ViewFavoriteProperties()
        {
            try
            {
                int userId = GetCurrentUserId();
                List<PropertyListing> favoriteProperties = _userRepository.GetFavoriteProperties(userId);
                return View(favoriteProperties);
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"An error occurred: {ex.Message}";
                return View("Error");
            }
        }

        public ActionResult EditProfile()
        {
            var userId = GetCurrentUserId();
            var userModel = _userRepository.GetUserDetails(userId);
            return View(userModel);
        }

        [HttpPost]
        public ActionResult EditProfile(User updatedUser)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Implement update user logic here
                    ViewBag.Message = "Profile updated successfully!";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = $"An error occurred: {ex.Message}";
                }
            }
            return View(updatedUser);
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
       /* public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Implement change password logic here
                    ViewBag.Message = "Password changed successfully!";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = $"An error occurred: {ex.Message}";
                }
            }
            return View(model);
        }*/

        public ActionResult Logout()
        {
            Session.Clear(); // Clears all session data
            return RedirectToAction("SignIn");
        }


        public int GetCurrentUserId()
        {
            if (Session["UserId"] != null)
            {
                return (int)Session["UserId"];
            }
            else
            {
                throw new InvalidOperationException("User is not logged in.");
            }
        }

    }
}
