# 📋 Panduan Migrasi ke SQL Server (db_master_data)

## Situasi

| | Sekarang (Dev) | Nanti (Production) |
|---|---|---|
| **Database** | MySQL (`form_cos`) | SQL Server (`db_master_data`) |
| **Tabel Master Data** | Dibuat oleh EF Migration + seed dummy | **Sudah ada** di SQL Server, terisi data asli |
| **Tabel COS** | Dibuat oleh EF Migration + seed | **Belum ada**, perlu dibuat |
| **Provider** | Pomelo.EntityFrameworkCore.MySql | Microsoft.EntityFrameworkCore.SqlServer |

## Peta Tabel: Mana yang Sudah Ada, Mana yang Harus Dibuat

### ✅ SUDAH ADA di SQL Server (JANGAN di-migrate, JANGAN di-seed)

| # | Tabel | Isi |
|---|-------|-----|
| 1 | `tlkp_plant` | Data pabrik |
| 2 | `tlkp_divisi` | Data divisi |
| 3 | `tlkp_departemen` | Data departemen |
| 4 | `tlkp_section` | Data seksi |
| 5 | `tlkp_line` | Line produksi (Line 2-8, dll) |
| 6 | `tlkp_group` | Grup kerja (Group A, B, C) |
| 7 | `tlkp_shift` | Shift kerja (Shift 1, 2, 3) |
| 8 | `tlkp_lineGroup` | Mapping line+group → leader, kasubsie |
| 9 | `tlkp_operator` | Data operator |
| 10 | `tlkp_userKasie` | Mapping kasie ke section |
| 11 | `tlkp_user` | Data user/login |
| 12 | `tlkp_item` | Data item/produk |
| 13 | `tlkp_kategori` | Kategori item |
| 14 | `tlkp_series` | Data series |
| 15 | `tlkp_mold` | Data cetakan/mold |
| 16 | `tlkp_moldtype` | Tipe mold |
| 17 | `tlkp_job` | Data jabatan |
| 18 | `VIEW_DATAAUTH` | View dari Sunfish HR (nama karyawan) |
| 19 | `VIEW_EMPLOYEE` | View karyawan sederhana |

### 🆕 HARUS DIBUAT di SQL Server (perlu migration + seed)

| # | Tabel | Isi |
|---|-------|-----|
| 1 | `cos_form_definitions` | Definisi form COS |
| 2 | `cos_check_items` | 32 item pengecekan |
| 3 | `cos_check_sub_rows` | 16 sub-row pengecekan |
| 4 | `cos_battery_types` | 6 tipe baterai |
| 5 | `cos_battery_standards` | 96 standar parameter |
| 6 | `cos_problem_columns` | 6 kolom masalah |
| 7 | `cos_signature_slots` | 4 slot tanda tangan |
| 8 | `cos_submissions` | Data submission (runtime) |
| 9 | `cos_check_values` | Nilai pengecekan (runtime) |
| 10 | `cos_problems` | Data masalah (runtime) |
| 11 | `cos_signature_entries` | Tanda tangan (runtime) |

---

## Langkah-Langkah Migrasi

### STEP 1: Install NuGet Package SQL Server

```bash
cd /home/ubuntu/gs/form/backend

# Hapus Pomelo MySQL
dotnet remove package Pomelo.EntityFrameworkCore.MySql

# Tambah SQL Server provider
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.*
```

### STEP 2: Update Connection String

Edit `appsettings.json` (atau buat jika belum ada):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=NAMA_SERVER;Database=db_master_data;User Id=sa;Password=PASSWORD;TrustServerCertificate=True;"
  }
}
```

> **Catatan**: Sesuaikan `Server`, `User Id`, `Password` dengan SQL Server Anda.

### STEP 3: Update `Program.cs`

Ganti konfigurasi database dari MySQL ke SQL Server:

```csharp
// ── SEBELUM (MySQL) ──
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost;Database=form_cos;User=root;Password=root;";
var serverVersion = ServerVersion.AutoDetect(connectionString);
builder.Services.AddDbContext<FormCosDbContext>(options =>
    options.UseMySql(connectionString, serverVersion, mysqlOptions =>
    {
        mysqlOptions.EnableRetryOnFailure(3);
    })
);

