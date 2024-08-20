using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Configuration;
using WebApplication4.Models;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication4.Repository
{
    public class AgentRepository
    {

        private readonly string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
        public bool ValidateAgent(SignInModel model)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_AuthenticateAgent", connection))
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

        public int GetAgentIdByUsername(string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_GetAgentIdByUsername", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Username", username);

                    connection.Open();
                    var result = command.ExecuteScalar();
                    return result != null ? (int)result : 0;
                }
            }
        }

        public AgentViewModel GetAgentDetails(int agentId)
        {
            AgentViewModel agent = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_GetAgentDetails", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@AgentID", agentId);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            agent = new AgentViewModel
                            {
                                AgentID = reader.GetInt32(reader.GetOrdinal("AgentID")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                Email = reader.GetString(reader.GetOrdinal("Email"))
                            };
                        }
                    }
                }
            }

            return agent;
        }
        public IEnumerable<AgentViewModel> GetAllAgents()
        {
            var agents = new List<AgentViewModel>();

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
                            agents.Add(new AgentViewModel
                            {
                                AgentID = reader.GetInt32(reader.GetOrdinal("AgentID")),
                                Username = reader.GetString(reader.GetOrdinal("Username"))
                            });
                        }
                    }
                }
            }

            return agents;
        }

        public void CreatePropertyListing(PropertyListing model, int agentID)
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
                    command.Parameters.AddWithValue("@AgentID", agentID); // Pass AgentID

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }


        public IEnumerable<PropertyListing> GetPropertyListingsByAgent(int agentID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_GetPropertyListingsByAgent", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@AgentID", agentID);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        var properties = new List<PropertyListing>();
                        while (reader.Read())
                        {
                            properties.Add(new PropertyListing
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Description = reader.GetString(2),
                                Address = reader.GetString(3),
                                City = reader.GetString(4),
                                State = reader.GetString(5),
                                ZipCode = reader.GetString(6),
                                Price = reader.GetDecimal(7),
                                Bedrooms = reader.GetInt32(8),
                                Bathrooms = reader.GetInt32(9),
                                ListingDate = reader.GetDateTime(10),
                                PhotoBase64 = reader.IsDBNull(11) ? null : reader.GetString(11),
                                AgentID = reader.GetInt32(12)
                            });
                        }
                        return properties;
                    }
                }
            }
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
        public bool DeletePropertyListing(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_DeletePropertyListing", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Id", id); // Ensure parameter name matches stored procedure

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting property listing: {ex.Message}");
                return false;
            }
        }

        public IEnumerable<VisitSchedule> GetVisitSchedulesForAgent(int agentId)
        {
            var visitSchedules = new List<VisitSchedule>();

            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand("sp_GetVisitSchedulesForAgent", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@AgentID", agentId);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            visitSchedules.Add(new VisitSchedule
                            {
                                PropertyId = reader.GetInt32(reader.GetOrdinal("PropertyId")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                VisitDate = reader.GetDateTime(reader.GetOrdinal("VisitDate")),
                                Title = reader.GetString(reader.GetOrdinal("PropertyTitle")), // Example, adjust based on your schema
                                UserName = reader.GetString(reader.GetOrdinal("UserName")) // Example, adjust based on your schema
                            });
                        }
                    }
                }
            }

            return visitSchedules;
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
