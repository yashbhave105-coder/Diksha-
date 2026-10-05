using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Disha.Web.Models.Entities
{
    [Table("quiz_responses")]
    public class QuizResponse
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("question_id")]
        public int QuestionId { get; set; }

        [Required]
        [Column("option_id")]
        public int OptionId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        [ForeignKey(nameof(QuestionId))]
        public QuizQuestion? Question { get; set; }

        [ForeignKey(nameof(OptionId))]
        public QuizOption? Option { get; set; }
    }
}
