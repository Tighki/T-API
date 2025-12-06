using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using UserService.Data;
using UserService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// PostgreSQL
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "User Service API",
        Version = "v1",
        Description = "Сервис управления настройками пользователей"
    });

    options.AddSecurityDefinition("X-User-Id", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = "X-User-Id",
        Description = "Идентификатор пользователя"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference 
                { 
                    Type = ReferenceType.SecurityScheme, 
                    Id = "X-User-Id" 
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// Auto-migrate
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();
