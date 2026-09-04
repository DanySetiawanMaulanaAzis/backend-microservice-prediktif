using smart_table.Interfaces;
using smart_table.Repositories;
using smart_table.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IMachineRepository, MachineRepository>();

// The machine-learning Flask service. Base address comes from ML_API_URL
// (docker-compose sets http://machine-learning:5000); localhost is the
// fallback for `dotnet run`.
var mlApiUrl = builder.Configuration["ML_API_URL"] ?? "http://localhost:5000";
builder.Services.AddHttpClient<IMachineService, MachineService>(client =>
{
    client.BaseAddress = new Uri(mlApiUrl.TrimEnd('/') + "/");
});

// Configure CORS. Origins come from configuration (Cors:Origins, or the
// Cors__Origins__0 / __1 env vars that docker-compose sets); the localhost pair
// is the fallback for `dotnet run`.
var corsOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>();
if (corsOrigins is null || corsOrigins.Length == 0)
{
    corsOrigins = new[] { "http://localhost:4300", "http://localhost:4200" };
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Skip HTTPS redirection when the app is served over plain HTTP only
// (containers set ASPNETCORE_HTTPS_REDIRECT=false).
if (!string.Equals(
        builder.Configuration["ASPNETCORE_HTTPS_REDIRECT"], "false",
        StringComparison.OrdinalIgnoreCase))
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapControllers();

app.Run();
