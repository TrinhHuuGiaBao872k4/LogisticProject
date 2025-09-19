using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;


var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();


// 🔹 Lấy base URL từ ENV hoặc appsettings
var logisticApiBaseUrl = builder.Configuration["LOGISTIC_API_BASE_URL"]
                         ?? builder.Configuration["ApiSettings:LogisticApiBaseUrl"]
                         ?? "http://localhost:5103/";

var paymentApiBaseUrl = builder.Configuration["PAYMENT_API_BASE_URL"]
                         ?? builder.Configuration["ApiSettings:PaymentApiBaseUrl"]
                         ?? "http://localhost:5203/";

// Đăng ký HttpClient chung cho Logistic API
builder.Services.AddHttpClient("LogisticApi", client =>
{
    client.BaseAddress = new Uri(logisticApiBaseUrl);
});

//deploy cài đặt lắng nghe port 80

if (builder.Environment.IsProduction())
{
    builder.WebHost.UseUrls("http://*:80");
}
builder.Services.AddScoped<HangHoaService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddSingleton<UserStateService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<RegisterService>();
builder.Services.AddScoped<DonHangService>();
builder.Services.AddScoped<DonHangHubService>();

builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddAuthorizationCore();




//setup middleware 
//middleware cross
builder.Services.AddCors(option =>
{
    option.AddPolicy("allow_origin", policy =>
    {
        //policy.AllowAnyOrigin : cho phép tất cả các client đều có thể gửi dữ liệu đến server
        // policy.WithOrigins("http://localhost:5103")
        // .AllowAnyHeader()//cho phép rq tất cả header
        // .AllowAnyMethod()//cho phep rq tất cả method(get,post,put,delete)
        // .AllowCredentials();/// cho phép tất cả cookie
        policy.WithOrigins("https://localhost:7163","http://localhost:5103")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();

    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseCors("allow_origin");
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