// ── SESUDAH (SQL Server) ──
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost;Database=db_master_data;User Id=sa;Password=PASSWORD;TrustServerCertificate=True;";
builder.Services.AddDbContext<FormCosDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(3);
    })
);
```

### STEP 4: Update `FormCosDbContext.cs` — Exclude Master Data dari Migration

Ini langkah **paling penting**. Tambahkan `.ToTable(t => t.ExcludeFromMigrations())` pada semua tabel master data supaya EF TIDAK mencoba membuat/mengubah tabel yang sudah ada.

Tambahkan blok ini di `OnModelCreating`, **sebelum** `SeedData(modelBuilder)`:

```csharp
// ═══ MASTER DATA: Exclude from migrations (tabel sudah ada di SQL Server) ═══
modelBuilder.Entity<TlkpPlant>().ToTable("tlkp_plant", t => t.ExcludeFromMigrations());
modelBuilder.Entity<TlkpDivisi>().ToTable("tlkp_divisi", t => t.ExcludeFromMigrations());
modelBuilder.Entity<TlkpDepartemen>().ToTable("tlkp_departemen", t => t.ExcludeFromMigrations());
modelBuilder.Entity<TlkpSection>().ToTable("tlkp_section", t => t.ExcludeFromMigrations());
modelBuilder.Entity<TlkpLine>().ToTable("tlkp_line", t => t.ExcludeFromMigrations());
modelBuilder.Entity<TlkpGroup>().ToTable("tlkp_group", t => t.ExcludeFromMigrations());
modelBuilder.Entity<TlkpShift>().ToTable("tlkp_shift", t => t.ExcludeFromMigrations());
modelBuilder.Entity<TlkpLineGroup>().ToTable("tlkp_lineGroup", t => t.ExcludeFromMigrations());
modelBuilder.Entity<TlkpOperator>().ToTable("tlkp_operator", t => t.ExcludeFromMigrations());
modelBuilder.Entity<TlkpUserKasie>().ToTable("tlkp_userKasie", t => t.ExcludeFromMigrations());
modelBuilder.Entity<TlkpUser>().ToTable("tlkp_user", t => t.ExcludeFromMigrations());
modelBuilder.Entity<TlkpItem>().ToTable("tlkp_item", t => t.ExcludeFromMigrations());
modelBuilder.Entity<TlkpKategori>().ToTable("tlkp_kategori", t => t.ExcludeFromMigrations());
modelBuilder.Entity<TlkpSeries>().ToTable("tlkp_series", t => t.ExcludeFromMigrations());
modelBuilder.Entity<TlkpMold>().ToTable("tlkp_mold", t => t.ExcludeFromMigrations());
modelBuilder.Entity<TlkpMoldType>().ToTable("tlkp_moldtype", t => t.ExcludeFromMigrations());
modelBuilder.Entity<TlkpJob>().ToTable("tlkp_job", t => t.ExcludeFromMigrations());
modelBuilder.Entity<ViewDataAuth>().ToTable("VIEW_DATAAUTH", t => t.ExcludeFromMigrations());
modelBuilder.Entity<ViewEmployee>().ToTable("VIEW_EMPLOYEE", t => t.ExcludeFromMigrations());
```

### STEP 5: Hapus Seed Data Master Data

Di method `SeedData()`, **hapus semua** `mb.Entity<TlkpXxx>().HasData(...)` dan `mb.Entity<ViewDataAuth>().HasData(...)`.

Yang harus **dihapus** (data sudah ada di SQL Server):
- `mb.Entity<TlkpShift>().HasData(...)`
- `mb.Entity<TlkpSection>().HasData(...)`
- `mb.Entity<TlkpLine>().HasData(...)`
- `mb.Entity<TlkpGroup>().HasData(...)`
- `mb.Entity<TlkpJob>().HasData(...)`
- `mb.Entity<TlkpUserKasie>().HasData(...)`
- `mb.Entity<TlkpLineGroup>().HasData(...)`
- `mb.Entity<TlkpOperator>().HasData(...)`
- `mb.Entity<ViewDataAuth>().HasData(...)`
- `mb.Entity<TlkpMold>().HasData(...)`

Yang harus **tetap ada** (tabel baru, perlu seed):
- `mb.Entity<CosFormDefinition>().HasData(...)`
- `mb.Entity<CosCheckItem>().HasData(...)`
- `mb.Entity<CosCheckSubRow>().HasData(...)`
- `mb.Entity<CosBatteryType>().HasData(...)`
- `mb.Entity<CosBatteryStandard>().HasData(...)`
- `mb.Entity<CosProblemColumn>().HasData(...)`
- `mb.Entity<CosSignatureSlot>().HasData(...)`

### STEP 6: Hapus Migration Lama, Buat Baru

```bash
cd /home/ubuntu/gs/form/backend

