using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Disha.Web.Models.Entities
{
    [Table("career_tags")]
    public class CareerTag
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("career_id")]
        public int CareerId { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("tag")]
        public string Tag { get; set; } = string.Empty;

        [ForeignKey(nameof(CareerId))]
        public Career? Career { get; set; }
    }
}
