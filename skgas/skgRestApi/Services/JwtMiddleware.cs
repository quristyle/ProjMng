using skgRestApi.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;


namespace skgRestApi.Middlewares;

/// <summary>
/// JWToken 인증 체크 미들웨어
/// </summary>
public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IJwtService jwtService, IWebHostEnvironment env)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (env.IsDevelopment())
        {
            context.Items["JwtCheckType"] = JwtCheckType.Success;
            context.Items["UserId"] = "dev_user"; // 개발용 임시 사용자 ID

            // 개발 환경에서는 편의상 admin 권한을 가진 사용자로 설정
            var claims = new List<Claim>
            {
                new Claim("id", "dev_user"),
                new Claim(ClaimTypes.Role, "admin")
            };
            var identity = new ClaimsIdentity(claims, "Development");
            context.User = new ClaimsPrincipal(identity);

            await _next(context);
            return;
        }

        var auth = context.Request.Headers["Authorization"].FirstOrDefault();
        if (auth != null && auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var token = auth.Substring("Bearer ".Length).Trim();

            // 토큰 검증
            var (jwtCheckType, claimsPrincipal) = await jwtService.ValidateJwtTokenAsync(token);

            // 검증 결과와 사용자 ID를 HttpContext에 저장
            context.Items["JwtCheckType"] = jwtCheckType;
            if (jwtCheckType == JwtCheckType.Success && claimsPrincipal != null)
            {
                var userId = claimsPrincipal.FindFirstValue("id");
                context.Items["UserId"] = userId;

                // 검증된 사용자 정보를 HttpContext.User에 할당
                context.User = claimsPrincipal;
            }
        }
        
        await _next(context);
    }
}