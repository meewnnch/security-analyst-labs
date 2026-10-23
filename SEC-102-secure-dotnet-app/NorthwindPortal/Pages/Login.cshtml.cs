using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace NorthwindPortal.Pages;

public class LoginModel : PageModel
{
    [BindProperty] public string Username { get; set; } = "";
    [BindProperty] public string Password { get; set; } = "";
    public string Message { get; set; } = "";
    private static readonly Dictionary<string, (int Fails, DateTime LockedUntil)> Attempts = new();
    private const int MaxFails = 3;

    public void OnPost()
    {
        if (Attempts.TryGetValue(Username, out var a) && a.LockedUntil > DateTime.UtcNow)
        {
            Message = "Account locked. Try again later.";
            return;
        }
        using var conn = new SqliteConnection("Data Source=northwind.db");
        conn.Open();
        // --VULNERABLE: user input is glued straight into the SQL
        // -----------------------------------------------------------------
        // var sql = $"SELECT Username FROM Users WHERE Username = '{Username}' AND Password = '{Password}'";
        // var result = cmd.ExecuteScalar();
        // -----------------------------------------------------------------
        
        // SECURE: Parameterized query: input is sent as data, never as SQL code
        // -----------------------------------------------------------------
        // var sql = "SELECT Username FROM Users WHERE Username = @u AND Password = @p";
        // using var cmd = new SqliteCommand(sql, conn);
        // cmd.Parameters.AddWithValue("@u", Username);
        // cmd.Parameters.AddWithValue("@p", Password);
        // var result = cmd.ExecuteScalar();
        // Message = result != null ? $"Welcome, {result}!" : "Invalid login.";
        // -----------------------------------------------------------------

        // SECURE: Parameterized query with password *hashing*: input is sent as data, never as SQL code
        var sql = "SELECT PasswordHash FROM Users WHERE Username = @u";
        using var cmd = new SqliteCommand(sql, conn);
        cmd.Parameters.AddWithValue("@u", Username);
        var hash = cmd.ExecuteScalar() as string;

        var hasher = new PasswordHasher<string>();
        bool ok = hash != null &&
            hasher.VerifyHashedPassword(Username, hash, Password) != PasswordVerificationResult.Failed;
        if (ok)
        {
            Attempts.Remove(Username);
            Message = $"Welcome, {Username}!";
        }
        else
        {
            var fails = Attempts.TryGetValue(Username, out var f) ? f.Fails + 1 : 1;
            var lockedUntil = fails >= MaxFails ? DateTime.UtcNow.AddMinutes(5) : DateTime.MinValue;
            Attempts[Username] = (fails, lockedUntil);
            Message = fails >= MaxFails ? "Account locked. Try again later." : "Invalid login.";
        }
    }
}