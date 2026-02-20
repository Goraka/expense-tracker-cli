using static System.Console;
public class ExpenseTracker
{
    private readonly ConsoleColor foregroundColor = ForegroundColor;

    public ExpenseTracker()
    {
        
    }

    public void Create(string account, decimal budgetAmt, string category)
    {
        FileManagement _fileManagement = new FileManagement();

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

            WriteLine("Enter the account type (e.g., Checking, Savings, Credit Card):");
            var accountType = ReadLine();

            if(string.IsNullOrEmpty(accountType))
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine("Account type cannot be empty. Please provide a valid account type.");
                ForegroundColor = foregroundColor;
                return;
            }

            WriteLine("Enter the initial balance for the account:");
            var balanceInput = ReadLine();

            if(decimal.TryParse(balanceInput, out decimal balance))
            {
                if(balance < 0)
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

                _fileManagement.CreateAccountFile(newAccount);
            }
            else
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine("Invalid input for initial balance. Please provide a valid decimal number.");
                ForegroundColor = foregroundColor;
                return;
            }

            WriteLine($"Account Type: {accountType}, Initial Balance: {balanceInput}");
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