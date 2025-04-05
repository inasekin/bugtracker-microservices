namespace CommonContracts.Auth
{
  /// <summary>
  /// Запрос на валидацию JWT токена
  /// </summary>
  public class JwtValidationRequest
  {
    public string Token { get; set; } = string.Empty;
  }

  /// <summary>
  /// Ответ на запрос валидации JWT токена
  /// </summary>
  public class JwtValidationResponse
  {
    public bool IsValid { get; set; }
    public Guid? UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime? Expiration { get; set; }
  }
}
