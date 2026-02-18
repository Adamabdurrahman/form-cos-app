using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly MasterDbContext _masterDb;
    public AuthController(MasterDbContext masterDb) => _masterDb = masterDb;

    /// <summary>
    /// POST /api/auth/login
    /// Body: { "username": "Full_Name", "password": "npk" }
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { success = false, error = "Username dan password wajib diisi." });

        // ── STEP 1: Cari employee berdasarkan Full_Name (case-insensitive) ──
        var auth = await _masterDb.ViewDataAuths
            .FirstOrDefaultAsync(a => a.FullName != null &&
                a.FullName.ToUpper() == request.Username.ToUpper());

        if (auth == null)
            return Unauthorized(new { success = false, error = "Nama tidak ditemukan." });

        // ── STEP 2: Validasi NPK dari VIEW_EMPLOYEE ──
        var employee = await _masterDb.ViewEmployees
            .FirstOrDefaultAsync(e => e.EmpId == auth.EmpId);

        if (employee == null)
            return Unauthorized(new { success = false, error = "Data karyawan tidak ditemukan." });

        if (string.IsNullOrWhiteSpace(employee.Npk) || employee.Npk.Trim() != request.Password.Trim())
            return Unauthorized(new { success = false, error = "NPK salah." });

        // ── STEP 3: Deteksi Role berdasarkan tabel-tabel relasi ──
        var empId = auth.EmpId!;
        var role = "unknown";
        var jobName = "";

        // Priority: Kasie > Kasubsie > Leader > Operator
        // (Seseorang bisa terdaftar di beberapa tempat, ambil yang tertinggi)

        // Check 1: Kasie? — ada di tlkp_userKasie.kasie_emp_id
        var isKasie = await _masterDb.TlkpUserKasies
            .AnyAsync(uk => uk.KasieEmpId == empId);
        if (isKasie)
        {
            role = "kasie";
            jobName = "Kasie";
        }

        // Check 2: Kasubsie? — ada di tlkp_lineGroup.lgp_kasubsie
        if (role == "unknown")
        {
            var isKasubsie = await _masterDb.TlkpLineGroups
                .AnyAsync(lg => lg.LgpKasubsie == empId);
            if (isKasubsie)
            {
                role = "kasubsie";
                jobName = "Kasubsie";
            }
        }

        // Check 3: Leader? — ada di tlkp_lineGroup.lgp_leader
        if (role == "unknown")
        {
            var isLeader = await _masterDb.TlkpLineGroups
                .AnyAsync(lg => lg.LgpLeader == empId);
            if (isLeader)
            {
                role = "leader";
                jobName = "Leader";
            }
        }

        // Check 4: Operator? — ada di tlkp_operator, JOIN tlkp_job → jopr_name LIKE 'OP%'
        if (role == "unknown")
        {
            var operatorRecord = await _masterDb.TlkpOperators
                .Where(op => op.UserId == empId)
                .FirstOrDefaultAsync();

            if (operatorRecord != null)
            {
                // Cek nama jabatan dari tlkp_job
                var job = await _masterDb.TlkpJobs
                    .FirstOrDefaultAsync(j => j.JoprId == operatorRecord.JoprId);

                if (job != null && job.JoprName != null)
                {
                    jobName = job.JoprName;

                    // Deteksi operator via jopr_name prefix
                    var nameUpper = job.JoprName.ToUpper();
                    if (nameUpper.StartsWith("OP"))
                        role = "operator";
                    else if (nameUpper.Contains("LEADER"))
                        role = "leader";
                    else if (nameUpper.Contains("KASUBSIE") || nameUpper.Contains("KASUBS"))
                        role = "kasubsie";
                    else if (nameUpper.Contains("KASIE") || nameUpper.Contains("KASI "))
                        role = "kasie";
                    else
                        role = "operator"; // Default fallback jika terdaftar di tlkp_operator
                }
                else
                {
                    role = "operator"; // Terdaftar di tlkp_operator tapi jopr kosong
                    jobName = "Operator";
                }
            }
        }

        // Fallback: jika tidak ditemukan di tabel manapun
        if (role == "unknown")
        {
            // Coba pakai pos_name_id dari VIEW_DATAAUTH sebagai fallback
            if (!string.IsNullOrWhiteSpace(auth.PosNameId))
            {
                var posUpper = auth.PosNameId.ToUpper();
                if (posUpper.Contains("OPERATOR") || posUpper.StartsWith("OP"))
                { role = "operator"; jobName = auth.PosNameId; }
                else if (posUpper.Contains("LEADER"))
                { role = "leader"; jobName = auth.PosNameId; }
                else if (posUpper.Contains("KASUBSIE"))
                { role = "kasubsie"; jobName = auth.PosNameId; }
                else if (posUpper.Contains("KASIE"))
                { role = "kasie"; jobName = auth.PosNameId; }
                else
                { role = "operator"; jobName = auth.PosNameId; }
            }
            else
            {
                role = "operator";
                jobName = "Staff";
            }
        }

        // ── STEP 4: Generate simple session token ──
        // NOTE: Untuk production, gunakan JWT. Ini simple token untuk dev.
        var token = Convert.ToBase64String(
            System.Text.Encoding.UTF8.GetBytes($"{empId}:{role}:{DateTime.UtcNow.Ticks}"));

        return Ok(new
        {
            success = true,
            user = new
            {
                empId,
                fullName = auth.FullName,
                npk = employee.Npk,
                role,
                jobName,
                department = employee.Dep,
                section = employee.Sec,
            },
            token,
        });
    }

    /// <summary>
    /// GET /api/auth/verify — validate token and return user info
    /// </summary>
    [HttpGet("verify")]
    public async Task<IActionResult> Verify([FromHeader(Name = "Authorization")] string? authHeader)
    {
        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            return Unauthorized(new { success = false, error = "Token diperlukan." });

        try
        {
            var token = authHeader["Bearer ".Length..];
            var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var parts = decoded.Split(':');
            if (parts.Length < 2) return Unauthorized(new { success = false, error = "Token tidak valid." });

            var empId = parts[0];
            var role = parts[1];

            var auth = await _masterDb.ViewDataAuths
                .FirstOrDefaultAsync(a => a.EmpId == empId);

            if (auth == null)
                return Unauthorized(new { success = false, error = "User tidak ditemukan." });

            var employee = await _masterDb.ViewEmployees
                .FirstOrDefaultAsync(e => e.EmpId == empId);

            return Ok(new
            {
                success = true,
                user = new
                {
                    empId,
                    fullName = auth.FullName,
                    npk = employee?.Npk,
                    role,
                    department = employee?.Dep,
                    section = employee?.Sec,
                },
            });
        }
        catch
        {
            return Unauthorized(new { success = false, error = "Token tidak valid." });
        }
    }
}

// ── Request DTO ──
public class LoginRequest
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
