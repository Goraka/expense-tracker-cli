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

    public async Task<List<Account>> GetAccounts()
    {
        FileManagement _fileManagement = new FileManagement();

        return await _fileManagement.ReadFromFileAsync<Account>(_accountFileName) ?? new List<Account>();
    }

    private async Task<List<Category>> GetCategories()
    {
        FileManagement _fileManagement = new FileManagement();

        return await _fileManagement.ReadFromFileAsync<Category>(_categoryFileName) ?? new List<Category>();
    }

    private async Task<List<Expense>> GetExpenses()
    {
        FileManagement _filemanagement = new FileManagement();

        return await _filemanagement.ReadFromFileAsync<Expense>(_expenseFileName) ?? new List<Expense>();
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
            await _fileManagement.CreateFile(newAccount, _accountFileName);
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

        var categories = await _fileManagement.ReadFromFileAsync<Category>(_categoryFileName);
        // int id = 1;

        if (categories != null)
        {
            if (categories.Any(c => c.Name.Equals(category, StringComparison.OrdinalIgnoreCase)))
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine($"The category '{category}' already exists. Please provide a unique category name.");
                ForegroundColor = foregroundColor;
                return;
            }

            // id = categories[categories.Count - 1].Id + 1;
        }

        Category newCategory = new Category()
        {
            ID = Guid.NewGuid(),
            Name = category
        };

        await _fileManagement.CreateFile<Category>(newCategory, _categoryFileName);

        WriteLine($"Creating a new expense category named {category}");
    }

    public async Task<Expense> AddExpense(string description, decimal amount, string accountName)
    {
        FileManagement _fileManagement = new FileManagement();
        var accounts = await GetAccounts();
        // var _categories = await GetCategories();
        var _expenseLst = await GetExpenses();
        // int id = 0;

        // if(_expenseLst != null && _expenseLst.Count() > 0)
        // {
        //     id = _expenseLst.Last().Id;
        // }

        var _account = accounts.FirstOrDefault(a => a.Name == accountName) ?? new Account();

        if (amount > _account.Balance)
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine("Amount exceeds available balance");
            ForegroundColor = foregroundColor;
            return new Expense() { Account = new Account() };
        }

        var updatedAccount = _account;
        updatedAccount.Balance -= amount;

        Expense expense = new Expense
        {
            ID = Guid.NewGuid(),
            Description = description,
            Amount = amount,
            Account = updatedAccount,
            CreatedDate = DateTime.Today
        };

        await _fileManagement.CreateFile(expense, _expenseFileName);

        await _fileManagement.CreateFile(updatedAccount, _accountFileName);

        return expense;
    }
}