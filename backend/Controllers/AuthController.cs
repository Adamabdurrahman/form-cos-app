using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Data;
using backend.DTOs;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly CosDbContext _cosDb;
    private readonly MasterDbContext _masterDb;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthController> _logger;

    public AuthController(CosDbContext cosDb, MasterDbContext masterDb,
        IConfiguration config, ILogger<AuthController> logger)
    {
        _cosDb = cosDb;
        _masterDb = masterDb;
        _config = config;
        _logger = logger;
    }

    // ═══════════════════════════════════════════════
    // FEATURE 1: REGISTER (Create Account)
    // POST /api/auth/register
    // ═══════════════════════════════════════════════
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
            });

        try
        {
            // Validasi 1: Cek NPK di VIEW_EMPLOYEE (db_master_data)
            var employee = await _masterDb.ViewEmployees
                .FirstOrDefaultAsync(e => e.Npk == request.Npk.Trim());

            if (employee == null)
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "NPK tidak terdaftar sebagai karyawan."
                });

            // Validasi 2: Cek apakah NPK sudah punya akun
            var existsByNpk = await _cosDb.UserAccounts
                .AnyAsync(u => u.Npk == request.Npk.Trim());

            if (existsByNpk)
                return Conflict(new AuthResponse
                {
                    Success = false,
                    Message = "NPK ini sudah memiliki akun. Gunakan fitur Reset Password jika lupa."
                });

            // Validasi 3: Cek ketersediaan username
            var existsByUsername = await _cosDb.UserAccounts
                .AnyAsync(u => u.Username == request.Username.Trim().ToLower());

            if (existsByUsername)
                return Conflict(new AuthResponse
                {
                    Success = false,
                    Message = "Username sudah digunakan. Pilih username lain."
                });

            // Simpan akun baru (password di-hash dengan BCrypt)
            var newAccount = new UserAccount
            {
                Npk = request.Npk.Trim(),
                Username = request.Username.Trim().ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            _cosDb.UserAccounts.Add(newAccount);
            await _cosDb.SaveChangesAsync();

            // Resolve employee name for response
            var auth = await _masterDb.ViewDataAuths
                .FirstOrDefaultAsync(a => a.EmpId == employee.EmpId);

            var employeeName = auth?.FullName ?? employee.EmpId ?? "Karyawan";

            _logger.LogInformation("Account created for NPK: {Npk}, Username: {Username}",
                request.Npk, request.Username);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = $"Akun berhasil dibuat untuk {employeeName}. Silakan login."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for NPK: {Npk}", request.Npk);
            return StatusCode(500, new AuthResponse
            {
                Success = false,
                Message = "Terjadi kesalahan server. Silakan coba lagi."
            });
        }
    }

    // ═══════════════════════════════════════════════
    // FEATURE 2: LOGIN (Username + Password → JWT)
    // POST /api/auth/login
    // ═══════════════════════════════════════════════
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new AuthResponse { Success = false, Message = "Username dan password wajib diisi." });

        try
        {
            // Step A: Cari user berdasarkan username di t_cos_user_accounts
            var account = await _cosDb.UserAccounts
                .FirstOrDefaultAsync(u => u.Username == request.Username.Trim().ToLower());

            if (account == null)
                return Unauthorized(new AuthResponse
                    { Success = false, Message = "Username atau password salah." });

            // Step B: Verifikasi password hash
            if (!BCrypt.Net.BCrypt.Verify(request.Password, account.PasswordHash))
                return Unauthorized(new AuthResponse
                    { Success = false, Message = "Username atau password salah." });

            // Step C: Ambil NPK dari akun → cari data karyawan di db_master_data
            var npk = account.Npk;

            var employee = await _masterDb.ViewEmployees
                .FirstOrDefaultAsync(e => e.Npk == npk);

            if (employee == null)
                return StatusCode(500, new AuthResponse
                {
                    Success = false,
                    Message = "Data karyawan tidak ditemukan di master data."
                });

            var auth = await _masterDb.ViewDataAuths
                .FirstOrDefaultAsync(a => a.EmpId == employee.EmpId);

            if (auth == null)
                return StatusCode(500, new AuthResponse
                {
                    Success = false,
                    Message = "Data autentikasi karyawan tidak ditemukan."
                });

            // Step D: Deteksi role (logic existing yang dipertahankan)
            var empId = auth.EmpId!;
            var (role, jobName) = await DetectRole(empId, auth);

            // Step E: Generate JWT Token (payload berisi NPK dan Role)
            var token = GenerateJwtToken(empId, npk, account.Username, auth.FullName ?? "", role);

            _logger.LogInformation("Login success: {Username} (NPK: {Npk}, Role: {Role})",
                account.Username, npk, role);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Login berhasil.",
                Token = token,
                User = new AuthUserData
                {
                    EmpId = empId,
                    Npk = npk,
                    FullName = auth.FullName ?? "",
                    Username = account.Username,
                    Role = role,
                    JobName = jobName,
                    Department = employee.Dep,
                    Section = employee.Sec,
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for Username: {Uname}", request.Username);
            return StatusCode(500, new AuthResponse
            {
                Success = false,
                Message = "Terjadi kesalahan server. Silakan coba lagi."
            });
        }
    }

    // ═══════════════════════════════════════════════
    // FEATURE 3a: VALIDATE NPK (step 1 untuk reset password)
    // POST /api/auth/validate-npk
    // ═══════════════════════════════════════════════
    [HttpPost("validate-npk")]
    public async Task<IActionResult> ValidateNpk([FromBody] ValidateNpkRequest request)
    {
        try
        {
            // Cek NPK di master data
            var employee = await _masterDb.ViewEmployees
                .FirstOrDefaultAsync(e => e.Npk == request.Npk.Trim());

            if (employee == null)
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "NPK tidak terdaftar sebagai karyawan."
                });

            // Cek apakah NPK punya akun
            var account = await _cosDb.UserAccounts
                .FirstOrDefaultAsync(u => u.Npk == request.Npk.Trim());

            if (account == null)
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "NPK ini belum memiliki akun. Silakan buat akun terlebih dahulu."
                });

            // Resolve full name
            var auth = await _masterDb.ViewDataAuths
                .FirstOrDefaultAsync(a => a.EmpId == employee.EmpId);

            return Ok(new
            {
                success = true,
                message = "NPK valid.",
                data = new
                {
                    npk = request.Npk.Trim(),
                    fullName = auth?.FullName ?? employee.EmpId ?? "",
                    currentUsername = account.Username,
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating NPK: {Npk}", request.Npk);
            return StatusCode(500, new AuthResponse { Success = false, Message = "Terjadi kesalahan server." });
        }
    }

    // ═══════════════════════════════════════════════
    // FEATURE 3b: RESET PASSWORD
    // POST /api/auth/reset-password
    // ═══════════════════════════════════════════════
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
            });

        try
        {
            // Validasi NPK di master data
            var employee = await _masterDb.ViewEmployees
                .FirstOrDefaultAsync(e => e.Npk == request.Npk.Trim());

            if (employee == null)
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "NPK tidak terdaftar sebagai karyawan."
                });

            // Cari akun berdasarkan NPK
            var account = await _cosDb.UserAccounts
                .FirstOrDefaultAsync(u => u.Npk == request.Npk.Trim());

            if (account == null)
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "NPK ini belum memiliki akun."
                });

            // Cek username baru (jika ingin ganti)
            if (!string.IsNullOrWhiteSpace(request.NewUsername))
            {
                var newUsernameLower = request.NewUsername.Trim().ToLower();
                var usernameExists = await _cosDb.UserAccounts
                    .AnyAsync(u => u.Username == newUsernameLower && u.Id != account.Id);

                if (usernameExists)
                    return Conflict(new AuthResponse
                    {
                        Success = false,
                        Message = "Username baru sudah digunakan. Pilih username lain."
                    });

                account.Username = newUsernameLower;
            }

            // Update password
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, workFactor: 12);
            account.UpdatedAt = DateTime.UtcNow;

            await _cosDb.SaveChangesAsync();

            _logger.LogInformation("Password reset for NPK: {Npk}", request.Npk);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Password berhasil direset. Silakan login dengan kredensial baru."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset for NPK: {Npk}", request.Npk);
            return StatusCode(500, new AuthResponse
            {
                Success = false,
                Message = "Terjadi kesalahan server."
            });
        }
    }

    // ═══════════════════════════════════════════════
    // VERIFY TOKEN (validate JWT and return user info)
    // GET /api/auth/verify
    // ═══════════════════════════════════════════════
    [HttpGet("verify")]
    public async Task<IActionResult> Verify()
    {
        try
        {
            // Extract token from Authorization header
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized(new AuthResponse { Success = false, Message = "Token diperlukan." });

            var tokenStr = authHeader["Bearer ".Length..];

            // Validate JWT
            var jwtSettings = _config.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"]!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal;
            try
            {
                principal = tokenHandler.ValidateToken(tokenStr, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(2),
                }, out _);
            }
            catch
            {
                return Unauthorized(new AuthResponse { Success = false, Message = "Token tidak valid atau sudah kadaluarsa." });
            }

            var empId = principal.FindFirst("empId")?.Value;
            var npk = principal.FindFirst("npk")?.Value;
            var role = principal.FindFirst(ClaimTypes.Role)?.Value ?? principal.FindFirst("role")?.Value;
            var username = principal.FindFirst("username")?.Value;

            if (string.IsNullOrEmpty(empId))
                return Unauthorized(new AuthResponse { Success = false, Message = "Token tidak valid." });

            // Fetch fresh user data
            var auth = await _masterDb.ViewDataAuths
                .FirstOrDefaultAsync(a => a.EmpId == empId);

            var employee = await _masterDb.ViewEmployees
                .FirstOrDefaultAsync(e => e.EmpId == empId);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Token valid.",
                User = new AuthUserData
                {
                    EmpId = empId,
                    Npk = npk ?? "",
                    FullName = auth?.FullName ?? "",
                    Username = username ?? "",
                    Role = role ?? "operator",
                    JobName = role ?? "operator",
                    Department = employee?.Dep,
                    Section = employee?.Sec,
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying token");
            return Unauthorized(new AuthResponse { Success = false, Message = "Token tidak valid." });
        }
    }

    // ═══════════════════════════════════════════════
    // HELPER: Detect Role (logic existing dipertahankan 100%)
    // Priority: Kasie > Kasubsie > Leader > Operator
    // ═══════════════════════════════════════════════
    private async Task<(string role, string jobName)> DetectRole(string empId, ViewDataAuth auth)
    {
        // Check 1: Kasie? — ada di tlkp_userKasie.kasie_emp_id
        var isKasie = await _masterDb.TlkpUserKasies
            .AnyAsync(uk => uk.KasieEmpId == empId);
        if (isKasie) return ("kasie", "Kasie");

        // Check 2: Kasubsie? — ada di tlkp_lineGroup.lgp_kasubsie
        var isKasubsie = await _masterDb.TlkpLineGroups
            .AnyAsync(lg => lg.LgpKasubsie == empId);
        if (isKasubsie) return ("kasubsie", "Kasubsie");

        // Check 3: Leader? — ada di tlkp_lineGroup.lgp_leader
        var isLeader = await _masterDb.TlkpLineGroups
            .AnyAsync(lg => lg.LgpLeader == empId);
        if (isLeader) return ("leader", "Leader");

        // Check 4: Operator? — ada di tlkp_operator, JOIN tlkp_job
        var operatorRec = await _masterDb.TlkpOperators
            .Where(op => op.UserId == empId)
            .FirstOrDefaultAsync();

        if (operatorRec != null)
        {
            var job = await _masterDb.TlkpJobs
                .FirstOrDefaultAsync(j => j.JoprId == operatorRec.JoprId);

            if (job?.JoprName != null)
            {
                var nameUpper = job.JoprName.ToUpper();
                if (nameUpper.StartsWith("OP"))
                    return ("operator", job.JoprName);
                if (nameUpper.Contains("LEADER"))
                    return ("leader", job.JoprName);
                if (nameUpper.Contains("KASUBSIE") || nameUpper.Contains("KASUBS"))
                    return ("kasubsie", job.JoprName);
                if (nameUpper.Contains("KASIE") || nameUpper.Contains("KASI "))
                    return ("kasie", job.JoprName);

                return ("operator", job.JoprName);
            }
            return ("operator", "Operator");
        }

        // Fallback: pos_name_id dari VIEW_DATAAUTH
        if (!string.IsNullOrWhiteSpace(auth.PosNameId))
        {
            var posUpper = auth.PosNameId.ToUpper();
            if (posUpper.Contains("LEADER")) return ("leader", auth.PosNameId);
            if (posUpper.Contains("KASUBSIE")) return ("kasubsie", auth.PosNameId);
            if (posUpper.Contains("KASIE")) return ("kasie", auth.PosNameId);
        }

        return ("operator", "Staff");
    }

    // ═══════════════════════════════════════════════
    // HELPER: Generate JWT Token
    // Payload: empId, npk, username, fullName, role
    // ═══════════════════════════════════════════════
    private string GenerateJwtToken(string empId, string npk, string username, string fullName, string role)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"]
            ?? throw new InvalidOperationException("JWT SecretKey not configured.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("empId", empId),
            new Claim("npk", npk),
            new Claim("username", username),
            new Claim(ClaimTypes.Name, fullName),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),
        };

        var expiryHours = double.Parse(jwtSettings["ExpiryInHours"] ?? "8");

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expiryHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
