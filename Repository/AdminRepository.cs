﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using WebApplication4.Models;

namespace WebApplication4.Repositories
{
    public class AdminRepository
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

        public Admin ValidateAdmin(string username, string password)
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
                        return new Admin
                        {
                            AdminId = (int)reader["AdminId"],
                            Username = reader["Username"].ToString()
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public void CreatePropertyListing(PropertyListing model)
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
                    connection.Close();
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

        public void UpdatePropertyListing(PropertyListing model)
        {
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
                    command.Parameters.AddWithValue("@PhotoBase64", model.PhotoBase64);
                    Console.WriteLine("Updating photo: " + model.PhotoBase64);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public void DeletePropertyListing(int id)
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
                                    Gender = reader.GetString(reader.GetOrdinal("Gender"))
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

        public List<User> GetAllUsers()
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
                                users.Add(user);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching users: {ex.Message}");
            }
            return users;
        }

        public bool DeleteUser(int id)
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
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting user: {ex.Message}");
                return false;
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

        public bool AddAgent(AgentViewModel agent)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_AddNewAgent", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Username", agent.Username);
                        command.Parameters.AddWithValue("@Password", agent.Password);
                        command.Parameters.AddWithValue("@Name", agent.Name);
                        command.Parameters.AddWithValue("@PhoneNumber", agent.PhoneNumber);
                        command.Parameters.AddWithValue("@Email", agent.Email);
                        connection.Open();
                        command.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding agent: {ex.Message}");
                return false;
            }
        }

        public List<AgentViewModel> GetAllAgents()
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
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching agents: {ex.Message}");
            }
            return agents;
        }

        public bool UpdateAgent(AgentViewModel agent)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_UpdateAgent", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@AgentID", agent.AgentID);
                        command.Parameters.AddWithValue("@Username", agent.Username);
                        command.Parameters.AddWithValue("@Password", agent.Password);
                        command.Parameters.AddWithValue("@Name", agent.Name);
                        command.Parameters.AddWithValue("@PhoneNumber", agent.PhoneNumber);
                        command.Parameters.AddWithValue("@Email", agent.Email);
                        connection.Open();
                        command.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating agent: {ex.Message}");
                return false;
            }
        }

        public bool DeleteAgent(int id)
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
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting agent: {ex.Message}");
                return false;
            }
        }

        
        }
}
