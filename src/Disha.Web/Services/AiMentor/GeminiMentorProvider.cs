using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Disha.Web.Models.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Disha.Web.Services.AiMentor
{
    public class GeminiMentorProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<GeminiMentorProvider> _logger;

        public GeminiMentorProvider(HttpClient httpClient, IConfiguration config, ILogger<GeminiMentorProvider> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
        }

        public async Task<string> GenerateReplyAsync(string systemPrompt, List<MentorChatMessage> history, string newMessage, CancellationToken cancellationToken)
        {
            string apiKey = _config["AiMentor:GeminiApiKey"] ?? string.Empty;
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("Gemini API Key is not configured.");
            }

            var contents = new List<object>();

            // System prompt as first instruction/user context
            contents.Add(new
            {
                role = "user",
                parts = new[] { new { text = $"[System Instruction]: {systemPrompt}" } }
            });
            contents.Add(new
            {
                role = "model",
                parts = new[] { new { text = "Understood. I am Disha AI Mentor, here to support the student warmly with IT career guidance." } }
            });

            foreach (var msg in history)
            {
                contents.Add(new
                {
                    role = msg.Sender == "user" ? "user" : "model",
                    parts = new[] { new { text = msg.Message } }
                });
            }

            contents.Add(new
            {
                role = "user",
                parts = new[] { new { text = newMessage } }
            });

            var payload = new { contents = contents };
            var requestJson = JsonSerializer.Serialize(payload);

            string endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}";
            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
            };

            _logger.LogInformation("Sending fallback request to Gemini API...");
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(responseJson);

            string reply = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? string.Empty;

            return reply.Trim();
        }
    }
}
