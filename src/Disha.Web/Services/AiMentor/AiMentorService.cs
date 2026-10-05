using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Disha.Web.Data;
using Disha.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Disha.Web.Services.AiMentor
{
    public class AiMentorService : IAiMentorService
    {
        private readonly DishaDbContext _context;
        private readonly GroqMentorProvider _groqProvider;
        private readonly GeminiMentorProvider _geminiProvider;
        private readonly IConfiguration _config;
        private readonly ILogger<AiMentorService> _logger;

        private const string DefaultSystemPrompt = @"You are the AI Mentor inside ""Disha,"" a career guidance app for underprivileged students exploring IT careers in India. Your student may be a first-generation learner with limited resources. Rules:
- Use simple, warm, encouraging, jargon-free language. Explain any technical term the first time you use it.
- Only discuss IT career paths, learning resources, realistic timelines, study tips, and encouragement. Do not give financial, legal, or medical advice.
- Always be honest that you are an AI assistant, not a human.
- If the student's message suggests personal distress unrelated to careers, gently encourage them to talk to a trusted adult, teacher, or counsellor rather than trying to handle it yourself.
- Prefer short, concrete answers with a clear next step over long essays.";

        public AiMentorService(
            DishaDbContext context,
            GroqMentorProvider groqProvider,
            GeminiMentorProvider geminiProvider,
            IConfiguration config,
            ILogger<AiMentorService> logger)
        {
            _context = context;
            _groqProvider = groqProvider;
            _geminiProvider = geminiProvider;
            _config = config;
            _logger = logger;
        }

        public async Task<List<MentorChatMessage>> GetChatHistoryAsync(int userId, int limit = 20)
        {
            return await _context.MentorChatMessages
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .Take(limit)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<string> SendMessageAsync(int userId, string studentMessage)
        {
            if (string.IsNullOrWhiteSpace(studentMessage))
            {
                return "Please enter a question or topic you'd like guidance on!";
            }

            // Save user message first
            var userMsgEntity = new MentorChatMessage
            {
                UserId = userId,
                Sender = "user",
                Message = studentMessage.Trim(),
                CreatedAt = DateTime.UtcNow
            };
            _context.MentorChatMessages.Add(userMsgEntity);
            await _context.SaveChangesAsync();

            // Fetch last 10 messages for context
            var history = await GetChatHistoryAsync(userId, 10);

            string systemPrompt = _config["AiMentor:SystemPrompt"] ?? DefaultSystemPrompt;
            string aiReply = string.Empty;
            string providerUsed = "static";

            // 1. Try Groq Provider with 10s timeout
            try
            {
                using var ctsGroq = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                aiReply = await _groqProvider.GenerateReplyAsync(systemPrompt, history, studentMessage, ctsGroq.Token);
                if (!string.IsNullOrWhiteSpace(aiReply))
                {
                    providerUsed = "groq";
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Groq Mentor provider failed or timed out. Falling back to Gemini...");
            }

            // 2. Fallback to Gemini Provider if Groq failed
            if (string.IsNullOrWhiteSpace(aiReply))
            {
                try
                {
                    using var ctsGemini = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                    aiReply = await _geminiProvider.GenerateReplyAsync(systemPrompt, history, studentMessage, ctsGemini.Token);
                    if (!string.IsNullOrWhiteSpace(aiReply))
                    {
                        providerUsed = "gemini";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Gemini Mentor provider failed or timed out.");
                }
            }

            // 3. Friendly static fallback if both fail or unconfigured
            if (string.IsNullOrWhiteSpace(aiReply))
            {
                providerUsed = "fallback";
                aiReply = "Hello! I am your Disha AI Mentor. I'm currently offline or updating my learning system. In the meantime, I recommend exploring our 'Recommended IT Tracks' or taking the Assessment Quiz to find your best IT career path!";
            }

            // Save AI reply to database
            var aiMsgEntity = new MentorChatMessage
            {
                UserId = userId,
                Sender = "ai",
                Message = aiReply,
                ProviderUsed = providerUsed,
                CreatedAt = DateTime.UtcNow
            };
            _context.MentorChatMessages.Add(aiMsgEntity);
            await _context.SaveChangesAsync();

            return aiReply;
        }
    }
}
