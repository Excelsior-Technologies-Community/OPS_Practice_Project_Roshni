using System.Data;
using OPS_Practice_Project.Models;
using Microsoft.Data.SqlClient;
using System.Diagnostics.Metrics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
namespace OPS_Practice_Project.Repository
{
    public class UserRepository
    {
        string _connectionString = "Server=DESKTOP-0OMS0D3\\SQLEXPRESS;Database=ExTPracticeProject;TrustServerCertificate=True;Trusted_Connection=True;MultipleActiveResultSets=True;";

        public List<CountryModel> GetCountries()
        {
            List<CountryModel> countries = new List<CountryModel>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT CountryId, Country FROM tbl_Country_Mst";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            countries.Add(new CountryModel
                            {
                                CountryId = Convert.ToInt32(reader["CountryId"]),
                                Country = reader["Country"].ToString()
                            });
                        }
                    }
                }
            }

            // Ensure an empty list is returned if no countries exist
            return countries.Count > 0 ? countries : new List<CountryModel>();
        }

        public List<StateModel> GetStates(int countryId)
        {
            List<StateModel> states = new List<StateModel>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT StateId, State FROM tbl_State_Mst WHERE CountryId = @CountryId";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CountryId", countryId);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            states.Add(new StateModel
                            {
                                StateId = Convert.ToInt32(reader["StateId"]),
                                State = reader["State"].ToString()
                            });
                        }
                    }
                }
            }
            return states;
        }

        public List<CityModel> GetCities(int stateId)
        {
            List<CityModel> cities = new List<CityModel>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT CityId, City FROM tbl_City_Mst WHERE StateId = @StateId";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@StateId", stateId);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cities.Add(new CityModel
                            {
                                CityId = Convert.ToInt32(reader["CityId"]),
                                City = reader["City"].ToString()
                            });
                        }
                    }
                }
            }
            return cities;
        }

        public List<UserTypeModel> GetUserTypes()
        {
            List<UserTypeModel> userTypes = new List<UserTypeModel>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT Id, UserType FROM tbl_User_Type_Mst";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            userTypes.Add(new UserTypeModel
                            {
                                Id = Convert.ToInt64(reader["Id"]),
                                UserType = reader["UserType"].ToString()
                            });
                        }
                    }
                }
            }
            return userTypes;
        }

        public string GetCountryNameById(int countryId)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                const string query = "SELECT Country FROM tbl_Country_Mst WHERE CountryId = @CountryId";
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CountryId", countryId);
                    con.Open();
                    return cmd.ExecuteScalar()?.ToString() ?? string.Empty;
                }
            }
        }

        public string GetStateNameById(int stateId)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                const string query = "SELECT State FROM tbl_State_Mst WHERE StateId = @StateId";
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@StateId", stateId);
                    con.Open();
                    return cmd.ExecuteScalar()?.ToString() ?? string.Empty;
                }
            }
        }

        public string GetCityNameById(int cityId)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                const string query = "SELECT City FROM tbl_City_Mst WHERE CityId = @CityId";
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CityId", cityId);
                    con.Open();
                    return cmd.ExecuteScalar()?.ToString() ?? string.Empty;
                }
            }
        }

        public string GetUserTypeById(int userTypeId)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                const string query = "SELECT UserType FROM tbl_User_Type_Mst WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", userTypeId);
                    con.Open();
                    return cmd.ExecuteScalar()?.ToString() ?? string.Empty;
                }
            }
        }


        public void InsertUserS(UserModel user, string ActionMode)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                
                using (SqlCommand command = new SqlCommand("SP_User_Save", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@FirstName", user.FirstName);
                    command.Parameters.AddWithValue("@MiddleName", user.MiddleName);
                    command.Parameters.AddWithValue("@LastName", user.LastName);
                    command.Parameters.AddWithValue("@ProfileImage", user.ProfileImage ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@EmailAddress", user.EmailAddress);
                    command.Parameters.AddWithValue("@MobileNumber", user.MobileNumber);
                    command.Parameters.AddWithValue("@UserName", user.UserName);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@Address", user.Address);
                    command.Parameters.AddWithValue("@CountryId", user.CountryId);
                    command.Parameters.AddWithValue("@StateId", user.StateId);
                    command.Parameters.AddWithValue("@CityId", user.CityId);
                    command.Parameters.AddWithValue("@UserTypeId", user.UserTypeId);
                    command.Parameters.AddWithValue("@Flag", user.Flag);
                    command.Parameters.AddWithValue("@CreateUser", user.CreateUser);
                    command.Parameters.AddWithValue("@CaptchaCode", user.CaptchaCode);
                    command.Parameters.AddWithValue("@CaptchaHashKey", user.CaptchaHashKey);
                    command.Parameters.AddWithValue("@CaptchaExpiry", user.CaptchaExpiry);
                    command.Parameters.AddWithValue("@CaptchaStatus", user.CaptchaStatus);
                    command.Parameters.AddWithValue("@ActionMode", ActionMode);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateUser(UserViewModel model ,string ActionMode)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_User_Save", connection))
                {

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@FirstName", model.Register.FirstName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@MiddleName", model.Register.MiddleName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@LastName", model.Register.LastName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ProfileImage",model.Register.ProfileImage ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@EmailAddress", model.Register.EmailAddress ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@MobileNumber", model.Register.MobileNumber ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UserName", model.Register.UserName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Password", model.Register.Password ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Address", model.Register.Address ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CountryId", model.Register.CountryId );
                    command.Parameters.AddWithValue("@StateId", model.Register.StateId );
                    command.Parameters.AddWithValue("@CityId", model.Register.CityId );
                    command.Parameters.AddWithValue("@UserTypeId", model.Register.UserTypeId);

                    command.Parameters.AddWithValue("@ActionMode", ActionMode);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteUser(int id, int createUser)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_User_Save", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Required Parameters with default values for Delete operation
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@FirstName", DBNull.Value);
                    command.Parameters.AddWithValue("@MiddleName", DBNull.Value);
                    command.Parameters.AddWithValue("@LastName", DBNull.Value);
                    command.Parameters.AddWithValue("@ProfileImage", DBNull.Value);
                    command.Parameters.AddWithValue("@EmailAddress", DBNull.Value);
                    command.Parameters.AddWithValue("@MobileNumber", DBNull.Value);
                    command.Parameters.AddWithValue("@UserName", DBNull.Value);
                    command.Parameters.AddWithValue("@Password", DBNull.Value);
                    command.Parameters.AddWithValue("@Address", DBNull.Value);
                    command.Parameters.AddWithValue("@CountryId", DBNull.Value);
                    command.Parameters.AddWithValue("@StateId", DBNull.Value);
                    command.Parameters.AddWithValue("@CityId", DBNull.Value);
                    command.Parameters.AddWithValue("@UserTypeId", DBNull.Value);
                    command.Parameters.AddWithValue("@Flag", 'D'); // 'D' for delete flag
                    command.Parameters.AddWithValue("@CreateUser", createUser);
                    command.Parameters.AddWithValue("@ActionMode", "Delete");

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }



        public UserModel GetUserById(int productId, string actionMode = "SelectById")
        {
            UserModel profile = new UserModel();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_User_List", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", productId);
                    command.Parameters.AddWithValue("@ActionMode", actionMode);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            profile = new UserModel
                            {
                                Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                                FirstName = reader["FirstName"].ToString(),
                                MiddleName = reader["MiddleName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                ProfileImage = reader["ProfileImage"] as string,
                                EmailAddress = reader["EmailAddress"].ToString(),
                                MobileNumber = reader["MobileNumber"].ToString(),
                                UserName = reader["UserName"].ToString(),
                                Password = reader["Password"].ToString(),
                                Address = reader["Address"].ToString(),
                                CountryId = reader["CountryId"] != DBNull.Value ? Convert.ToInt32(reader["CountryId"]) : 0,
                                StateId = reader["StateId"] != DBNull.Value ? Convert.ToInt32(reader["StateId"]) : 0,
                                CityId = reader["CityId"] != DBNull.Value ? Convert.ToInt32(reader["CityId"]) : 0,
                                UpdateDate = reader["UpdateDate"] != DBNull.Value ? Convert.ToDateTime(reader["UpdateDate"]) : (DateTime?)null,


                            };

                            //profileList.Add(profile);
                        }
                    }
                }
            }

            return profile;
        }

        public List<UserModel> GetUserList(long? userId = null, string actionMode = "Select")
        {
            List<UserModel> profileList = new List<UserModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_User_List", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ActionMode", actionMode);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UserModel profile = new UserModel
                            {
                                Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                                FirstName = reader["FirstName"].ToString(),
                                MiddleName = reader["MiddleName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                ProfileImage = reader["ProfileImage"] as string,
                                EmailAddress = reader["EmailAddress"].ToString(),
                                MobileNumber = reader["MobileNumber"].ToString(),
                                UserName = reader["UserName"].ToString(),
                                Password = reader["Password"].ToString(),
                                Address = reader["Address"].ToString(),
                                CountryId = reader["CountryId"] != DBNull.Value ? Convert.ToInt32(reader["CountryId"]) : 0,
                                StateId = reader["StateId"] != DBNull.Value ? Convert.ToInt32(reader["StateId"]) : 0,
                                CityId = reader["CityId"] != DBNull.Value ? Convert.ToInt32(reader["CityId"]) : 0,
                                UserTypeId = reader["UserTypeId"] != DBNull.Value ? Convert.ToInt32(reader["UserTypeId"]) : 0,
                                UpdateDate = reader["UpdateDate"] != DBNull.Value ? Convert.ToDateTime(reader["UpdateDate"]) : (DateTime?)null,
                                CaptchaCode = reader["CaptchaCode"] as string,
                                CaptchaHashKey = reader["CaptchaHashKey"] as string,  // Ensure this exists in UserModel
                                CaptchaExpiry = reader["CaptchaExpiry"] != DBNull.Value ? Convert.ToDateTime(reader["CaptchaExpiry"]) : (DateTime?)null,
                                CaptchaStatus = reader["CaptchaStatus"] as string
                            };

                            profileList.Add(profile);
                        }
                    }
                }
            }


            return profileList;
        }


    }
}
