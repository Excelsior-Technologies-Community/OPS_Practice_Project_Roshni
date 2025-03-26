using Mono.TextTemplating;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OPS_Practice_Project.Models
{
    public class CityModel
    {
        [Key]
        public long CityId { get; set; }

        [Required, StringLength(500)]
        public string? City { get; set; }

        public char? Flag { get; set; }
        public long? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }

        [ForeignKey("StateId")]
        public long StateId { get; set; }
        public StateModel? State { get; set; }
    }
}
