using System.Text.Json;
using static System.Console;
public class ExpenseTracker
{
    private readonly ConsoleColor foregroundColor = ForegroundColor;
    private readonly string _accountFileName = "accounts.json";
    private readonly string _categoryFileName = "categories.json";
    private readonly string _expenseFileName = $"expenses.json_{DateTime.Now:yyyy-MM-dd}";

    public ExpenseTracker()
    {

    }

    public async Task CreateAccounts(string account)
    {
        FileManagement _fileManagement = new FileManagement();

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

        WriteLine($"New account created for {account} with an initial balance of {balanceInput}/-");
    }

    public async Task CreateCategories(string category)
    {
        FileManagement _fileManagement = new FileManagement();

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

        WriteLine($"Creating a new expense category named {category}");
    }
}