using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using skgRestApi.Data;
using skgRestApi.Endpoints;
using skgRestApi.Options;
using skgRestApi.Services;
using skgRestApi.Middlewares;
using Microsoft.OpenApi.Models;
using System.Text;



var builder = WebApplication.CreateBuilder(args);


//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
//                       ?? Environment.GetEnvironmentVariable("GHUB");

//if (connectionString == null)
//{
var   connectionString = @"Host=jin114.co.kr;Port=31015;Database=ghub;Username=ghub;Password=ghub;Search Path=ghub";
//}

builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseNpgsql(connectionString, o => o.MigrationsHistoryTable("__EFMigrationsHistory", "ghub")));

builder.Services.AddMemoryCache();
builder.Services.AddScoped<SysService>();



// IHttpContextAccessor를 등록하여 서비스 내에서 HttpContext에 접근할 수 있도록 합니다.
builder.Services.AddHttpContextAccessor();

// JSON 직렬화 시 순환 참조 문제를 해결하기 위한 설정
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
  options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();


builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "GHub RESTful API", Version = "v1" });

  // JWT Bearer 토큰 인증을 위한 보안 정의 추가
  c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
    Description = @"JWT Authorization header using the Bearer scheme. 
                      'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
    Name = "Authorization",
    In = ParameterLocation.Header,
    Type = SecuritySchemeType.Http,
    Scheme = "Bearer",
    BearerFormat = "JWT"
  });

  // 모든 엔드포인트에 인증을 요구하도록 보안 요구사항 추가
  c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
          new string[] {} }
    });
});

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();





//builder.Services.AddSwaggerGen(c => {
//  c.SwaggerDoc("v1", new OpenApiInfo {
//    Title = "GHub Rest",
//    Version = "v1",
//    Description = "GHub 프로젝트의 RESTful API",
//    TermsOfService = new Uri("https://example.com/terms"),
//    Contact = new OpenApiContact {
//      Name = "지원팀",
//      Email = string.Empty,
//      Url = new Uri("https://example.com/contact"),
//    },
//    License = new OpenApiLicense {
//      Name = "라이선스",
//      Url = new Uri("https://example.com/license"),
//    }
//  });

//  // XML 주석 파일을 사용하도록 SwaggerGen을 구성합니다.
//  // 프로젝트 속성에서 "XML 문서 파일 생성"을 활성화해야 합니다.
//  var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
//  var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
//  if (File.Exists(xmlPath)) {
//    c.IncludeXmlComments(xmlPath);
//  }
//});




//Authorization 미들웨어
builder.Services.AddAuthorization();

// JWT 
var jwtKey = builder.Configuration["Jwt:Key"] ?? "quristyle_skgas_secret_key_1234567890!@#$";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "skgRestApi";

builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuer = true,
    ValidateAudience = false,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = jwtIssuer,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
  };
});

// VAPID 옵션 + 푸시 관련 DI
builder.Services.Configure<VapidOptions>(builder.Configuration.GetSection("Vapid"));
builder.Services.AddScoped<IPushSubscriptionStore, DbPushSubscriptionStore>();
builder.Services.AddSingleton<IWebPushService>(sp => new WebPushService(
    sp.GetRequiredService<IOptions<VapidOptions>>(),
    sp.GetRequiredService<IServiceScopeFactory>(),
    sp.GetRequiredService<ILogger<WebPushService>>()
));

var app = builder.Build();

// 애플리케이션 시작 시 DB 마이그레이션 및 시드 수행(프로덕션에서는 주의)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {

        // 마이그레이션 추가 명령어 예시:
      // dotnet ef migrations add AddLoginHistories

        // 마이그레이션을 실행하여 누락된 테이블(LoginHistories 등)을 생성합니다.
        await db.Database.MigrateAsync();

        await DbInitializer.InitializeAsync(db, seedBy: "init-seed");

        // 모델 XML 주석을 DB 테이블/칼럼 코멘트로 반영
        await DbCommentsApplier.ApplyCommentsAsync(db);
    }
    catch (Exception ex)
    {
        // 마이그레이션/시드 중 발생한 예외를 로깅하고 애플리케이션이 바로 종료되지 않도록 합니다.
        logger.LogError(ex, "Database migration or seeding failed. Check connection string, database availability, and migrations.");
        // 필요하면 예외를 다시 던져 애플리케이션을 중단하도록 할 수 있습니다.
        // throw;
    }
}

if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI();
  //app.UseSwaggerUI(c => {
  //  c.SwaggerEndpoint("/swagger/v1/swagger.json", "skgRestApi v1");
  //  c.RoutePrefix = "swagger"; // Swagger UI가 "/swagger" 경로에서 열리도록 설정합니다.
  //});
}


app.UseHttpsRedirection();



app.UseMiddleware<JwtMiddleware>(); // JWT 미들웨어 추가

app.UseAuthentication();

app.UseAuthorization();


// Endpoint 

// 권한처리용.
app.MapAuthEndpoints();
// 테스트
app.MapDataEndpoints();

app.Run();
