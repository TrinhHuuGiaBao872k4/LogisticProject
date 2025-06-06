using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using LogisticBlazorWebApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

//Add service httpclient để gọi api
builder.Services.AddHttpClient();

//deploy cài đặt lắng nghe port 80

if (builder.Environment.IsProduction()) {
    builder.WebHost.UseUrls("http://*:80");    
}


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

if (app.Environment.IsProduction())
{
    app.Urls.Add("http://*:80");
}

app.Run();
