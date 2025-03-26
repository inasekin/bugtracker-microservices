using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Domain.Models
{
  [Table("users")]
  public class UserResponse
  {
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required]
    [MaxLength(100)]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    /// <summary>
    /// Роль пользователя в системе
    /// </summary>
    public UserRole Role { get; set; } = UserRole.Guest;

    /// <summary>
    /// Дата создания пользователя
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата последнего обновления записи пользователя
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
  }
}
