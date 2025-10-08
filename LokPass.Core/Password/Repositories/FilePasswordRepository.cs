using System.Text.Json;
using System.Text.Json.Serialization;

namespace LokPass.Core.Password.Repositories;

public class FilePasswordRepository(string filePath) : IPasswordRepository
{
    // TODO: die gesamte Datei verschlüsseln
    // TODO: Titel und Nutzername verschlüsseln
    // TODO: Additional security:
    /*
     * // 1. Temporäre Datei für atomare Schreibvorgänge
       var tempFile = _filePath + ".tmp";

       var json = JsonSerializer.Serialize(passwords, _jsonOptions);
       await File.WriteAllTextAsync(tempFile, json);

       // 2. Atomarer Move
       File.Move(tempFile, _filePath, true);

       // 3. Dateiberechtigungen setzen (nur für aktuellen Benutzer)
       if (OperatingSystem.IsWindows())
       {
           var fileInfo = new FileInfo(_filePath);
           var fileSecurity = fileInfo.GetAccessControl();
           // Zugriff nur für aktuellen Benutzer
       }
     */

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public async Task<IEnumerable<UserPassword>> GetAllPasswordsAsync()
    {
        if (!File.Exists(filePath))
            return [];

        var fileContent = await File.ReadAllTextAsync(filePath);
        if (string.IsNullOrEmpty(fileContent))
            return [];

        try
        {
            var passwords = JsonSerializer.Deserialize<List<UserPassword>>(fileContent, _jsonOptions);
            return passwords ?? Enumerable.Empty<UserPassword>();
        }
        catch (JsonException)
        {
            return [];
        }
    }

    public async Task<UserPassword?> GetPasswordByIdAsync(Guid id)
    {
        var passwords = await GetAllPasswordsAsync();
        return passwords.FirstOrDefault(p => p.Id == id);
    }

    public async Task AddPasswordAsync(UserPassword userPassword)
    {
        var passwords = (await GetAllPasswordsAsync()).ToList();
        passwords.Add(userPassword);
        await SavePasswordsAsync(passwords);
    }

    public async Task UpdatePasswordAsync(UserPassword userPassword)
    {
        var passwords = (await GetAllPasswordsAsync()).ToList();
        var index = passwords.FindIndex(p => p.Id == userPassword.Id);

        if (index >= 0)
        {
            passwords[index] = userPassword;
            await SavePasswordsAsync(passwords);
        }
    }

    public async Task DeletePasswordAsync(Guid id)
    {
        var passwords = (await GetAllPasswordsAsync()).ToList();
        passwords.RemoveAll(p => p.Id == id);

        await SavePasswordsAsync(passwords);
    }

    private async Task SavePasswordsAsync(IEnumerable<UserPassword> passwords)
    {
        var json = JsonSerializer.Serialize(passwords, _jsonOptions);

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) Directory.CreateDirectory(directory);

        await File.WriteAllTextAsync(filePath, json);
    }
}

// TODO: save methods! 
/*

private readonly string _filePath;
   private readonly byte[] _encryptionKey;

   public SecureFilePasswordRepository(string filePath, byte[] encryptionKey)
   {
       _filePath = filePath;
       _encryptionKey = encryptionKey;
   }

   private async Task SavePasswordsSecureAsync(IEnumerable<UserPassword> passwords)
   {
       var json = JsonSerializer.Serialize(passwords, _jsonOptions);
       var encryptedData = EncryptData(Encoding.UTF8.GetBytes(json));

       await File.WriteAllBytesAsync(_filePath, encryptedData);
   }

   private byte[] EncryptData(byte[] data)
   {
       // Implementierung mit AES-Verschlüsselung
       // Verwenden Sie System.Security.Cryptography
       throw new NotImplementedException("AES-Verschlüsselung implementieren");
   }

*/