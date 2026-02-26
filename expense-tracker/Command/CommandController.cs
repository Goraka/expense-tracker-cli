using System.CommandLine;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using static System.Console;

public class CommandController
{
    private static Config? _configurationBuilder;
    private readonly ConsoleColor foregroundColor = ForegroundColor;
    private readonly decimal _currentBudget = 0;

    public CommandController(Config config)
    {
        _configurationBuilder = config;
        _currentBudget = _configurationBuilder.MonthlyBudget.Amount;
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
                MonthlyBudget monthlyBudget = new MonthlyBudget { Amount = budget };
                ConfigSettings.SetValue(monthlyBudget, _configurationBuilder);
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

            if (res != null)
            {
                WriteLine($"New expense added for {res.Account.Name}, Account balance is {res.Account.Balance}");
            }
        });

        return addCommand;
    }

    public Command SC_LIST()
    {
        var listCommand = new Command("list", "List all expenses");

        listCommand.Aliases.Add("ls");

        listCommand.SetAction(context =>
        {
            WriteLine("Listing all expenses...");
        });

        return listCommand;
    }
}