using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Disha.Web.Models.Entities
{
    [Table("bookmarks")]
    public class Bookmark
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("item_type")]
        public string ItemType { get; set; } = "career"; // 'career' or 'aid'

        [Required]
        [Column("item_id")]
        public int ItemId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
    }
}
