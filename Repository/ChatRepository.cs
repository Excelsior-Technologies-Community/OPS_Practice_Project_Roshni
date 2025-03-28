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

    }
}


