using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using skgRestApi.Models;
using System.Security.Cryptography;
using skgRestApi.Data;
using Microsoft.EntityFrameworkCore;

namespace skgRestApi.Services;

/// <summary>
/// JWT 체크 타입
/// </summary>
public enum JwtCheckType
{
    /// <summary>
    /// 토큰 검증 성공
    /// </summary>
    Success,
    /// <summary>
    /// Request의 Token 값이 없습니다.
    /// </summary>
    NotJwt,
    /// <summary>
    /// JWT SecretKey키 값이 없습니다.
    /// </summary>
    JwtKeyEmpty,
    /// <summary>
    /// 토큰 검증 변수가 null입니다.
    /// </summary>
    ValidNull,
    /// <summary>
    /// 토큰이 만료되었습니다.
    /// </summary>
    ValidExpires,
    /// <summary>
    /// 토큰이 아직 유효하지 않습니다. (nbf)
    /// </summary>
    NotYetValid,
    /// <summary>
    /// 위조된 토큰입니다.
    /// </summary>
    Forgery,
}

/// <summary>
/// JWToen 생성 인증 서비스
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// JWToken Access 토큰 발급
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    (string accessToken, string accessTokenId) GenerateJwtToken(string userId);

    /// <summary>
    /// Refresh Token 생성
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    UserToken GenerateRefreshToken(string userId);

    /// <summary>
    /// JWToken 검증
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<(JwtCheckType jwtCheckType, ClaimsPrincipal? claimsPrincipal)> ValidateJwtTokenAsync(string token);
}

/// <summary>
/// JWToen 생성 인증 서비스
/// </summary>
public class JwtService : IJwtService
{
    /// <summary>
    /// appsettings.json 데이터 서비스
    /// </summary>
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _dbContext;

    public JwtService(IConfiguration configuration, ApplicationDbContext dbContext)
    {
        _configuration = configuration;
        _dbContext = dbContext;
    }

    /// <summary>
    /// JWToken Access 토큰 생성
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public (string accessToken, string accessTokenId) GenerateJwtToken(string userId)
    {
        // JWToken 시크릿 키 Config에서 가져오기
        string? jwtKey = _configuration["Jwt:SecretKey"];
        // JWToken 인증시간 Config에서 가져오기
        int expiresTime = _configuration.GetValue<int>("Jwt:ExpiresInSeconds");
        // 시크릿키 없을때 예외처리
        if (string.IsNullOrEmpty(jwtKey))
            throw new Exception("JWT Key를 찾을 수 없습니다.");
        // 시크릿키 Byte로 변환
        var key = Encoding.ASCII.GetBytes(jwtKey);
        // AccessTokenID 생성 (저장된 Token정보 데이터 베이스 검증용)
        var tokenId = Guid.NewGuid().ToString();

        var claims = new List<Claim>
        {
            new Claim("id", userId),
            new Claim(JwtRegisteredClaimNames.Jti, tokenId)
        };

        // 'admin' 사용자에게 'admin' 역할(role) 부여
        if (userId == "admin")
        {
            claims.Add(new Claim(ClaimTypes.Role, "admin"));
        }

        var claimsIdentity = new ClaimsIdentity(claims);
        // sha256 으로 인증키 셋팅
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = claimsIdentity, // 크레임 정보 셋팅
            Expires = DateTime.UtcNow.AddSeconds(expiresTime), // 인증 만료 시간
            SigningCredentials = signingCredentials // 인증키 셋팅
        };

        // jwtoken 토큰 생성
        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(descriptor);
 
        return (handler.WriteToken(token), tokenId);
    }

    /// <summary>
    /// Refresh Token 생성
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public UserToken GenerateRefreshToken(string userId)
    {
        // 암호학적으로 안전한 난수 생성기를 사용하여 랜덤 바이트 생성
        using var rng = RandomNumberGenerator.Create();
        var randomBytes = new byte[64];
        rng.GetBytes(randomBytes);

        return new UserToken
        {
            UserId = userId,
            // Jti를 Refresh Token의 Token 필드에 저장
            Token = Guid.NewGuid().ToString(), // Refresh Token 자체의 고유 값
            Expires = DateTime.UtcNow.AddDays(7), // Refresh Token 유효기간: 7일
            Created = DateTime.UtcNow
        };
    }

    /// <summary>
    /// JWToken 검증
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<(JwtCheckType jwtCheckType, ClaimsPrincipal? claimsPrincipal)> ValidateJwtTokenAsync(string token)
    {
        // 토큰 정보가 NULL일때
        if (string.IsNullOrEmpty(token))
            return (JwtCheckType.NotJwt, null);

        try
        {
            // JWToken 시크릿 키 Config에서 가져오기
            string? jwtKey = _configuration["Jwt:SecretKey"];

            // 시크릿 키가 NULL일때
            if (string.IsNullOrEmpty(jwtKey))
                return (JwtCheckType.JwtKeyEmpty, null);

            // 시크릿키 Byte변환
            var key = Encoding.ASCII.GetBytes(jwtKey);

            var handler = new JwtSecurityTokenHandler();
            // 토큰 키 일치 검증
            var validateForgeryToken = await handler.ValidateTokenAsync(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true, // 키 일치 검증
                IssuerSigningKey = new SymmetricSecurityKey(key), // 시크릿 키 
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false, // 만료시간은 별도 체크
                ClockSkew = TimeSpan.Zero,
            });

            /// 토큰 키 불일치
            if (!validateForgeryToken.IsValid)
            {
                return (JwtCheckType.Forgery, null);
            }

            // 토큰 만료 시간 검증
            var validateExpiredTimeToken = await handler.ValidateTokenAsync(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true, // 만료시간 검증
                ClockSkew = TimeSpan.Zero,
            });

            // 토큰에서 기본 정보 가져와 셋팅
            var claims = (validateForgeryToken.SecurityToken as JwtSecurityToken)?.Claims;
            var userId = claims?.FirstOrDefault(c => c.Type == "id")?.Value ?? string.Empty;
            var jti = claims?.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

            // JTI를 사용하여 DB에서 Refresh Token의 유효성 검사
            if (!string.IsNullOrEmpty(jti))
            {
                var userToken = await _dbContext.UserTokens.AsNoTracking().SingleOrDefaultAsync(rt => rt.Token == jti);
                if (userToken == null || !userToken.IsActive) {
                    return (JwtCheckType.Forgery, null); // 무효화된 토큰으로 간주
                }
            }

            // 토큰 만료시간 지났는지 체크
            if (!validateExpiredTimeToken.IsValid)
            {
                return (JwtCheckType.ValidExpires, new ClaimsPrincipal(validateForgeryToken.ClaimsIdentity));
            }

            // 토큰이 아직 유효하지 않은 경우 (nbf)
            if (validateExpiredTimeToken.Exception is SecurityTokenNotYetValidException)
            {
                return (JwtCheckType.NotYetValid, new ClaimsPrincipal(validateForgeryToken.ClaimsIdentity));
            }


            // 토큰 인증 성공
            return (JwtCheckType.Success, new ClaimsPrincipal(validateExpiredTimeToken.ClaimsIdentity));
        }
        catch (Exception ex)
        {
            // It's a good practice to log the exception 'ex' here for debugging purposes.
            return (JwtCheckType.Forgery, null);
        }
    }
}