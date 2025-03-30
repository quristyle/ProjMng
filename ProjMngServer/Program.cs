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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Cors 정보 등록
app.UseCors(cors => cors
               .AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod()
           //.WithMethods("POST")
           //.AllowCredentials()
           //.WithHeaders("Content-Type", "Content-Length", "Accept-Encoding", "Connection", "Accept", "User-Agent", "Host", "Authorization")
           );
//Cors 정보 등록

app.Run();