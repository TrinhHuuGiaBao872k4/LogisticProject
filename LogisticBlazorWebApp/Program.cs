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
builder.Services.AddScoped<ProductStateService>();

//setup middleware 
//middleware cross
builder.Services.AddCors(option =>
{
    option.AddPolicy("allow_origin", policy =>
    {
        //policy.AllowAnyOrigin : cho phép tất cả các client đều có thể gửi dữ liệu đến server
        policy.WithOrigins("http://localhost:5103")
        .AllowAnyHeader()//cho phép rq tất cả header
        .AllowAnyMethod()//cho phep rq tất cả method(get,post,put,delete)
        .AllowCredentials();/// cho phép tất cả cookie
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

app.Run();
