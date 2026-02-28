using System.CommandLine;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using static System.Console;

public class CommandController
{
    private static Config? _configurationBuilder;
    private readonly ConsoleColor foregroundColor = ForegroundColor;
    private readonly decimal _currentBudget = 0;
    private readonly decimal _currentExpenses = 0;
    // private readonly string _appDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    private static readonly string _currentDirectory = Directory.GetCurrentDirectory();
    private readonly IConfigurationRoot _configuration;

    public CommandController(IConfigurationRoot configuration)
    {
        _configuration = configuration;
        _configurationBuilder = _configuration.Get<Config>();
        var monthlyBudgetSection = _configuration.GetSection("MonthlyBudget").Get<MonthlyBudget>();
        _currentExpenses = monthlyBudgetSection?.Expenses ?? 0;

        if(monthlyBudgetSection?.Expenses > 0)
        {
            _currentBudget = monthlyBudgetSection.Amount - monthlyBudgetSection.Expenses;
        }
        else
        {
            _currentBudget = monthlyBudgetSection?.Amount ?? 0;
        }
    }

    public Command SC_CREATE()
    {
        Option<string> account = new Option<string>("--account", ["--acc"])
        {
            Description = "Create a new account"
        };
        Option<decimal> budgetAmt = new Option<decimal>("--budget", ["--b"])
        {
            Description = "Create a monthly budget",
            DefaultValueFactory = parseResult => 0
        };
        Option<string> _category = new Option<string>("--category", ["--cat", "--c"])
        {
            Description = "Create a new Expense Category"
        };

        var createCommand = new Command("create", "Create a new account, budget or category")
        {
            account,
            budgetAmt,
            _category
        };

        createCommand.SetAction(context =>
        {
            ExpenseTracker expenseTracker = new ExpenseTracker();

            string acc = context.GetValue(account) ?? string.Empty;
            decimal budget = context.GetValue(budgetAmt);
            string category = context.GetValue(_category) ?? string.Empty;

            if (string.IsNullOrEmpty(acc) && string.IsNullOrEmpty(category) && budget <= 0)
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine("No options provided. Please provide an option to create an account, budget or category.");
                ForegroundColor = foregroundColor;
                return;
            }
            else if (!string.IsNullOrEmpty(acc) && !string.IsNullOrEmpty(category) && budget > 0)
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine("Invalid command. Please provide only one option at a time.");
                ForegroundColor = foregroundColor;
                return;
            }

            if (budget > 0)
            {
                MonthlyBudget monthlyBudget = new MonthlyBudget { Amount = budget, Expenses = 0 };
                if (_configurationBuilder != null)
                {
                    ConfigSettings.SetValue(monthlyBudget, _configurationBuilder, _currentDirectory);
                }
                WriteLine($"Creating a monthly budget of {budgetAmt}");
                return;
            }

            if (!string.IsNullOrEmpty(acc))
            {
                expenseTracker.CreateAccounts(acc).Wait();
            }

            if (!string.IsNullOrEmpty(category))
            {
                expenseTracker.CreateCategories(category).Wait();
            }

        });

        return createCommand;
    }

    public Command SC_ADD()
    {
        ExpenseTracker expenseTracker = new ExpenseTracker();
        var accountInfo = Task.Run(() => expenseTracker.GetAccounts()).Result;
        string[] _accountOptionNames = accountInfo.Select(a => a.Name).ToArray();

        var description = new Option<string>("--description", ["--desc"])
        {
            Description = "Add a description for the expense",
            Required = true
        };

        var amount = new Option<decimal>("--amount", ["--amt"])
        {
            Description = "Add expense amount to be tracked",
            Required = true,
            DefaultValueFactory = _ => 0
        };

        amount.Validators.Add(result =>
        {
            if (result.GetValue(amount) <= 0)
            {
                result.AddError("Amount must be greater than 0");
            }
        });

        var account = new Option<string>("--acc")
        {
            Description = "Select account",
            Required = true
        }.AcceptOnlyFromAmong(_accountOptionNames);

        var addCommand = new Command("add", "Add an expense")
        {
            account,
            description,
            amount
        };

        addCommand.SetAction(async context =>
        {
            decimal amt = context.GetValue(amount);
            string desc = context.GetValue(description) ?? string.Empty;
            string _acc = context.GetValue(account) ?? string.Empty;

            if (amt > _currentBudget)
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine("Amount exceeds current budget. Continue? (Y/n) : ");
                ForegroundColor = foregroundColor;
                var _continueOverBudget = ReadKey();

                if (_continueOverBudget.Key == ConsoleKey.N) return;
            }

            if (amt > accountInfo.FirstOrDefault(a => a.Name == _acc)?.Balance)
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine("Amount exceeds available balance.");
                ForegroundColor = foregroundColor;
                return;
            }

            var res = await expenseTracker.AddExpense(desc, amt, _acc);

            if (res != null && _configurationBuilder?.MonthlyBudget != null)
            {
                MonthlyBudget budget = new MonthlyBudget
                {
                    Amount = _configurationBuilder.MonthlyBudget.Amount,
                    Expenses = amt + _configurationBuilder.MonthlyBudget.Expenses
                };

                ExpenseInfo expenseInfo = new ExpenseInfo { LastAddedExpenseID = res.ID };

                ConfigSettings.SetValue<MonthlyBudget>(budget, _configurationBuilder, _currentDirectory);
                ConfigSettings.SetValue(expenseInfo, _configurationBuilder, _currentDirectory);

                WriteLine($"New expense added for {res.Account.Name}, Account balance is {res.Account.Balance}");
            }
        });

        return addCommand;
    }

    public Command SC_LIST()
    {
        ExpenseTracker expenseTracker = new ExpenseTracker();
        var _accounts = expenseTracker.GetAccounts().Result;
        var listCommand = new Command("list", "List all expenses");

        listCommand.Aliases.Add("ls");

        listCommand.SetAction(async context =>
        {
            WriteLine("Listing all expenses...");
            var expenses = await expenseTracker.GetExpensesList();
            WriteLine($"# {"ID",-5} {"Description",-20} {"Amount",-10} {"Account",-15} {"CreatedDate",-20}");
            foreach (var expense in expenses)
            {
                WriteLine($"# {expense.UserIdentifier, -5} {expense.Description, -20} {expense.Amount, -15} {_accounts.FirstOrDefault(a=>a.ID == expense.AccountID)?.Name, -20} {expense.CreatedDate, -20:d}");
                // WriteLine($"Expense: {expense.Description}, Amount: {expense.Amount}, Account: {_accounts.FirstOrDefault(a=>a.ID == expense.AccountID)?.Name}");
            }
        });

        return listCommand;
    }
}