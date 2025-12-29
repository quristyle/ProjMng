using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using skgRestApi.Data;
using skgRestApi.Models;

namespace skgRestApi.Services;

/// <summary>
/// 사용자 인증을 처리하는 서비스 인터페이스
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 사용자 자격 증명을 검증합니다.
    /// </summary>
    /// <param name="userId">사용자 ID</param>
    /// <param name="password">비밀번호</param>
    /// <returns>인증 성공 여부</returns>
    Task<bool> ValidateUserAsync(string userId, string password);
}

/// <summary>
/// 사용자 인증을 처리하는 서비스
/// </summary>
public class AuthService : IAuthService
{
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly int _maxFailedAttempts;
    private readonly TimeSpan _lockoutDuration;

    // 실제 애플리케이션에서는 데이터베이스에서 사용자 정보를 조회하고 암호를 비교해야 합니다.
    // 여기서는 데모 목적으로 사용자를 하드코딩합니다.
    private readonly Dictionary<string, string> _users = new()
    {
        { "test", "test" }, // 일반 사용자
        { "test1", "test1" },{ "test2", "test2" },{ "test3", "test3" },{ "test4", "test4" },
        { "admin", "admin" }  // 관리자
    };

    public AuthService(IMemoryCache cache, IConfiguration configuration, ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _cache = cache;
        _configuration = configuration;
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _maxFailedAttempts = _configuration.GetValue<int>("LoginPolicy:MaxFailedAttempts", 5);
        _lockoutDuration = TimeSpan.FromMinutes(_configuration.GetValue<int>("LoginPolicy:LockoutMinutes", 15));
    }

    public async Task<bool> ValidateUserAsync(string userId, string password)
    {
        var lockoutKey = $"lockout_{userId}";
        var failedAttemptsKey = $"failed_{userId}";
        var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

        var loginHistory = new LoginHistory
        {
            UserId = userId,
            IpAddress = ipAddress,
            LoginTime = DateTime.UtcNow
        };

        // 1. 계정이 잠겨 있는지 확인
        if (_cache.TryGetValue(lockoutKey, out _))
        {
            var reason = $"계정이 잠겼습니다. {_lockoutDuration.TotalMinutes}분 후에 다시 시도해주세요.";
            loginHistory.Success = false;
            loginHistory.FailureReason = "Account Locked";
            _dbContext.LoginHistories.Add(loginHistory);
            await _dbContext.SaveChangesAsync();
            throw new Exception(reason);
        }

        if (_users.TryGetValue(userId, out var storedPassword))
        {
            if (password == storedPassword)
            {
                // 2. 로그인 성공: 실패 횟수 초기화
                _cache.Remove(failedAttemptsKey);

                loginHistory.Success = true;
                _dbContext.LoginHistories.Add(loginHistory);
                await _dbContext.SaveChangesAsync();
                return true;
            }
        }

        // 3. 로그인 실패: 실패 횟수 증가
        var failedAttempts = _cache.GetOrCreate(failedAttemptsKey, entry => 0);
        failedAttempts++;

        if (failedAttempts >= _maxFailedAttempts)
        {
            // 4. 실패 횟수 초과: 계정 잠금
            _cache.Set(lockoutKey, true, _lockoutDuration);
            _cache.Remove(failedAttemptsKey); // 잠금이 설정되면 실패 횟수는 초기화
            var reason = $"로그인 {_maxFailedAttempts}회 실패하여 계정이 잠겼습니다. {_lockoutDuration.TotalMinutes}분 후에 다시 시도해주세요.";
            loginHistory.Success = false;
            loginHistory.FailureReason = "Max Failed Attempts";
            _dbContext.LoginHistories.Add(loginHistory);
            await _dbContext.SaveChangesAsync();
            throw new Exception(reason);
        }
        else
        {
            _cache.Set(failedAttemptsKey, failedAttempts, TimeSpan.FromMinutes(30)); // 실패 기록은 30분간 유효
        }

        loginHistory.Success = false;
        loginHistory.FailureReason = "Invalid Credentials";
        _dbContext.LoginHistories.Add(loginHistory);
        await _dbContext.SaveChangesAsync();
        return false; // 로그인 실패
    }
}