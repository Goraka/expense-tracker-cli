using System.Text.Json;
using static System.Console;
public class ExpenseTracker
{
    private readonly ConsoleColor foregroundColor = ForegroundColor;
    private readonly string _accountFileName = "accounts.json";
    private readonly string _budgetFileName = "budgets.json";
    private readonly string _categoryFileName = "categories.json";
    private readonly string _expenseFileName = $"expenses.json_{DateTime.Now:yyyy-MM-dd}";

    public ExpenseTracker()
    {

    }

    public async Task ConfigAppSettings<T>(T data, Config config)
    {
        var configItem = config.GetType().GetProperties().FirstOrDefault(p => p.PropertyType == typeof(T));

        if (configItem != null)
        {
            configItem.SetValue(config, data);
        }
        else
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine($"No matching configuration item found for type {typeof(T).Name}");
            ForegroundColor = foregroundColor;
            return;
        }

        var appSettingsJson = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true});
        File.WriteAllText("appsettings.json", appSettingsJson);
    }

    public async Task Create(string account, string category)
    {
        FileManagement _fileManagement = new FileManagement();

        if (string.IsNullOrEmpty(account) && string.IsNullOrEmpty(category))
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine("No options provided. Please provide an option to create an account, budget or category.");
            ForegroundColor = foregroundColor;
            return;
        }

        if (!string.IsNullOrEmpty(account) && string.IsNullOrEmpty(category))
        {
            WriteLine($"Creating account with name {account}");

            WriteLine("Enter the account type (e.g., Checking, Savings, Credit Card):");
            var accountType = ReadLine();

            if (string.IsNullOrEmpty(accountType))
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine("Account type cannot be empty. Please provide a valid account type.");
                ForegroundColor = foregroundColor;
                return;
            }

            WriteLine("Enter the initial balance for the account:");
            var balanceInput = ReadLine();

            if (decimal.TryParse(balanceInput, out decimal balance))
            {
                if (balance < 0)
                {
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine("Initial balance cannot be negative. Please provide a valid initial balance.");
                    ForegroundColor = foregroundColor;
                    return;
                }

                Account newAccount = new Account()
                {
                    ID = Guid.NewGuid(),
                    Name = account,
                    AccountType = accountType,
                    Balance = balance
                };

                // _fileManagement.CreateAccountFile(newAccount);
                await _fileManagement.CreateFile<Account>(newAccount, _accountFileName);
            }
            else
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine("Invalid input for initial balance. Please provide a valid decimal number.");
                ForegroundColor = foregroundColor;
                return;
            }

            WriteLine($"Account Type: {accountType}, Initial Balance: {balanceInput}");
        }
        else if (string.IsNullOrEmpty(account) && !string.IsNullOrEmpty(category))
        {
            var categories = await _fileManagement.ReadFromFileAsync<List<Category>>(_categoryFileName);
            int id = 1;

            if (categories != null)
            {
                if (categories.Any(c => c.Name.Equals(category, StringComparison.OrdinalIgnoreCase)))
                {
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine($"The category '{category}' already exists. Please provide a unique category name.");
                    ForegroundColor = foregroundColor;
                    return;
                }

                id = categories[categories.Count - 1].Id + 1;
            }

            Category newCategory = new Category()
            {
                Name = category
            };

            await _fileManagement.CreateFile<Category>(newCategory, _categoryFileName);

            WriteLine($"Creating a new expense category with name {category}");
        }
        else
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine("Invalid command. Please provide only one option at a time.");
            ForegroundColor = foregroundColor;
            return;
        }
    }
}