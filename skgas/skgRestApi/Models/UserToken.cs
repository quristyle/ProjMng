using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace skgRestApi.Models;

/// <summary>
/// Refresh Token 정보를 저장하는 엔티티
/// </summary>
[Table("UserTokens")]
public class UserToken
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [StringLength(128)]
    public string Token { get; set; } = string.Empty;

    [Required]
    public DateTime Expires { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public DateTime? Revoked { get; set; }

    [StringLength(128)]
    public string? ReplacedByToken { get; set; }

    public bool IsExpired => DateTime.UtcNow >= Expires;

    public bool IsRevoked => Revoked != null;

    public bool IsActive => !IsRevoked && !IsExpired;
}