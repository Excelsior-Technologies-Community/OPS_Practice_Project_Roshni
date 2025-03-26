using System.ComponentModel.DataAnnotations;

namespace OPS_Practice_Project.Models
{
    public class UserTypeModel
    {
        [Key]
        public long Id { get; set; }

        [Required, StringLength(500)]
        public string? UserType { get; set; }

        public char? Flag { get; set; }
        public long? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
