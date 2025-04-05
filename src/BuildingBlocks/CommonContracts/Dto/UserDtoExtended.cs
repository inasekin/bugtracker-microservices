namespace CommonContracts.Dto
{
  /// <summary>
  /// Расширенный DTO пользователя для взаимодействия между сервисами
  /// </summary>
  public class UserDtoExtended
  {
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "Guest";
    public DateTime CreatedAt { get; set; }
  }
}
