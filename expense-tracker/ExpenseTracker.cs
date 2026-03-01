using System.Text.Json;
using static System.Console;
public class ExpenseTracker
{
    private readonly ConsoleColor foregroundColor = ForegroundColor;
    private readonly string _accountFileName = "accounts.json";
    private readonly string _categoryFileName = "categories.json";
    private readonly string _expenseFileName = $"expenses_{DateTime.Now:yyyy-MM-dd}.json";

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
        // var _expenseLst = await GetExpenses();

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

    public async Task<List<ExpenseDTO>> GetExpensesList()
    {
        FileManagement _fileManagement = new FileManagement();

        var expenses = await _fileManagement.ReadFilesAsync<Expense>("expenses_*.json");

        List<ExpenseDTO> expenseDTOs = expenses.Select((e, index) => new ExpenseDTO
        {
            UserIdentifier = index + 1,
            ID = e.ID,
            Description = e.Description,
            Amount = e.Amount,
            AccountID = e.Account.ID,
            CreatedDate = e.CreatedDate
        }).ToList();

        return expenseDTOs;
    }

    public async Task<bool> DeleteExpense(Guid expenseId, string createdAt)
    {
        FileManagement _fileManagement = new FileManagement();

        var _delExpenseFileName = $"expenses_{createdAt}";
        
        var _expenses = await _fileManagement.ReadFromFileAsync<Expense>(_delExpenseFileName);

        var _expense = _expenses.Find(x=>x.ID == expenseId);

        if(_expense != null)
        {
            var removeExpense = _expenses.Remove(_expense);
            var res = await _fileManagement.CreateFile(_expenses, _delExpenseFileName);

            if (res)
            {
                var _acc = _expense.Account;
                _acc.Balance += _expense.Amount;

                await _fileManagement.CreateFile(_acc, _accountFileName);
            }

            return res;
        }

        return false;
    }
}