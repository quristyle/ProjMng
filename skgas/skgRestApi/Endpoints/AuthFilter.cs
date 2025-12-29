using skgRestApi.Services;
using Microsoft.AspNetCore.Http; // StatusCodes를 사용하기 위해 추가

namespace skgRestApi.Endpoints;

public class AuthFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        
        var jwtCheckType = context.HttpContext.Items["JwtCheckType"];

        if (jwtCheckType == null)
        {
            return Results.Json(new { message = "권한 인증 실패! Request의 Token 값이 없습니다." }, statusCode: StatusCodes.Status401Unauthorized);
        }

        var checkType = (JwtCheckType)jwtCheckType;

        switch (checkType)
        {
            case JwtCheckType.Success:
                // UserId가 있는지 추가로 확인
                if (context.HttpContext.Items["UserId"] == null)
                {
                    return Results.Json(new { message = "권한이 없습니다. (유효하지 않은 토큰)" }, statusCode: StatusCodes.Status401Unauthorized);
                }
                // 성공 시 다음 필터 또는 엔드포인트 핸들러 실행
                return await next(context);

            case JwtCheckType.ValidExpires:
                return Results.Json(new { message = "토큰 인증시간 만료." }, statusCode: StatusCodes.Status401Unauthorized);

            case JwtCheckType.NotYetValid:
                return Results.Json(new { message = "토큰이 아직 유효하지 않습니다. 잠시 후 다시 시도해주세요." }, statusCode: StatusCodes.Status403Forbidden);

            case JwtCheckType.Forgery:
                return Results.Json(new { message = "권한 인증 실패! 위조된 토큰입니다." }, statusCode: StatusCodes.Status401Unauthorized);

            case JwtCheckType.NotJwt:
                return Results.Json(new { message = "권한 인증 실패! Request의 Token 값이 없습니다." }, statusCode: StatusCodes.Status401Unauthorized);

            case JwtCheckType.JwtKeyEmpty:
                return Results.Json(new { message = "권한 인증 실패! JWT SecretKey키 값이 없습니다." }, statusCode: StatusCodes.Status401Unauthorized);

            default:
                return Results.Json(new { message = "알 수 없는 인증 오류가 발생했습니다." }, statusCode: StatusCodes.Status401Unauthorized);
        }
    }
}