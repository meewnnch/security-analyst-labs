var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Create the database and seed it with initial data
// ------------------------------------------------------------------------
// using (var conn = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=northwind.db"))
// {
//     conn.Open();
//     var cmd = conn.CreateCommand();
//     cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Users (Username TEXT, Password TEXT);
//         DELETE FROM Users;
//         INSERT INTO Users VALUES ('admin','Winter2026!'), ('reception','password123');";
//     cmd.ExecuteNonQuery();
// }
// ------------------------------------------------------------------------

// Fix: Create the database and seed it with initial data
// salted hashes, which work one way only: the app can check a password against the hash, but nobody can turn the hash back into the password.
using (var conn = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=northwind.db"))
{
    conn.Open();
    var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<string>();
    var cmd = conn.CreateCommand();
    cmd.CommandText = @"DROP TABLE IF EXISTS Users;
        CREATE TABLE Users (Username TEXT, PasswordHash TEXT);
        INSERT INTO Users VALUES ('admin', @h1), ('reception', @h2);";
    cmd.Parameters.AddWithValue("@h1", hasher.HashPassword("admin", "Winter2026!"));
    cmd.Parameters.AddWithValue("@h2", hasher.HashPassword("reception", "password123"));
    cmd.ExecuteNonQuery();
}

// Custom middleware: runs on every request before the page is built
app.Use(async (context, next) =>
{
    // Don't let any site show this page inside a frame (blocks clickjacking)
    context.Response.Headers["X-Frame-Options"] = "DENY";

    // Browser must use the declared file type, never guess (blocks MIME-sniffing tricks)
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";

    // Don't send this page's URL to other sites when a link is clicked (stops URL leaks)
    context.Response.Headers["Referrer-Policy"] = "no-referrer";

    // Only load scripts, styles and images from our own site (helps block XSS)
    context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; img-src 'self' data:";

    // Hand the request to the next step in the pipeline (the actual page)
    await next();
}); // End of middleware

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
