using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Disha.Web.Models.Entities
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("username")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(150)]
        [Column("full_name")]
        public string? FullName { get; set; }

        [Required]
        [Column("role")]
        public string Role { get; set; } = "student"; // 'student' or 'admin'

        [MaxLength(50)]
        [Column("education_level")]
        public string? EducationLevel { get; set; }

        [Column("prior_computer_exposure")]
        public string PriorComputerExposure { get; set; } = "none"; // 'none', 'some', 'a_lot'

        [MaxLength(100)]
        [Column("state")]
        public string? State { get; set; }

        [MaxLength(10)]
        [Column("language_pref")]
        public string LanguagePref { get; set; } = "en";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
