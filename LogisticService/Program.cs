using LogisticService.Models;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add service entity framework
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<LogisticDbServiceContext>(options =>
    options
        .UseLazyLoadingProxies(false)
        .UseSqlServer(connectionString));
//Add middleware controller
builder.Services.AddControllers();

//bật cors 
builder.Services.AddCors(options =>
{
    options.AddPolicy("allow_all", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// cache
builder.Services.AddMemoryCache();
//cache redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379"; // hoặc connection string từ Cloud
    options.InstanceName = "Logistic:";
});
//Làm việc với nhiều db redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => {
  return ConnectionMultiplexer.Connect("localhost:6379");
});
builder.Services.AddSingleton<RedisHelper>();

//Repository Pattern & Unitofwork
//Unitofwrork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//Repository
builder.Services.AddScoped<IHangHoaRepository, HangHoaRepository>();
//Service
builder.Services.AddScoped<IHangHoaService, HangHoaService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("allow_all");
app.MapControllers();
app.UseHttpsRedirection();


app.Run();