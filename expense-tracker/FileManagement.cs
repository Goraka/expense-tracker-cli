using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml;

public class FileManagement
{
    private readonly string _currentDirectory = Directory.GetCurrentDirectory();
    private readonly string _folderName = "ExpenseTrackerData";

    private readonly string _folderPath;

    public FileManagement()
    {
        _folderPath = Path.Combine(_currentDirectory, _folderName);
    }

    //     public void CreateAccountFile(Account account)
    //     {
    //         List<Account> accounts = new List<Account>();
    // ;
    //         if (!Directory.Exists(_folderPath))
    //         {
    //             Directory.CreateDirectory(_folderPath);
    //         }

    //         string accountFilePath = Path.Combine(_folderPath, _accountFileName);

    //         accounts = ReadFromFileAsync<List<Account>>(accountFilePath).Result;

    //         if(accounts == null)
    //         {
    //             accounts = new List<Account>();
    //         }

    //         accounts.Add(account);

    //         string accountJson = JsonSerializer.Serialize(accounts, new JsonSerializerOptions { WriteIndented = true });
    //         File.WriteAllText(accountFilePath, accountJson);

    //         // var lstAccounts = ReadFromFileAsync<List<Account>>(accountFilePath).Result;
    //         // Console.WriteLine($"Total accounts: {lstAccounts.Count}");

    //         // foreach(var a in lstAccounts)
    //         // {
    //         //     Console.WriteLine($"Account ID: {a.ID}, Name: {a.Name}, Type: {a.AccountType}, Balance: {a.Balance}");
    //         // }
    //     }

    public async Task CreateFile<T>(T model, string _fileName)
    {
        List<T> models = new List<T>();
        ;
        if (!Directory.Exists(_folderPath))
        {
            Directory.CreateDirectory(_folderPath);
        }

        string _filePath = Path.Combine(_folderPath, _fileName);
        try
        {
            models = ReadFromFileAsync<List<T>>(_filePath).Result;

            if (models == null)
            {
                models = new List<T>();
            }

            models.Add(model);
        
            string modelJson = JsonSerializer.Serialize(models, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, modelJson);
        }
        catch(Exception ex)
        {
            throw new Exception($"Failed to write to file '{_filePath}': {ex.Message}");
        }

        // var lstAccounts = ReadFromFileAsync<List<Account>>(accountFilePath).Result;
        // Console.WriteLine($"Total accounts: {lstAccounts.Count}");

        // foreach(var a in lstAccounts)
        // {
        //     Console.WriteLine($"Account ID: {a.ID}, Name: {a.Name}, Type: {a.AccountType}, Balance: {a.Balance}");
        // }
    }

    public async Task<T> ReadFromFileAsync<T>(string fileName)
    {
        string _filePath = Path.Combine(_folderPath, fileName);

        if (!File.Exists(_filePath))
        {
            return default;
            // throw new FileNotFoundException($"The file '{fileName}' was not found in the directory '{_folderPath}'.");
        }

        string fileContent = await File.ReadAllTextAsync(_filePath);

        if (string.IsNullOrEmpty(fileContent) || string.IsNullOrWhiteSpace(fileContent) || fileContent == "[]")
        {
            throw new InvalidDataException($"The file '{fileName}' is empty or contains invalid data.");
        }

        T? data = JsonSerializer.Deserialize<T>(fileContent);

        if (data == null)
        {
            throw new InvalidDataException($"Failed to deserialize the content of '{fileName}'. The data may be malformed or not in the expected format.");
        }

        return data;
    }
}