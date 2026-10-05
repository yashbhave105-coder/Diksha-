using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Disha.Web.Data;
using Disha.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Disha.Web.Services
{
    public class BookmarkService : IBookmarkService
    {
        private readonly DishaDbContext _context;

        public BookmarkService(DishaDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ToggleBookmarkAsync(int userId, string itemType, int itemId)
        {
            var existing = await _context.Bookmarks
                .FirstOrDefaultAsync(b => b.UserId == userId && b.ItemType == itemType && b.ItemId == itemId);

            if (existing != null)
            {
                _context.Bookmarks.Remove(existing);
                await _context.SaveChangesAsync();
                return false; // Removed
            }
            else
            {
                _context.Bookmarks.Add(new Bookmark
                {
                    UserId = userId,
                    ItemType = itemType,
                    ItemId = itemId,
                    CreatedAt = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
                return true; // Added
            }
        }

        public async Task<bool> IsBookmarkedAsync(int userId, string itemType, int itemId)
        {
            return await _context.Bookmarks
                .AnyAsync(b => b.UserId == userId && b.ItemType == itemType && b.ItemId == itemId);
        }

        public async Task<List<Career>> GetBookmarkedCareersAsync(int userId)
        {
            var careerIds = await _context.Bookmarks
                .Where(b => b.UserId == userId && b.ItemType == "career")
                .Select(b => b.ItemId)
                .ToListAsync();

            return await _context.Careers
                .Where(c => careerIds.Contains(c.Id))
                .Include(c => c.Tags)
                .ToListAsync();
        }

        public async Task<List<FinancialAid>> GetBookmarkedAidAsync(int userId)
        {
            var aidIds = await _context.Bookmarks
                .Where(b => b.UserId == userId && b.ItemType == "aid")
                .Select(b => b.ItemId)
                .ToListAsync();

            return await _context.FinancialAids
                .Where(a => aidIds.Contains(a.Id))
                .ToListAsync();
        }

        public async Task<int> GetBookmarkCountAsync(int userId)
        {
            return await _context.Bookmarks.CountAsync(b => b.UserId == userId);
        }
    }
}
