using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace skgRestApi.Models;

/// <summary>
/// Web Push 구독 정보를 저장하는 엔티티
/// </summary>
public class PushSubscription
{
  /// <summary>
  /// Push Service Endpoint URL (PK)
  /// </summary>
  [Key]
  public string Endpoint { get; set; } = string.Empty;
  /// <summary>
  /// P256DH 키
  /// </summary>
  public string P256dh { get; set; } = string.Empty;
  /// <summary>
  /// 인증 키
  /// </summary>
  public string Auth { get; set; } = string.Empty;

  /// <summary>
  /// 사용자 ID 
  /// </summary>
  public int UserId { get; set; }

}