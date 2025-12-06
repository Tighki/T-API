using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Threading.RateLimiting;
using CoolingService.Data;
using CoolingService.Services;
using T_API.Shared.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(o => o.Filters.Add<SanitizeInputFilter>());
builder.Services.AddDbContext<CoolingDbContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ICoolingRepository, CoolingRepository>();

builder.Services.AddRateLimiter(o =>
{
    o.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
        RateLimitPartition.GetFixedWindowLimiter(
            ctx.Request.Headers["X-User-Id"].ToString() ?? ctx.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            _ => new FixedWindowRateLimiterOptions { PermitLimit = 100, Window = TimeSpan.FromMinutes(1) }));
    o.RejectionStatusCode = 429;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "Cooling Service", Version = "v1" });
    o.AddSecurityDefinition("X-User-Id", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey, In = ParameterLocation.Header, Name = "X-User-Id"
    });
    o.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "X-User-Id" } }, [] }
    });
});

builder.Services.AddCors(o => o.AddPolicy("Default", p =>
{
    if (builder.Environment.IsDevelopment())
        p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    else
        p.WithOrigins(builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? [])
         .AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("Default");
app.UseRateLimiter();
app.MapControllers();
app.Run();
