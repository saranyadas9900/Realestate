using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using WebApplication4.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Microsoft.Owin.BuilderProperties;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication4.Repositories
{
    public class UserRepository
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

        public void RegisterUser(User user)
        {
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
                    command.Parameters.AddWithValue("@Password", PasswordHasher.HashPassword(user.Password));
                   

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool ValidateUser(SignInModel model)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_ValidateUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Username", model.Username);
                    command.Parameters.AddWithValue("@Password", PasswordHasher.HashPassword(model.Password));

                    connection.Open();
                    var result = command.ExecuteScalar();
                    return result != null && (int)result > 0;

                }
            }
        }

        public User GetUserDetails(int userId)
        {
            User user = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_GetUserDetails", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", userId);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                UserID = reader.GetInt32(reader.GetOrdinal("UserId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                                Gender = reader.GetString(reader.GetOrdinal("Gender")),
                                PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                EmailAddress = reader.GetString(reader.GetOrdinal("EmailAddress")),
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                State = reader.GetString(reader.GetOrdinal("State")),
                                City = reader.GetString(reader.GetOrdinal("City")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                Password = reader.GetString(reader.GetOrdinal("Password"))
                            };
                        }
                    }
                }
            }

            return user;
        }

        public int GetUserIdByUsername(string username)
        {
            int userId = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_GetUserIdByUsername", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Username", username);

                    connection.Open();
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        userId = Convert.ToInt32(result);

                       
                    }
                }
            }
            return userId;
        }
        public List<PropertyListing> GetPropertyListings()
        {
            List<PropertyListing> propertyListings = new List<PropertyListing>();

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
                }
            }

            return propertyListings;
        }
        public PropertyListing GetPropertyListingById(int id)
        {
            PropertyListing listing = null;
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
            return listing;
        }


        public void RemoveExistingVisit(int propertyId, int userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_RemoveExistingVisit", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@PropertyId", propertyId);
                        command.Parameters.AddWithValue("@UserId", userId);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new ApplicationException("An error occurred while removing the existing visit.", ex);
            }
        }

        public bool IsVisitTimeTaken(int agentId, DateTime visitDate)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_IsVisitTimeTaken", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@AgentId", agentId);
                    command.Parameters.AddWithValue("@VisitDate", visitDate);
                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }








        public void ScheduleVisit(int propertyId, int userId, DateTime visitDate)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand("sp_ScheduleVisit", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@PropertyId", propertyId);
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@VisitDate", visitDate);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public List<PropertyListing> SearchProperties(string state, string city, int? minBedrooms, int? minBathrooms, decimal? minPrice, decimal? maxPrice)
        {
            var properties = new List<PropertyListing>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_SearchProperties", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@State", state);
                    command.Parameters.AddWithValue("@City", city);
                    command.Parameters.AddWithValue("@MinBedrooms", minBedrooms.HasValue ? minBedrooms.Value : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@MinBathrooms", minBathrooms.HasValue ? minBathrooms.Value : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@MinPrice", minPrice.HasValue ? minPrice.Value : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@MaxPrice", maxPrice.HasValue ? maxPrice.Value : (object)DBNull.Value);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            properties.Add(new PropertyListing
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
                            });
                        }
                    }
                }
            }

            return properties;
        }

        public void AddToFavorites(int userId, int propertyId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_AddToFavorites", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.Parameters.AddWithValue("@PropertyID", propertyId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<PropertyListing> GetFavoriteProperties(int userId)
        {
            var properties = new List<PropertyListing>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_GetFavoriteProperties", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserID", userId);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            properties.Add(new PropertyListing
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
                            });
                        }
                    }
                }
            }

            return properties;
        }

        public void RemoveFromFavorites(int userId, int propertyId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_RemoveFromFavorites", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.Parameters.AddWithValue("@PropertyID", propertyId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool UpdateUser(User user)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_UpdateUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", user.UserID);
                        command.Parameters.AddWithValue("@FirstName", user.FirstName);
                        command.Parameters.AddWithValue("@LastName", user.LastName);
                        command.Parameters.AddWithValue("@EmailAddress", user.EmailAddress);
                        command.Parameters.AddWithValue("@Address", user.Address);
                        command.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                        command.Parameters.AddWithValue("@City", user.City);
                        command.Parameters.AddWithValue("@State", user.State);
                        command.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);
                        command.Parameters.AddWithValue("@Gender", user.Gender);
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating user: {ex.Message}");
                return false;
            }
        }

        public User GetUserById(int id)
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
                                    Gender = reader.GetString(reader.GetOrdinal("Gender")),
                                    Password = reader.GetString(reader.GetOrdinal("Password"))
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching user: {ex.Message}");
            }
            return user;
        }


        public static class PasswordHasher
        {
            public static string HashPassword(string password)
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                    StringBuilder builder = new StringBuilder();
                    foreach (byte b in bytes)
                    {
                        builder.Append(b.ToString("x2"));
                    }
                    return builder.ToString();
                }
            }
        }



    }
}
