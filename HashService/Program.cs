using Microsoft.OpenApi.Models;
using System.Threading.RateLimiting;
using HashService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IHashService, HashService.Services.HashService>();
builder.Services.AddScoped<IGuidService, GuidService>();

builder.Services.AddRateLimiter(o =>
{
    o.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
        RateLimitPartition.GetFixedWindowLimiter(
            ctx.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            _ => new FixedWindowRateLimiterOptions { PermitLimit = 100, Window = TimeSpan.FromMinutes(1) }));
    o.RejectionStatusCode = 429;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o => o.SwaggerDoc("v1", new OpenApiInfo { Title = "Hash Service", Version = "v1" }));

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
