using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using skgRestApi.Dtos;
using skgRestApi.Data;
using skgRestApi.Services;
using skgRestApi.Models;
using skgRestApi.Endpoints;

namespace skgRestApi.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication");

        group.MapPost("/login", async (
                [FromBody] LoginRequest loginRequest,
                IAuthService authService,
                IJwtService jwtService,
                ApplicationDbContext dbContext,
                IConfiguration configuration) =>
            {
                try
                {
                    var isValidUser = await authService.ValidateUserAsync(loginRequest.UserId, loginRequest.Password);
                    if (!isValidUser)
                    {
                        return Results.Unauthorized();
                    }
                }
                catch (Exception ex)
                {
                    // 로그인 시도 횟수 초과 등으로 인한 예외 처리
                    return Results.Json(new { message = ex.Message }, statusCode: StatusCodes.Status403Forbidden);
                }

                // 기존에 발급된 모든 활성 Refresh Token을 무효화 처리
                var existingTokens = await dbContext.UserTokens
                    .Where(rt => rt.UserId == loginRequest.UserId && rt.Revoked == null && rt.Expires > DateTime.UtcNow)
                    .ToListAsync();

                foreach (var token in existingTokens)
                {
                    token.Revoked = DateTime.UtcNow;
                }

                var (accessToken, accessTokenId) = jwtService.GenerateJwtToken(loginRequest.UserId);
                var refreshToken = jwtService.GenerateRefreshToken(loginRequest.UserId);
                refreshToken.Token = accessTokenId; // Access Token의 JTI를 Refresh Token의 Token 필드에 저장

                // DB에 Refresh Token 저장
                dbContext.UserTokens.Add(refreshToken);
                await dbContext.SaveChangesAsync();

                var expiresIn = configuration.GetValue<int>("Jwt:ExpiresInSeconds");

                return Results.Ok(new { 
                    accessToken, 
                    refreshToken = accessTokenId, // JTI를 Refresh Token으로 사용
                    expiresIn // 만료 시간(초) 추가
                });
            })
            .WithName("Login")
            .WithSummary("사용자 로그인 및 토큰 발급")
            .WithDescription("사용자 ID와 비밀번호로 인증하고 JWT 토큰을 발급받습니다. (예시 ID: testuser, PW: password123)")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/refresh", async (
                [FromBody] RefreshRequest refreshRequest,
                IJwtService jwtService,
                ApplicationDbContext dbContext,
                IConfiguration configuration) =>
            {
                var userToken = await dbContext.UserTokens.SingleOrDefaultAsync(u => u.Token == refreshRequest.RefreshToken);

                // 1. 토큰이 존재하지 않거나 만료된 경우
                if (userToken == null || userToken.IsExpired)
                {
                    return Results.Unauthorized();
                }

                // 2. 토큰이 이미 사용된 경우 (재사용 탐지)
                if (userToken.IsRevoked)
                {
                    // 재사용된 토큰으로부터 파생된 모든 토큰을 무효화
                    var allTokens = await dbContext.UserTokens
                        .Where(t => t.UserId == userToken.UserId && t.Revoked == null && t.Expires > DateTime.UtcNow)
                        .ToListAsync();

                    foreach (var token in allTokens) {
                        token.Revoked = DateTime.UtcNow;
                    }
                    await dbContext.SaveChangesAsync();
                    return Results.Unauthorized();
                }

                // 기존 Refresh Token 무효화 (회전)
                var (newAccessToken, newAccessTokenId) = jwtService.GenerateJwtToken(userToken.UserId);
                var newRefreshToken = jwtService.GenerateRefreshToken(userToken.UserId);
                newRefreshToken.Token = newAccessTokenId; // 새로운 JTI를 저장

                userToken.Revoked = DateTime.UtcNow;
                userToken.ReplacedByToken = newRefreshToken.Token; // 추적을 위해 새로운 JTI 기록
                dbContext.UserTokens.Add(newRefreshToken);

                await dbContext.SaveChangesAsync();

                var expiresIn = configuration.GetValue<int>("Jwt:ExpiresInSeconds");

                return Results.Ok(new { 
                    accessToken = newAccessToken,
                    refreshToken = newRefreshToken.Token, // 새로운 JTI를 Refresh Token으로 반환
                    expiresIn // 만료 시간(초) 추가
                });
            })
            .WithName("RefreshToken")
            .WithSummary("토큰 갱신 (토큰 회전)")
            .WithDescription("유효한 Refresh Token을 사용하여 새로운 Access Token과 Refresh Token을 발급받습니다. 사용된 Refresh Token은 무효화됩니다.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/admin/revoke/{userId}", async (
                [FromRoute] string userId,
                ApplicationDbContext dbContext) =>
            {
                // 해당 사용자의 모든 활성 Refresh Token을 조회합니다.
                var activeTokens = await dbContext.UserTokens
                    .Where(rt => rt.UserId == userId && rt.Revoked == null && rt.Expires > DateTime.UtcNow)
                    .ToListAsync();

                if (!activeTokens.Any())
                {
                    return Results.NotFound(new { message = $"사용자 '{userId}'에 대한 활성 세션을 찾을 수 없습니다." });
                }

                // 조회된 모든 토큰을 무효화(Revoke) 처리합니다.
                foreach (var token in activeTokens)
                {
                    token.Revoked = DateTime.UtcNow;
                }

                await dbContext.SaveChangesAsync();

                return Results.Ok(new { message = $"사용자 '{userId}'의 모든 활성 세션({activeTokens.Count}개)이 성공적으로 만료되었습니다." });
            })
            .WithName("RevokeUserSessions")
            .WithSummary("특정 사용자의 모든 세션 강제 만료 (관리자용)")
            .WithDescription("관리자가 특정 사용자의 모든 활성 Refresh Token을 무효화하여 모든 기기에서의 세션을 강제로 만료시킵니다.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .AddEndpointFilter<AuthFilter>() // 인증 필터 적용
            .AddEndpointFilter<AdminAuthFilter>(); // 관리자 권한 확인 필터 적용

    }
}