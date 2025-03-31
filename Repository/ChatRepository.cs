using System.Data;
using OPS_Practice_Project.Models;
using Microsoft.Data.SqlClient;
using System.Diagnostics.Metrics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

namespace OPS_Practice_Project.Repository
{
    public class ChatRepository
    {
        string _connectionString = "Server=DESKTOP-0OMS0D3\\SQLEXPRESS;Database=ExTPracticeProject;TrustServerCertificate=True;Trusted_Connection=True;MultipleActiveResultSets=True;";

        public string InsertChat(ChatModel model,string action)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("sp_ManageMessages", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ActionType", action);
                cmd.Parameters.AddWithValue("@SenderId", model.SenderId);
                cmd.Parameters.AddWithValue("@ReceiverId", model.ReceiverId);
                cmd.Parameters.AddWithValue("@Message", model.Message);
                cmd.Parameters.AddWithValue("@MessageType", model.MessageType);
                cmd.Parameters.AddWithValue("@IsAdminReply", model.IsAdminReply ?? false);
                cmd.Parameters.AddWithValue("@IsRead", model.IsRead ?? false);
                cmd.Parameters.AddWithValue("@MessageId", DBNull.Value);

                return cmd.ExecuteScalar()?.ToString();
            }
        }

        
        public string UpdateChat(ChatModel model,string action)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("sp_ManageMessages", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ActionType",action);
                cmd.Parameters.AddWithValue("@MessageId", model.MessageId);
                cmd.Parameters.AddWithValue("@SenderId", model.SenderId);
                cmd.Parameters.AddWithValue("@ReceiverId", model.ReceiverId);
                cmd.Parameters.AddWithValue("@Message", model.Message);
                cmd.Parameters.AddWithValue("@MessageType", model.MessageType);
                cmd.Parameters.AddWithValue("@IsAdminReply", model.IsAdminReply ?? false);
                cmd.Parameters.AddWithValue("@IsRead", model.IsRead ?? (object)DBNull.Value);

                return cmd.ExecuteScalar()?.ToString();
            }
        }

        
        public string DeleteChat(int messageId,string action)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("sp_ManageMessages", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ActionType", action);
                cmd.Parameters.AddWithValue("@MessageId", messageId);
                cmd.Parameters.AddWithValue("@SenderId", DBNull.Value);
                cmd.Parameters.AddWithValue("@ReceiverId", DBNull.Value);
                cmd.Parameters.AddWithValue("@Message", DBNull.Value);
                cmd.Parameters.AddWithValue("@MessageType", DBNull.Value);
                cmd.Parameters.AddWithValue("@IsAdminReply", DBNull.Value);

                return cmd.ExecuteScalar()?.ToString();
            }
        }

        
        public List<ChatModel> GetChatHistory(int senderId, int receiverId)
        {
            List<ChatModel> chatHistory = new List<ChatModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(@"SELECT * FROM tbl_ChatMessages
                                                  WHERE (SenderId = @SenderId AND ReceiverId = @ReceiverId)
                                                  OR (SenderId = @ReceiverId AND ReceiverId = @SenderId)
                                                  ORDER BY SentOn ASC", connection);

                cmd.Parameters.AddWithValue("@SenderId", senderId);
                cmd.Parameters.AddWithValue("@ReceiverId", receiverId);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    chatHistory.Add(new ChatModel
                    {
                        MessageId = Convert.ToInt32(reader["MessageId"]),
                        SenderId = reader["SenderId"] != DBNull.Value ? Convert.ToInt32(reader["SenderId"]) : (int?)null,
                        ReceiverId = reader["ReceiverId"] != DBNull.Value ? Convert.ToInt32(reader["ReceiverId"]) : (int?)null,
                        Message = reader["Message"].ToString(),
                        MessageType = reader["MessageType"].ToString(),
                        IsAdminReply = reader["IsAdminReply"] != DBNull.Value ? Convert.ToBoolean(reader["IsAdminReply"]) : (bool?)null,
                        IsRead = reader["IsRead"] != DBNull.Value ? Convert.ToBoolean(reader["IsRead"]) : (bool?)null,
                        SentOn = reader["SentOn"] != DBNull.Value ? Convert.ToDateTime(reader["SentOn"]) : (DateTime?)null
                    });
                }
            }

            return chatHistory;
        }
        public List<ChatModel> SelectAllMessages(string action)
        {
            List<ChatModel> messages = new List<ChatModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("sp_GetMessages", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ActionType", action);
                cmd.Parameters.AddWithValue("@MessageId", DBNull.Value);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    messages.Add(new ChatModel
                    {
                        MessageId = Convert.ToInt32(reader["MessageId"]),
                        SenderId = reader["SenderId"] != DBNull.Value ? Convert.ToInt32(reader["SenderId"]) : (int?)null,
                        ReceiverId = reader["ReceiverId"] != DBNull.Value ? Convert.ToInt32(reader["ReceiverId"]) : (int?)null,
                        Message = reader["Message"].ToString(),
                        MessageType = reader["MessageType"].ToString(),
                        IsAdminReply = reader["IsAdminReply"] != DBNull.Value ? Convert.ToBoolean(reader["IsAdminReply"]) : (bool?)null,
                        SentOn = reader["SentOn"] != DBNull.Value ? Convert.ToDateTime(reader["SentOn"]) : (DateTime?)null
                    });
                }
            }

            return messages;
        }
        public Dictionary<int, int> GetUnreadMessageCounts()
        {
            var unreadCounts = new Dictionary<int, int>();

            string query = @"
        SELECT ReceiverId, COUNT(*) AS UnreadCount 
        FROM tbl_ChatMessages 
        WHERE IsRead = 0 AND IsAdminReply = 0
        GROUP BY ReceiverId";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int userId = Convert.ToInt32(reader["ReceiverId"]);
                            int count = Convert.ToInt32(reader["UnreadCount"]);
                            unreadCounts[userId] = count;
                        }
                    }
                }
            }

            return unreadCounts;
        }
        public Dictionary<int, int> GetUnreadMessageCountsForAdmin(int adminId)
        {
            var unreadCounts = new Dictionary<int, int>();

            string query = @"
            SELECT SenderId, COUNT(*) AS UnreadCount 
            FROM tbl_ChatMessages 
            WHERE ReceiverId = @AdminId AND IsRead = 0 AND IsAdminReply = 0
            GROUP BY SenderId";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AdminId", adminId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int userId = Convert.ToInt32(reader["SenderId"]);
                            int count = Convert.ToInt32(reader["UnreadCount"]);
                            unreadCounts[userId] = count;
                        }
                    }
                }
            }

            return unreadCounts;
        }

        public void MarkMessagesAsRead(int senderId, int receiverId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(@"
            UPDATE tbl_ChatMessages 
            SET IsRead = 1 
            WHERE SenderId = @SenderId AND ReceiverId = @ReceiverId AND IsRead = 0", connection);

                cmd.Parameters.AddWithValue("@SenderId", senderId);
                cmd.Parameters.AddWithValue("@ReceiverId", receiverId);

                cmd.ExecuteNonQuery();
            }
        }

    }
}


