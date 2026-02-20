using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml;

public class FileManagement
{
    private readonly string _currentDirectory = Directory.GetCurrentDirectory();
    private readonly string _folderName = "ExpenseTrackerData";
    private readonly string _accountFileName = "accounts.json";
    private readonly string _budgetFileName = "budgets.json";
    private readonly string _categoryFileName = "categories.json";
    private readonly string _expenseFileName = $"expenses.json_{DateTime.Now:yyyy-MM-dd}";
    private readonly string _folderPath;

    public FileManagement()
    {
        _folderPath = Path.Combine(_currentDirectory, _folderName);
    }

    public void CreateAccountFile(Account account)
    {
        List<Account> accounts = new List<Account>();
;
        if (!Directory.Exists(_folderPath))
        {
            Directory.CreateDirectory(_folderPath);
        }

        string accountFilePath = Path.Combine(_folderPath, _accountFileName);

        accounts = ReadFromFileAsync<List<Account>>(accountFilePath).Result;

        if(accounts == null)
        {
            accounts = new List<Account>();
        }

        accounts.Add(account);

        string accountJson = JsonSerializer.Serialize(accounts, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(accountFilePath, accountJson);

        // var lstAccounts = ReadFromFileAsync<List<Account>>(accountFilePath).Result;
        // Console.WriteLine($"Total accounts: {lstAccounts.Count}");

        // foreach(var a in lstAccounts)
        // {
        //     Console.WriteLine($"Account ID: {a.ID}, Name: {a.Name}, Type: {a.AccountType}, Balance: {a.Balance}");
        // }
    }

    public async Task<T> ReadFromFileAsync<T>(string _filePath)
    {
        // string _filePath = Path.Combine(_folderPath, fileName);

        if (!File.Exists(_filePath))
        {
            throw new FileNotFoundException($"The file '{_filePath}' was not found in the directory '{_folderPath}'.");
        }
        
        string fileContent = await File.ReadAllTextAsync(_filePath);

        if(string.IsNullOrEmpty(fileContent) || string.IsNullOrWhiteSpace(fileContent) || fileContent == "[]")
        {
            throw new InvalidDataException($"The file '{_filePath}' is empty or contains invalid data.");
        }

        T? data = JsonSerializer.Deserialize<T>(fileContent);

        if(data == null)
        {
            throw new InvalidDataException($"Failed to deserialize the content of '{_filePath}'. The data may be malformed or not in the expected format.");
        }

        return data;
    }
}