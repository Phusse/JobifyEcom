using System.Security.Cryptography;

Console.WriteLine("\n--- Jobify Secret Generator ---\n");

List<(string Path, string Value)> secrets = [];

// 1. Data Encryption Keys
Console.Write("How many NEW encryption keys do you want to generate? (default: 1): ");
string? numKeysInput = Console.ReadLine();
int numKeys = int.TryParse(numKeysInput, out int nk) ? nk : (numKeysInput == "0" ? 0 : 1);

Console.Write("Do you have any OLD encryption keys to keep? (comma-separated list, or press enter for none): ");
string? oldKeysInput = Console.ReadLine();
string[] oldKeys = string.IsNullOrWhiteSpace(oldKeysInput)
    ? []
    : oldKeysInput.Split(',').Select(k => k.Trim()).Where(k => !string.IsNullOrEmpty(k)).ToArray();

int currentVersion = oldKeys.Length + numKeys;

// Add existing keys first
for (int i = 0; i < oldKeys.Length; i++)
{
    secrets.Add(($"DataEncryption:Keys:{i + 1}", oldKeys[i]));
}

// Generate new keys
for (int i = 0; i < numKeys; i++)
{
    int keyVersion = oldKeys.Length + i + 1;
    secrets.Add(($"DataEncryption:Keys:{keyVersion}", GenerateKey(32)));
}

if (currentVersion > 0)
{
    secrets.Add(("DataEncryption:CurrentKeyVersion", currentVersion.ToString()));
}

// 2. Hashing Keys
Console.Write("Generate new EmailHmacKey? (y/n, default: y): ");
string? generateHmac = Console.ReadLine();
if (!string.Equals(generateHmac, "n", StringComparison.OrdinalIgnoreCase))
{
    secrets.Add(("Hashing:EmailHmacKey", GenerateKey(64)));
}

// 3. Database & Redis placeholders
Console.Write("Include placeholders for DB and Redis? (y/n, default: n): ");
string? includePlaceholders = Console.ReadLine();
if (string.Equals(includePlaceholders, "y", StringComparison.OrdinalIgnoreCase))
{
    secrets.Add(("ConnectionStrings:Database", "Server=localhost;Database=JobifyDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"));
    secrets.Add(("ConnectionStrings:Redis", "localhost:6379"));
}

Console.WriteLine("\n--- Generated Commands ---\n");
Console.WriteLine("Copy and paste the following commands into your terminal (run from src/Jobify.Api directory):\n");

foreach ((string Path, string Value) in secrets)
{
    Console.WriteLine($"dotnet user-secrets set \"{Path}\" \"{Value}\"");
}

Console.WriteLine("\n--- End of Commands ---\n");

static string GenerateKey(int bytes)
{
    byte[] data = new byte[bytes];
    using RandomNumberGenerator rng = RandomNumberGenerator.Create();
    rng.GetBytes(data);
    return Convert.ToBase64String(data);
}
