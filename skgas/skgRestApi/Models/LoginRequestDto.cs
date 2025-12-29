using System.ComponentModel.DataAnnotations;

namespace skgRestApi.Models;

/// <summary>
/// 로그인 요청을 위한 DTO
/// </summary>
public class LoginRequest
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class RefreshRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}