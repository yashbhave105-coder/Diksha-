using Disha.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Disha.Web.Data
{
    public class DishaDbContext : DbContext
    {
        public DishaDbContext(DbContextOptions<DishaDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Career> Careers => Set<Career>();
        public DbSet<CareerTag> CareerTags => Set<CareerTag>();
        public DbSet<QuizQuestion> QuizQuestions => Set<QuizQuestion>();
        public DbSet<QuizOption> QuizOptions => Set<QuizOption>();
        public DbSet<QuizResponse> QuizResponses => Set<QuizResponse>();
        public DbSet<FinancialAid> FinancialAids => Set<FinancialAid>();
        public DbSet<Bookmark> Bookmarks => Set<Bookmark>();
        public DbSet<MentorChatMessage> MentorChatMessages => Set<MentorChatMessage>();
        public DbSet<ChecklistItem> ChecklistItems => Set<ChecklistItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();
            });

            modelBuilder.Entity<Bookmark>(entity =>
            {
                entity.HasIndex(b => new { b.UserId, b.ItemType, b.ItemId })
                      .HasDatabaseName("uq_bookmark")
                      .IsUnique();
            });

            modelBuilder.Entity<CareerTag>(entity =>
            {
                entity.HasOne(ct => ct.Career)
                      .WithMany(c => c.Tags)
                      .HasForeignKey(ct => ct.CareerId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<QuizOption>(entity =>
            {
                entity.HasOne(qo => qo.Question)
                      .WithMany(q => q.Options)
                      .HasForeignKey(qo => qo.QuestionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<QuizResponse>(entity =>
            {
                entity.HasOne(qr => qr.User)
                      .WithMany()
                      .HasForeignKey(qr => qr.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Bookmark>(entity =>
            {
                entity.HasOne(b => b.User)
                      .WithMany()
                      .HasForeignKey(b => b.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MentorChatMessage>(entity =>
            {
                entity.HasOne(m => m.User)
                      .WithMany()
                      .HasForeignKey(m => m.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ChecklistItem>(entity =>
            {
                entity.HasOne(c => c.User)
                      .WithMany()
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
