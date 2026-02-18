using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Database: MasterDbContext → db_master_data (Group A) ──
builder.Services.AddDbContext<MasterDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            // Level 120 = SQL Server 2014
            // Mencegah EF Core pakai OPENJSON yang tidak tersedia di SQL Express lama
            sqlOptions.UseCompatibilityLevel(120);
        }
    ));

// ── Database: CosDbContext → db_cos_checksheet (Group B + C) ──
builder.Services.AddDbContext<CosDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("CosChecksheetConnection"),
        sqlOptions =>
        {
            sqlOptions.UseCompatibilityLevel(120);
        }
    ));

// ── Database: PadCostDbContext → db_padCost (ReadOnly — production data) ──
builder.Services.AddDbContext<PadCostDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("PadCostConnection"),
        sqlOptions =>
        {
            sqlOptions.UseCompatibilityLevel(120);
        }
    ));

// ── Application Services ──
builder.Services.AddScoped<ProductionDataService>();

// ── Background Services ──
builder.Services.AddHostedService<BatteryTypeSyncService>();

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
// NOTE: MasterDbContext TIDAK di-migrate karena db_master_data dikelola
//       secara terpisah (already existing database).
// CosDbContext juga TIDAK di-auto-migrate — gunakan MigrationScript.sql manual.
// Uncomment baris di bawah HANYA setelah MigrationScript.sql sudah dijalankan:
//
// using (var scope = app.Services.CreateScope())
// {
//     var cosDb = scope.ServiceProvider.GetRequiredService<CosDbContext>();
//     cosDb.Database.Migrate();
// }

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
