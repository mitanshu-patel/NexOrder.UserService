using Newtonsoft.Json.Linq;
using NexOrder.UserService.Application.ApiTypes;
using NexOrder.UserService.Application.Services;
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
        public AuthServiceClient(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }
        public async Task<AuthTokenResult> GenerateTokenAsync(string username)
        {
            var payload = new
            {
                Email = username
            };

            var request = new HttpRequestMessage(
            HttpMethod.Post,
            "v1/authservice/generate-token")
            {

                Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            { 
                var tokenResult = await response.Content.ReadFromJsonAsync<TokenResult>();
                return new AuthTokenResult(true, tokenResult.Token, string.Empty);
            }

            if(response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return new AuthTokenResult(false, null, "Auth service encountered an internal error.");
            }

            var abc = await response.Content.ReadFromJsonAsync<object>();

            var errorMessage = await response.Content.ReadFromJsonAsync<string>();
            return new AuthTokenResult(false, null, errorMessage);

        }
    }
}
