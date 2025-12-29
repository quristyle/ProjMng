using Microsoft.EntityFrameworkCore;
using skgRestApi.Models;
using Microsoft.AspNetCore.Mvc;
using skgRestApi.Services;
using System;
using System.Text.Json.Serialization;
using System.Reflection;
using System.Security.Claims;

namespace skgRestApi.Endpoints;

/// <summary>
/// 
/// </summary>
public static class DataEndpoints {
  /// <summary>
  /// 프로젝트 관련 엔드포인트를 애플리케이션에 매핑합니다.
  /// </summary>
  public static void MapDataEndpoints(this IEndpointRouteBuilder routes) {
    var group = routes.MapGroup("/api/data");


    group.MapPost("/", async (SysService _service, TestRequest reqs) => {
      return await ApiResponseBuilder.CreateAsync<dynamic>(async () => {
        // 여기 안에서 프로시저 호출 하여 결과를 리턴.
        Dictionary<string, ResultInfo<dynamic>?> rid = new Dictionary<string, ResultInfo<dynamic>?>();
        foreach(var req in reqs.requests)
        {
          rid.Add(req.rkep, await _service.GetDataAsync(req.proc, req.param));
        }
        return rid;
      });
    })
            .WithName("GetTestData")
            .WithSummary("ssssssssss (인증 필요)")
            .WithDescription("xxxxxxxxxxxxxxxxxxxx 조회합니다.")
            .AddEndpointFilter<AuthFilter>();





  }


public class TestRequest{
  public string userId {get; set;} = string.Empty;
  public string userType {get; set;} = string.Empty;
    public List<RequestItem> requests {get; set;} = new List<RequestItem>();

    /// <summary>
    /// 커스텀 바인딩: Request Body의 JSON 데이터와 HttpContext의 인증 정보를 조합하여 객체를 생성합니다.
    /// </summary>
    public static async ValueTask<TestRequest?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var req = await context.Request.ReadFromJsonAsync<TestRequest>();
        if (req != null)
        {
            if (context.Items["UserId"] is string userId)
            {
                req.userId = userId;
            }

            var role = context.User.FindFirst(ClaimTypes.Role)?.Value;
            if (!string.IsNullOrEmpty(role))
            {
                req.userType = role;
            }
        }
        return req;
    }
}


public record RequestItem(
    string rkep,
    string proc,
    IDictionary<string, string> param
  );

}