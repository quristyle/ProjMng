using ProjMngServer.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Services.AddControllers();

var app = builder.Build();



// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment()){
//    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}


#region Cors 정보 등록
// Cors 정보 등록
app.UseCors(cors => cors
               .AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod()
               //.WithMethods("POST")
               //.AllowCredentials()
               //.WithHeaders("Content-Type", "Content-Length", "Accept-Encoding", "Connection", "Accept", "User-Agent", "Host", "Authorization")
           );
#endregion Cors 정보 등록



app.UseHttpsRedirection();


app.UseAntiforgery();


app.Run();
