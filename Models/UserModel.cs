using Mono.TextTemplating;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace OPS_Practice_Project.Models
{
    public class UserModel
    {
        [Key]
        public long Id { get; set; }

        [Required, StringLength(500)]
        public string? FirstName { get; set; }

        [StringLength(500)]
        public string? MiddleName { get; set; }

        [Required, StringLength(500)]
        public string? LastName { get; set; }
        public IFormFile? ProfileImagePath { get; set; }
        public string? ProfileImage { get; set; }

        [Required, StringLength(500), EmailAddress]
        public string? EmailAddress { get; set; }

        [Required, StringLength(500)]
        public string? MobileNumber { get; set; }

        [Required, StringLength(500)]
        public string? UserName { get; set; }

        [Required, StringLength(500)]
        public string? Password { get; set; }

        public string? Address { get; set; }

        [ForeignKey("CountryId")]
        public long CountryId { get; set; }
        public CountryModel? Country { get; set; }

        [ForeignKey("StateId")]
        public long StateId { get; set; }
        public StateModel? State { get; set; }

        [ForeignKey("CityId")]
        public long CityId { get; set; }
        public CityModel? City { get; set; }

        [ForeignKey("Id")]
        public long UserTypeId { get; set; }
        public UserTypeModel? UserType { get; set; }

        public char? Flag { get; set; }
        public long? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }

        [StringLength(10)]
        public string CaptchaCode { get; set; }

        [StringLength(100)]
        public string CaptchaHashKey { get; set; }

        public DateTime? CaptchaExpiry { get; set; }

        [StringLength(10)]
        public string CaptchaStatus { get; set; }

        public string? CountryName { get; set; }
        public string? StateName { get; set; }
        public string? CityName { get; set; }
        public string? UserTypeName { get; set; }

        public int UnreadCount { get; set; }
    }
}
