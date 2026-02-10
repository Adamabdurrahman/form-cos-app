using Microsoft.EntityFrameworkCore;
using backend.Data;

var builder = WebApplication.CreateBuilder(args);

// ── Database ──
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost;Database=form_cos;User=root;Password=root;";

var serverVersion = ServerVersion.AutoDetect(connectionString);

builder.Services.AddDbContext<FormCosDbContext>(options =>
    options.UseMySql(connectionString, serverVersion, mysqlOptions =>
    {
        mysqlOptions.EnableRetryOnFailure(3);
    })
);

// ── Controllers ──
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        opts.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// ── CORS (hanya untuk development, saat frontend jalan terpisah) ──
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:5174",
                "http://localhost:5175",
                "http://localhost:3000"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// ── Auto-migrate on startup (dev only) ──
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FormCosDbContext>();
    db.Database.Migrate();
}

// ── Middleware pipeline ──
app.UseCors("AllowFrontend");

// Serve static files dari wwwroot (hasil build frontend)
app.UseDefaultFiles();     // index.html sebagai default
app.UseStaticFiles();      // serve file dari wwwroot/

// API routes
app.MapControllers();

// SPA fallback: semua route yang bukan /api/* dan bukan static file
// diarahkan ke index.html supaya React Router bisa handle routing
app.MapFallbackToFile("index.html");

app.Run();
