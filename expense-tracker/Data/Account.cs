public class Account
{
    // public Account(string _accType, string _name, decimal _balance)
    // {
    //     AccountType = _accType;
    //     Name = _name;
    //     Balance = _balance;
    // }
    
    public Guid ID {get;set;}
    public string AccountType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}