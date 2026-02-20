// See https://aka.ms/new-console-template for more information
using System.CommandLine;
using static System.Console;

// ForegroundColor = ConsoleColor.DarkGreen;

WriteLine("Hello, World!");

var rootCommand = new RootCommand("xp")
{
    CommandController.SC_CREATE(),
    CommandController.SC_ADD(),
    CommandController.SC_LIST()
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