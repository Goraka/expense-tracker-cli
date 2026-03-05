// See https://aka.ms/new-console-template for more information
using System.CommandLine;
using Microsoft.Extensions.Configuration;
using static System.Console;


string _currentDirectory = Directory.GetCurrentDirectory();

var configuration = new ConfigurationBuilder()
    .SetBasePath(_currentDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

WriteLine("Hello, World!");

CommandController commandController = new CommandController(configuration);

var rootCommand = new RootCommand()
{
    commandController.SC_CREATE(),
    commandController.SC_ADD(),
    commandController.SC_LIST(),
    commandController.SC_DELETE(),
    commandController.SC_SUMMARY(),
    commandController.SC_EXPORT()
};

rootCommand.Description = "A simple cli based expense tracker";

rootCommand.Parse(args).Invoke();