using System.CommandLine;
using Microsoft.Extensions.Configuration;

public class CommandController
{
    private static Config _configurationBuilder;

    public CommandController(Config config)
    {
        _configurationBuilder = config;
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

            if(budget > 0)
            {
                MonthlyBudget monthlyBudget = new MonthlyBudget { Amount = budget };
                expenseTracker.ConfigAppSettings(monthlyBudget, _configurationBuilder).Wait();
                Console.WriteLine($"Creating a monthly budget of {budgetAmt}");
                return;
            }

            expenseTracker.Create(acc, category).Wait();
        });

        return createCommand;
    }

    public Command SC_ADD()
    {
        var addCommand = new Command("add", "Add an expense")
        {
            new Option<decimal>("--amount", ["--amt", "--n"])
            {
                Description = "Add expense amount to be tracked",
                Required = true
            },
            new Option<string>("--description", ["--desc"])
            {
                Description = "Add a description for the expense",
            }
        };

        addCommand.SetAction(context =>
        {
            decimal amount = context.GetValue((Option<decimal>)addCommand.Options[0]);
            string description = context.GetValue((Option<string>)addCommand.Options[1]) ?? string.Empty;

            Console.WriteLine($"The amount is {amount} and the description is {description}");
        });

        return addCommand;
    }

    public Command SC_LIST()
    {
        var listCommand = new Command("list", "List all expenses");

        listCommand.Aliases.Add("ls");

        listCommand.SetAction(context =>
        {
            Console.WriteLine("Listing all expenses...");
        });

        return listCommand;
    }

}