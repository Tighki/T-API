using Microsoft.OpenApi.Models;
using PortfolioService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<IDataStore, InMemoryDataStore>();
builder.Services.AddScoped<ICoolingCalculator, CoolingCalculator>();
builder.Services.AddSingleton<TokenGenerator>();

// HTTP клиенты для межсервисного взаимодействия
builder.Services.AddHttpClient<IUserServiceClient, UserServiceClient>();
builder.Services.AddHttpClient<ICoolingServiceClient, CoolingServiceClient>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Portfolio Service API",
        Version = "v1",
        Description = "Сервис управления портфолио желаемых покупок"
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

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();

