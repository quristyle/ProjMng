using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using skgRestApi.Services;

namespace skgRestApi.Endpoints;

/// <summary>
/// 관리자 권한을 확인하는 엔드포인트 필터
/// </summary>
public class AdminAuthFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var principal = context.HttpContext.User;

        // 'role' 클레임이 'admin'인지 확인합니다.
        // JwtMiddleware에서 개발 모드일 때 임시 사용자에 대해 ClaimTypes.Role을 "admin"으로 설정하므로,
        // 개발 환경에서도 이 체크를 정상적으로 통과합니다.
        if (principal?.IsInRole("admin") != true)
        {
            return Results.Json(new { message = "관리자 권한이 없습니다." }, statusCode: StatusCodes.Status403Forbidden);
        }

        // 모든 검증 통과 시 다음 미들웨어/엔드포인트 실행
        return await next(context);
    }
}