using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CommonContracts.Auth
{
  /// <summary>
  /// Клиент для взаимодействия с сервисом пользователей
  /// </summary>
  public class UserServiceClient : IUserServiceClient
  {
    private readonly HttpClient _httpClient;

    public UserServiceClient(IHttpClientFactory httpClientFactory)
    {
      _httpClient = httpClientFactory.CreateClient("UserService");
    }

    /// <inheritdoc/>
    public async Task<JwtValidationResponse> ValidateTokenAsync(string token)
    {
      var requestContent = new StringContent(
        JsonConvert.SerializeObject(new JwtValidationRequest { Token = token }),
        Encoding.UTF8,
        "application/json");

      var response = await _httpClient.PostAsync("/api/user/auth/validate", requestContent);
      response.EnsureSuccessStatusCode();

      var content = await response.Content.ReadAsStringAsync();
      return JsonConvert.DeserializeObject<JwtValidationResponse>(content);
    }

    /// <inheritdoc/>
    public async Task<bool> HasRoleAsync(Guid userId, string role)
    {
      var response = await _httpClient.GetAsync($"/api/user/role/{userId}/has/{role}");

      if (!response.IsSuccessStatusCode)
      {
        return false;
      }

      var content = await response.Content.ReadAsStringAsync();
      return JsonConvert.DeserializeObject<bool>(content);
    }
  }
}
