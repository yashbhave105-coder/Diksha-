using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Disha.Web.Models.Entities
{
    [Table("financial_aid")]
    public class FinancialAid
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        [Column("provider")]
        public string Provider { get; set; } = string.Empty;

        [Required]
        [Column("description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column("eligibility")]
        public string Eligibility { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("amount")]
        public string? Amount { get; set; }

        [MaxLength(300)]
        [Column("apply_url")]
        public string? ApplyUrl { get; set; }

        [Column("applicable_states", TypeName = "json")]
        public string? ApplicableStatesJson { get; set; }

        [Column("applicable_classes", TypeName = "json")]
        public string? ApplicableClassesJson { get; set; }

        [Column("last_verified_date")]
        public DateTime? LastVerifiedDate { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;
    }
}
