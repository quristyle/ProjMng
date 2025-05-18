using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ProjMngWasm;
using ProjMngWasm.Commons;
using Radzen;
using WasmShear;
using WasmShear.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore();

//builder.Services.AddSingleton<ISessionStorageService, SessionStorageService>();
//builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddRadzenComponents();
builder.Services.AddRadzenQueryStringThemeService();

builder.Services.AddHttpClient<UMSService>(client => {        client.BaseAddress = new Uri("https://nums.api.hanjucorp.co.kr");            });

builder.Services.AddHttpClient("jsini", client => { client.BaseAddress = new Uri("https://api.jsini.co.kr"); });

builder.Services.AddHttpClient<FuneralService>(client => { client.BaseAddress = new Uri("https://funeralfr.jsini.co.kr");   });

builder.Services.AddScoped<JsiniService>();
builder.Services.AddSingleton<DevService>();
builder.Services.AddSingleton<SysService>();
builder.Services.AddSingleton<AppData>();
builder.Services.AddSingleton<AppProjData>();

var app = builder.Build();

await  app.RunAsync();
