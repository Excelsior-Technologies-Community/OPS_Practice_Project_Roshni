using Microsoft.Data.SqlClient;
using OPS_Practice_Project.Models;
using System.Data;

namespace OPS_Practice_Project.Repository
{
    public class LoginRepository
    {
        string _connectionString = "Server=DESKTOP-0OMS0D3\\SQLEXPRESS;Database=ExTPracticeProject;TrustServerCertificate=True;Trusted_Connection=True;MultipleActiveResultSets=True;";

        public async Task<List<UserLoginHistory>> GetUserLoginHistoryAsync(long? id = null)
        {
            var loginHistoryList = new List<UserLoginHistory>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("sp_GetUserLoginHistory", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", (object)id ?? DBNull.Value);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            loginHistoryList.Add(new UserLoginHistory
                            {
                                Id = reader.GetInt64("Id"),
                                UserId = reader.GetInt64("UserId"),
                                UserName = reader.GetString("UserName"),
                                LoginTime = reader.GetDateTime("LoginTime"),
                                LogoutTime = reader.IsDBNull("LogoutTime") ? (DateTime?)null : reader.GetDateTime("LogoutTime"),
                                IPAddress = reader.GetString("IPAddress"),
                                UserAgent = reader.GetString("UserAgent"),
                                Status = reader.GetString("Status")
                            });
                        }
                    }
                }
            }
            return loginHistoryList;
        }


        public async Task<bool> InsertLoginRecordAsync(long userId, string userName, string ipAddress, string userAgent, string action)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("sp_ManageUserLoginHistory", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", action);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    cmd.Parameters.AddWithValue("@IPAddress", ipAddress);
                    cmd.Parameters.AddWithValue("@UserAgent", userAgent);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }


        public async Task<bool> UpdateLogoutRecordAsync(int userId, string action)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("sp_ManageUserLoginHistory", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", action);
                    cmd.Parameters.AddWithValue("@Id", userId);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }



        public async Task<bool> DeleteLoginRecordAsync(long id, string action)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("sp_ManageUserLoginHistory", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", action);
                    cmd.Parameters.AddWithValue("@Id", id);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }
        public long GetLastInsertedId(long userId)
        {
            long lastId = 0;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 Id FROM tbl_User_Login_History WHERE UserId = @UserId ORDER BY LoginTime DESC", conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        lastId = Convert.ToInt64(result);
                    }
                }
            }
            return lastId;
        }

    }
}
