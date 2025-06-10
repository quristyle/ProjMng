using ProjMngServer;
using ProjMngServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddScoped<DevService>();
builder.Services.AddScoped<ProjService>();
builder.Services.AddScoped<SysService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register services
builder.Services.AddSingleton<AppData>();




// 1. CORS 정책을 먼저 서비스에 등록
builder.Services.AddCors(options =>{
  options.AddPolicy("AllowAll", policy =>  {
    policy.AllowAnyOrigin()
          .AllowAnyHeader()
          .AllowAnyMethod();
  });
});





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI();
}



// 2. CORS 미들웨어는 반드시 UseRouting() 앞 또는 직후에 위치해야 함
//app.UseRouting();

// 3. 등록한 정책 이름으로 CORS 활성화
app.UseCors("AllowAll");






app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();



// Cors 정보 등록
//app.UseCors(cors => cors
//               .AllowAnyOrigin()
//               .AllowAnyHeader()
//               .AllowAnyMethod()
//           //.WithMethods("POST")
//           //.AllowCredentials()
//           //.WithHeaders("Content-Type", "Content-Length", "Accept-Encoding", "Connection", "Accept", "User-Agent", "Host", "Authorization")
//           );
//Cors 정보 등록

app.Run();