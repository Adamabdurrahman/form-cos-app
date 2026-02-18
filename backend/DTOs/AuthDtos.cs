using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

// ══════════════════════════════════════════════════
// AUTH DTOs — Request/Response untuk Auth Overhaul
// ══════════════════════════════════════════════════

/// <summary>POST /api/auth/register</summary>
public class RegisterRequest
{
    [Required(ErrorMessage = "NPK wajib diisi.")]
    [StringLength(20)]
    public string Npk { get; set; } = string.Empty;

    [Required(ErrorMessage = "Username wajib diisi.")]
    [StringLength(50, MinimumLength = 4, ErrorMessage = "Username harus 4-50 karakter.")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username hanya boleh huruf, angka, dan underscore.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password wajib diisi.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password minimal 6 karakter.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Konfirmasi password wajib diisi.")]
    [Compare("Password", ErrorMessage = "Password dan konfirmasi tidak cocok.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>POST /api/auth/login</summary>
public class LoginRequest
{
    [Required(ErrorMessage = "Username wajib diisi.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password wajib diisi.")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>POST /api/auth/validate-npk (step 1 reset password)</summary>
public class ValidateNpkRequest
{
    [Required(ErrorMessage = "NPK wajib diisi.")]
    public string Npk { get; set; } = string.Empty;
}

/// <summary>POST /api/auth/reset-password</summary>
public class ResetPasswordRequest
{
    [Required(ErrorMessage = "NPK wajib diisi.")]
    public string Npk { get; set; } = string.Empty;

    [StringLength(50, MinimumLength = 4, ErrorMessage = "Username baru harus 4-50 karakter.")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username hanya boleh huruf, angka, dan underscore.")]
    public string? NewUsername { get; set; }

    [Required(ErrorMessage = "Password baru wajib diisi.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password minimal 6 karakter.")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Konfirmasi password wajib diisi.")]
    [Compare("NewPassword", ErrorMessage = "Password dan konfirmasi tidak cocok.")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

/// <summary>Unified response for all auth endpoints</summary>
public class AuthResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
    public AuthUserData? User { get; set; }
}

/// <summary>User data returned on login/verify</summary>
public class AuthUserData
{
    public string EmpId { get; set; } = string.Empty;
    public string Npk { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string JobName { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Section { get; set; }
}
