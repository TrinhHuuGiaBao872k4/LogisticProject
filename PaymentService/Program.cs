var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

if (builder.Environment.IsProduction())
{
    builder.WebHost.UseUrls("http://*:81");
}

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


if (app.Environment.IsProduction())
{
    app.Urls.Add("http://*:81");
}

app.Run();


