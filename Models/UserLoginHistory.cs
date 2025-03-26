namespace OPS_Practice_Project.Models
{
    public class UserLoginHistory
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; } // Nullable if not always set
        public string IPAddress { get; set; }  // Add this
        public string UserAgent { get; set; }  // Add this
        public string Status { get; set; }
    }
}
