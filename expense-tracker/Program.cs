// See https://aka.ms/new-console-template for more information
using System.CommandLine;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using static System.Console;

// ForegroundColor = ConsoleColor.DarkGreen;

Config builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build()
    .Get<Config>() ?? new Config();

decimal currentBudget = builder?.MonthlyBudget.Amount ?? 0;

WriteLine("Hello, World!");

CommandController commandController = new CommandController(builder);

var rootCommand = new RootCommand("xp")
{
    commandController.SC_CREATE(),
    commandController.SC_ADD(),
    commandController.SC_LIST()
};

rootCommand.Description = "A simple cli based expense tracker";

// ParseResult parseResult = rootCommand.Parse(args);

// if (parseResult.Errors.Count == 0 && parseResult.GetValue(nameOption) is string name)
// {
//     printText(name);
// }

// var amtOption = new Option<decimal>("--amount")
//     {
//         Description = "The amount of the expense",
//     };

// var subCommand = new Command("add", "Add an expense")
// {
//     amtOption
// };

// rootCommand.Subcommands.Add(subCommand);

// subCommand.SetAction(context => printAmount(context.GetValue(amtOption)));

rootCommand.Parse(args).Invoke();

// void printText(string name)
// {
//     WriteLine($"Hello {name}");
// }

// void printAmount(decimal amount)
// {
//     WriteLine($"The amount is {amount}");
// }