using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Disha.Web.Models.Entities
{
    [Table("quiz_questions")]
    public class QuizQuestion
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(300)]
        [Column("question_text")]
        public string QuestionText { get; set; } = string.Empty;

        [Required]
        [Column("order_index")]
        public int OrderIndex { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        public ICollection<QuizOption> Options { get; set; } = new List<QuizOption>();
    }
}
