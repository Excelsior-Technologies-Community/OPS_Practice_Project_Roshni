namespace OPS_Practice_Project.Models
{
    public class ChatModel
    {
        public int MessageId { get; set; }
        public int? SenderId { get; set; }
        public int? ReceiverId { get; set; }
        public string Message { get; set; }
        public string MessageType { get; set; }
        public bool? IsAdminReply { get; set; }
        public DateTime? SentOn { get; set; }
    }
}
