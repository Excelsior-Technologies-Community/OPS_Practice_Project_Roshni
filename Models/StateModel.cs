using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace OPS_Practice_Project.Models
{
    public class StateModel
    {
        [Key]
        public long StateId { get; set; }

        [Required, StringLength(500)]
        public string? State { get; set; }

        public char? Flag { get; set; }
        public long? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }

        [ForeignKey("CountryId")]
        public long CountryId { get; set; }
        public CountryModel? Country { get; set; }
    }
}
