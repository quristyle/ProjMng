using Microsoft.EntityFrameworkCore;
using skgRestApi.Models;
using System.Linq;

namespace skgRestApi.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext context, string seedBy = "system")
    {
        // 최신 마이그레이션 적용
        try
        {
            // 마이그레이션이 있는 경우에만 적용
            var pending = context.Database.GetPendingMigrations();
            if (pending != null && pending.Any())
            {
                await context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            // 명확한 예외 메시지를 포함하여 문제가 되는 원인을 쉽게 파악하도록 합니다.
            throw new InvalidOperationException("Database migration failed. Check connection string, database availability and migrations.", ex);
        }

        // 샘플 데이터가 존재하면 건너뜀
        if (await context.BirthdayEntries.AnyAsync()) return;

        var now = DateTime.UtcNow;

        // Birthday 샘플
        var birthdays = new[]
        {
            new BirthdayEntry { SubjectId = "emp001", Name = "홍길동", BirthDate = DateOnly.FromDateTime(new DateTime(1990,1,15)), IsLunar = false, IsCelebrated = true, CreatedBy = seedBy, CreatedAt = now },
            new BirthdayEntry { SubjectId = "emp002", Name = "김영희", BirthDate = DateOnly.FromDateTime(new DateTime(1988,5,20)), IsLunar = false, IsCelebrated = false, CreatedBy = seedBy, CreatedAt = now }
        };

        // MealMenu 및 Items 샘플
        var lunchMenu = new MealMenu
        {
            Date = DateOnly.FromDateTime(now),
            Type = MealType.Lunch,
            CreatedBy = seedBy,
            CreatedAt = now
        };

        var menuItems = new[]
        {
            new MealMenuItem { Name = "비빔밥", Description = "야채, 고기 포함", Calories = 750, AllergyInfo = "계란,우유", CreatedBy = seedBy, CreatedAt = now },
            new MealMenuItem { Name = "된장국", Description = "두부 포함", Calories = 120, CreatedBy = seedBy, CreatedAt = now }
        };

        lunchMenu.Items.AddRange(menuItems);

        var mealFeedbacks = new[]
        {
            new MealFeedback { MealMenuId = lunchMenu.Id, AuthorId = "emp001", Content = "오늘 비빔밥 맛있어요", Rating = 5, Likes = 3, CreatedBy = seedBy, CreatedAt = now }
        };

        // Weather 샘플
        var weathers = new[]
        {
            new WeatherInfo { Location = "본사", ObservationTime = now, TemperatureC = 18.5, Condition = "맑음", Humidity = 45, WindSpeed = 1.2, CreatedBy = seedBy, CreatedAt = now }
        };

        // ConnectHub 샘플
        var hubs = new[]
        {
            new ConnectHubItem { Title = "사내포털", Url = "/portal", Category = "포털", IsExternal = false, Order = 1, CreatedBy = seedBy, CreatedAt = now },
            new ConnectHubItem { Title = "문서센터", Url = "/docs", Category = "문서", IsExternal = false, Order = 2, CreatedBy = seedBy, CreatedAt = now }
        };

        // SFM 샘플
        var sfm = new[]
        {
            new SfmReport { ReporterId = "emp003", ReportedAt = now, MeasurementScore = 87.5m, Feedback = "안전 장비 착용 필요", Category = "안전", IsAcknowledged = false, CreatedBy = seedBy, CreatedAt = now }
        };

        // SafetyStandard 샘플
        var standards = new[]
        {
            new SafetyStandard { Code = "STD-001", Title = "기본 안전 수칙", Summary = "개인 보호구 착용", Details = "항상 안전모 및 안전화를 착용.", Version = "1.0", EffectiveDate = DateOnly.FromDateTime(now.Date), CreatedBy = seedBy, CreatedAt = now }
        };

        // SafetySticker 샘플
        var stickers = new[]
        {
            new SafetySticker { StickerCode = "STK-0001", RecipientId = "emp001", IssuedAt = now, ExpiresAt = now.AddYears(1), Reason = "교육 이수", IssuedBy = seedBy, CreatedBy = seedBy, CreatedAt = now }
        };

        // SafetyViolation 샘플
        var violations = new[]
        {
            new SafetyViolation { ViolatorId = "emp004", ObservedAt = now, Location = "작업장 A", ViolationType = "미착용", Severity = 3, Description = "안전모 미착용", IsResolved = false, CreatedBy = seedBy, CreatedAt = now }
        };

        // AppSetting 샘플
        var settings = new[]
        {
            new AppSetting { Key = "SiteTitle", Value = "GHub SKG", Section = "UI", DataType = "String", IsSensitive = false, CreatedBy = seedBy, CreatedAt = now }
        };

        // UserProfile 샘플
        var profiles = new[]
        {
            new UserProfile { UserId = "emp001", FullName = "홍길동", Email = "hong@example.com", Phone = "010-0000-0001", Department = "생산", Position = "팀원", PhotoUrl = null, PreferencesJson = null, CreatedBy = seedBy, CreatedAt = now }
        };

        // ReferenceStandard 샘플
        var refs = new[]
        {
            new ReferenceStandard { Key = "REF-TEMP-MAX", Name = "최대 허용 온도", NumericValue = 50m, Unit = "℃", Description = "장비 허용 최대 온도", CreatedBy = seedBy, CreatedAt = now }
        };

        // DB 추가
        await context.AddRangeAsync(birthdays);
        await context.AddAsync(lunchMenu); // lunchMenu에 포함된 Items는 자동으로 추가됨
        await context.AddRangeAsync(mealFeedbacks);
        await context.AddRangeAsync(weathers);
        await context.AddRangeAsync(hubs);
        await context.AddRangeAsync(sfm);
        await context.AddRangeAsync(standards);
        await context.AddRangeAsync(stickers);
        await context.AddRangeAsync(violations);
        await context.AddRangeAsync(settings);
        await context.AddRangeAsync(profiles);
        await context.AddRangeAsync(refs);

        await context.SaveChangesAsync();
    }
}