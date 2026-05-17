using Newtonsoft.Json.Linq;
using NexOrder.UserService.Application.ApiTypes;
using NexOrder.UserService.Application.Services;
using Polly;
using Polly.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NexOrder.UserService.Infrastructure.HttpClients
{
    public class AuthServiceClient : IAuthServiceClient
    {
        private readonly HttpClient _httpClient;

        private readonly ResiliencePipeline pipeline;
        public AuthServiceClient(HttpClient httpClient, ResiliencePipelineProvider<string> pipelineProvider)
        {
            this._httpClient = httpClient;
            this.pipeline = pipelineProvider.GetPipeline("authservice-pipeline");
        }
        public async Task<AuthTokenResult> GenerateTokenAsync(string username, Guid userId)
        {
            var payload = new
            {
                Email = username,
                UserId = userId
            };

            return await this.pipeline.ExecuteAsync(async token =>
            {
                // Create a fresh HttpRequestMessage for each attempt because
                // HttpRequestMessage/HttpContent cannot be reused across sends.
                using var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    "v1/authservice/generate-token")
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(payload),
                        Encoding.UTF8,
                        "application/json")
                };

                var response = await _httpClient.SendAsync(request, token);

                // Throw on server errors so the resilience retry can handle them.
                if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    throw new HttpRequestException("Auth service encountered an internal error.");
                }

                if (response.IsSuccessStatusCode)
                {
                    var tokenResult = await response.Content.ReadFromJsonAsync<TokenResult>();
                    return new AuthTokenResult(true, tokenResult!.Token, string.Empty);
                }

                // For other non-success status codes, return the error payload (no retry).
                var errorMessage = await response.Content.ReadFromJsonAsync<string>();
                return new AuthTokenResult(false, null, errorMessage);
            });
        }
    }
}
