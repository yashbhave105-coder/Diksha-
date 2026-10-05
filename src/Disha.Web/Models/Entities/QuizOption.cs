using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Disha.Web.Models.Entities
{
    [Table("quiz_options")]
    public class QuizOption
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("question_id")]
        public int QuestionId { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("option_text")]
        public string OptionText { get; set; } = string.Empty;

        [Required]
        [Column("tag_weights", TypeName = "json")]
        public string TagWeightsJson { get; set; } = "{}";

        [ForeignKey(nameof(QuestionId))]
        public QuizQuestion? Question { get; set; }
    }
}
