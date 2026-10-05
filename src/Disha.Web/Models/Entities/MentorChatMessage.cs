using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Disha.Web.Models.Entities
{
    [Table("mentor_chat_messages")]
    public class MentorChatMessage
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("sender")]
        public string Sender { get; set; } = "user"; // 'user' or 'ai'

        [Required]
        [Column("message")]
        public string Message { get; set; } = string.Empty;

        [MaxLength(20)]
        [Column("provider_used")]
        public string? ProviderUsed { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
    }
}