# Hapus folder migrations lama (MySQL-based)
rm -rf Migrations/

# Buat migration baru (hanya akan membuat tabel cos_*)
dotnet ef migrations add InitCosTablesOnSqlServer
```

> **Verifikasi**: Buka file migration yang dibuat di `Migrations/`. Pastikan:
> - ✅ Ada `CREATE TABLE cos_form_definitions`, `cos_check_items`, dll
> - ✅ Ada `INSERT` seed data untuk tabel COS
> - ❌ TIDAK ada `CREATE TABLE tlkp_*` atau `VIEW_*`
> - ❌ TIDAK ada `INSERT` ke tabel `tlkp_*`

### STEP 7: Apply Migration ke SQL Server

```bash
# Jalankan migration
dotnet ef database update
```

Atau biarkan auto-migrate lewat `Program.cs`:

```csharp
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FormCosDbContext>();
    db.Database.Migrate();
}
```

### STEP 8: Test

```bash
# Start backend
dotnet run --urls "http://0.0.0.0:5131"

# Test endpoint yang baca master data (harus return data asli SQL Server)
curl http://localhost:5131/api/battery/lines
curl http://localhost:5131/api/battery/shifts
curl http://localhost:5131/api/battery/molds
curl http://localhost:5131/api/personnel/operators

# Test endpoint COS (harus return seed data)
curl http://localhost:5131/api/battery/types
curl http://localhost:5131/api/checkitem
curl http://localhost:5131/api/formdefinition/by-code/COS_VALIDATION
```

---

## Ringkasan Perubahan File

| File | Apa yang berubah |
|------|-----------------|
| `backend.csproj` | `Pomelo.EntityFrameworkCore.MySql` → `Microsoft.EntityFrameworkCore.SqlServer` |
| `appsettings.json` | Connection string MySQL → SQL Server |
| `Program.cs` | `UseMySql(...)` → `UseSqlServer(...)` |
| `FormCosDbContext.cs` | Tambah `ExcludeFromMigrations()` pada 19 tabel master data |
| `FormCosDbContext.cs` | Hapus seed data `TlkpXxx` dan `ViewDataAuth` |
| `Migrations/` | Hapus lama, buat baru (hanya tabel `cos_*`) |

> **Model files (`MasterData.cs`, `CosModels.cs`) dan semua Controller TIDAK perlu diubah sama sekali.**

---

## ⚠️ Catatan Penting

1. **VIEW_DATAAUTH & VIEW_EMPLOYEE** — Di SQL Server asli, ini adalah **VIEW** (bukan TABLE). 
   EF Core bisa membaca dari View dengan normal selama kolom mapping-nya sesuai.
   `ExcludeFromMigrations()` memastikan EF tidak mencoba membuat/alter view tersebut.

2. **Composite Primary Key** `tlkp_operator (user_id, jopr_id)` — Konfigurasi `HasKey()` 
   di DbContext tetap dibutuhkan supaya EF tahu cara query-nya, walaupun tabelnya sudah ada.

3. **Schema** — Jika tabel master data ada di schema selain `dbo` (misal `[dbo].[tlkp_plant]`), 
   tambahkan schema pada `ToTable`:
   ```csharp
   modelBuilder.Entity<TlkpPlant>()
       .ToTable("tlkp_plant", "dbo", t => t.ExcludeFromMigrations());
   ```

4. **Nama kolom** — Model `MasterData.cs` sudah memakai `[Column("xxx")]` attribute yang sesuai 
   dengan nama kolom di `script.sql`. Jika ada perbedaan kecil (case sensitivity, dll), 
   cek dan sesuaikan attribute `[Column]` di model.

5. **Auto-migrate di production** — Pertimbangkan untuk TIDAK auto-migrate di production. 
   Gunakan `dotnet ef database update` secara manual, atau apply SQL script dari migration.
   Untuk generate SQL script tanpa langsung apply:
   ```bash
   dotnet ef migrations script -o cos_migration.sql
   ```
   Lalu review dan jalankan script tersebut secara manual di SQL Server Management Studio.
