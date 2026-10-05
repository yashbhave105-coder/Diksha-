using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Disha.Web.Models.Entities
{
    [Table("careers")]
    public class Career
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(300)]
        [Column("summary")]
        public string Summary { get; set; } = string.Empty;

        [Required]
        [Column("description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column("roadmap", TypeName = "json")]
        public string RoadmapJson { get; set; } = "[]";

        [MaxLength(100)]
        [Column("avg_salary_range")]
        public string? AvgSalaryRange { get; set; }

        [MaxLength(100)]
        [Column("duration_to_start")]
        public string? DurationToStart { get; set; }

        [MaxLength(100)]
        [Column("cost_estimate")]
        public string? CostEstimate { get; set; }

        [MaxLength(50)]
        [Column("demand_badge")]
        public string? DemandBadge { get; set; }

        [MaxLength(50)]
        [Column("icon")]
        public string? Icon { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        public ICollection<CareerTag> Tags { get; set; } = new List<CareerTag>();
    }
}
