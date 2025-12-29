using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace skgRestApi.Models;

/// <summary>
/// 로그인 시도 기록을 저장하는 엔티티
/// </summary>
[Table("LoginHistories")]
public class LoginHistory
{
    [Key]
    public long Id { get; set; }

    [Required]
    [StringLength(256)]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public DateTime LoginTime { get; set; } = DateTime.UtcNow;

    [StringLength(45)] // IPv6 주소를 고려한 길이
    public string? IpAddress { get; set; }

    [Required]
    public bool Success { get; set; }

    public string? FailureReason { get; set; }
}