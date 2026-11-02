using AISmokingDetection.Data;
using AISmokingDetection.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var apiProjectPath = Path.GetFullPath(
    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "AISmokingDetection")
);

var settingsPath = Path.Combine(apiProjectPath, "appsettings.json");

if (!File.Exists(settingsPath))
{
    Console.WriteLine("Could not find the API appsettings.json file.");
    return;
}

var configuration = new ConfigurationBuilder()
    .SetBasePath(apiProjectPath)
    .AddJsonFile("appsettings.json", optional: false)
    .AddUserSecrets<AISmokingDetection.Program>(optional: true)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("DefaultConnection is missing.");
    return;
}

var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlServer(connectionString)
    .Options;

Console.WriteLine("Create the first local Administrator account.");
Console.WriteLine("Database: AISmokingDetectionDB");
Console.WriteLine();

Console.Write("Administrator full name: ");
var fullName = Console.ReadLine()?.Trim();

Console.Write("Administrator email: ");
var email = Console.ReadLine()?.Trim();

if (string.IsNullOrWhiteSpace(fullName) ||
    string.IsNullOrWhiteSpace(email))
{
    Console.WriteLine("Name and email are required.");
    return;
}

Console.Write("Password (minimum 12 characters): ");
var password = ReadHiddenPassword();
Console.WriteLine();

if (password.Length < 12)
{
    Console.WriteLine("Password must be at least 12 characters.");
    return;
}

Console.Write("Confirm password: ");
var confirmPassword = ReadHiddenPassword();
Console.WriteLine();

if (password != confirmPassword)
{
    Console.WriteLine("Passwords do not match.");
    return;
}

await using var context = new AppDbContext(options);

var canConnect = await context.Database.CanConnectAsync();

if (!canConnect)
{
    Console.WriteLine("Could not connect to the configured database.");
    return;
}

var databaseName = context.Database.GetDbConnection().Database;

if (!string.Equals(
    databaseName,
    "AISmokingDetectionDB",
    StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine($"Unexpected database: {databaseName}. No account was created.");
    return;
}

if (await context.Staff.AnyAsync())
{
    Console.WriteLine("Staff records already exist. No account was created.");
    return;
}

var staff = new Staff
{
    FullName = fullName,
    Email = email,
    Role = "Administrator",
    IsActive = true
};

staff.PasswordHash = new PasswordHasher<Staff>()
    .HashPassword(staff, password);

context.Staff.Add(staff);
await context.SaveChangesAsync();

Console.WriteLine();
Console.WriteLine("Administrator account created successfully.");
Console.WriteLine($"StaffId: {staff.StaffId}");

static string ReadHiddenPassword()
{
    var password = new System.Text.StringBuilder();

    while (true)
    {
        var key = Console.ReadKey(intercept: true);

        if (key.Key == ConsoleKey.Enter)
        {
            break;
        }

        if (key.Key == ConsoleKey.Backspace)
        {
            if (password.Length > 0)
            {
                password.Length--;
            }

            continue;
        }

        if (!char.IsControl(key.KeyChar))
        {
            password.Append(key.KeyChar);
        }
    }

    return password.ToString();
}
