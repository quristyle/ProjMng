using System.Collections;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;



using skgRestApi.Models;
using Microsoft.AspNetCore.Routing;
using System.Dynamic;
using Microsoft.EntityFrameworkCore;
using skgRestApi.Data;
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace skgRestApi.Models;

/// <summary>
/// API 응답을 위한 표준 래퍼 클래스
/// </summary>
/// <typeparam name="T">데이터 페이로드의 타입</typeparam>
public class ApiResponse<T> {
  /// <summary>요청 성공 여부</summary>
  public bool Success { get; set; }

  /// <summary>응답 메시지</summary>
  public string Message { get; set; }
 // public T? Columns { get; set; }

  /// <summary>실제 데이터</summary>
  public T? ResData { get; set; }

  /// <summary>부가 정보</summary>
  public Metadata? Meta { get; set; }

  /// <summary>
  /// ApiResponse의 새 인스턴스를 초기화합니다.
  /// </summary>
  /// <param name="success">요청 성공 여부</param>
  /// <param name="message">응답 메시지</param>
  /// <param name="ResData">실제 데이터</param>
  /// <param name="meta">부가 정보</param>
  public ApiResponse(bool success, string message, T? reqs, Metadata? meta) {
    Success = success;
    Message = message;
    ResData = reqs;
    Meta = meta;
  }
  public ApiResponse(bool success, string message, Metadata? meta) {
    Success = success;
    Message = message;
    Meta = meta;
  }

}

  /// <summary>
  /// 응답에 대한 부가 정보
  /// </summary>
  public class Metadata {
    /// <summary>요청 시작 시간 (UTC)</summary>
    public string RequestTime { get; set; } = string.Empty;

    /// <summary>요청 완료 시간 (UTC)</summary>
    public string CompletionTime { get; set; } = string.Empty;

    /// <summary>총 처리 소요 시간</summary>
    public string Duration { get; set; } = string.Empty;

    /// <summary>데이터 행의 수</summary>
    public int? RowCount { get; set; }

    /// <summary>데이터 열(속성)의 수</summary>
    public int? ColumnCount { get; set; }
  }

/// <summary>
/// 표준 API 응답 객체를 생성하는 헬퍼 클래스
/// </summary>
/// <remarks>
/// 이 클래스는 API 응답을 일관된 형식으로 래핑하고, 메타데이터를 추가하며, 민감한 정보를 필터링하는 역할을 합니다.
/// </remarks>
public static class ApiResponseBuilder {
  private static readonly HashSet<string> SensitiveFieldNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
  {
            "passwordHash"
        };

  private const int MaxTraverseDepth = 5;
  /// <summary>
  /// 비동기 작업을 실행하고 결과를 표준 API 응답 형식으로 래핑합니다.
  /// </summary>
  /// <typeparam name="T">반환될 데이터의 타입</typeparam>
  /// <param name="action">실행할 비동기 작업</param>
  /// <param name="successMessage">성공 시 메시지</param>
  /// <param name="successStatusCode">성공 시 HTTP 상태 코드</param>
  /// <returns>IResult 형태의 표준 API 응답</returns>
  public static async Task<IResult> CreateAsync<T>(Func<Task<Dictionary<string, ResultInfo<dynamic>?>>> action, string successMessage = "Request processed successfully.", int successStatusCode = 200) where T : class {
    var stopwatch = Stopwatch.StartNew();
    var requestTime = DateTime.UtcNow;

    try {
      Dictionary<string, ResultInfo<dynamic>?> reqs = await action();
      stopwatch.Stop();
      var completionTime = DateTime.UtcNow;

      if (reqs == null || reqs.Count == 0) {
        return Results.NotFound(new ApiResponse<object>(false, "Resource not found.", CreateMetadata(requestTime, completionTime, stopwatch.ElapsedMilliseconds)));
      }



      int? rowCount = null;
      int? colCount = null;

      var meta = CreateMetadata(requestTime, completionTime, stopwatch.ElapsedMilliseconds, rowCount, colCount);
      var response = new ApiResponse<object>(true, successMessage, reqs, meta);

      // 원본 data 객체에서 totalpagecount와 totalcount 추출
      //int? totalPageCount = null;
      //int? totalCount = null;


      return successStatusCode switch {
        201 => Results.Created(string.Empty, response),
        _ => Results.Ok(response),
      };
    }
    catch (Exception ex) {
      stopwatch.Stop();
      var completionTime = DateTime.UtcNow;
      // 실제 운영 환경에서는 ex.Message 대신 일반적인 오류 메시지를 사용하고, ex는 로깅.
      var response = new ApiResponse<object>(false, $"An error occurred: {ex.Message}", CreateMetadata(requestTime, completionTime, stopwatch.ElapsedMilliseconds));
      return Results.BadRequest(response);
    }
  }

  private static Metadata CreateMetadata(DateTime reqTime, DateTime compTime, long duration, int? rowCount = null, int? colCount = null) => new Metadata {
    RequestTime = reqTime.ToString("o"), // ISO 8601 format
    CompletionTime = compTime.ToString("o"),
    Duration = $"{duration}ms",
    RowCount = rowCount,
    ColumnCount = colCount
  };



}