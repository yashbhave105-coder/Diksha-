using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Disha.Web.Models.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Disha.Web.Services.AiMentor
{
    public class GroqMentorProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<GroqMentorProvider> _logger;

        public GroqMentorProvider(HttpClient httpClient, IConfiguration config, ILogger<GroqMentorProvider> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
        }

        public async Task<string> GenerateReplyAsync(string systemPrompt, List<MentorChatMessage> history, string newMessage, CancellationToken cancellationToken)
        {
            string apiKey = _config["AiMentor:GroqApiKey"] ?? string.Empty;
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("Groq API Key is not configured.");
            }

            var messages = new List<object>
            {
                new { role = "system", content = systemPrompt }
            };

            foreach (var msg in history)
            {
                messages.Add(new
                {
                    role = msg.Sender == "user" ? "user" : "assistant",
                    content = msg.Message
                });
            }

            messages.Add(new { role = "user", content = newMessage });

            var payload = new
            {
                model = "llama-3.1-8b-instant",
                messages = messages,
                temperature = 0.7,
                max_tokens = 500
            };

            var requestJson = JsonSerializer.Serialize(payload);
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions")
            {
                Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            _logger.LogInformation("Sending request to Groq API...");
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(responseJson);

            string reply = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;

            return reply.Trim();
        }
    }
}
