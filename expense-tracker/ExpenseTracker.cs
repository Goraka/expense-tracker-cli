using static System.Console;
public class ExpenseTracker
{
    private readonly ConsoleColor foregroundColor = ForegroundColor;
    public ExpenseTracker()
    {
        
    }

    public void Create(string account, decimal budgetAmt, string category)
    {
        if(string.IsNullOrEmpty(account) && budgetAmt == 0 && string.IsNullOrEmpty(category))
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine("No options provided. Please provide an option to create an account, budget or category.");
            ForegroundColor = foregroundColor;
            return;
        }

        if(!string.IsNullOrEmpty(account) && budgetAmt == 0 && string.IsNullOrEmpty(category))
        {
            WriteLine($"Creating account with name {account}");
        }
        else if(string.IsNullOrEmpty(account) && budgetAmt > 0 && string.IsNullOrEmpty(category))
        {
            WriteLine($"Creating a monthly budget of {budgetAmt}");
        }
        else if(string.IsNullOrEmpty(account) && budgetAmt == 0 && !string.IsNullOrEmpty(category))
        {
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