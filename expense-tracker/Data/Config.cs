public class Config
{
    public MonthlyBudget MonthlyBudget { get; set; }
}

public class MonthlyBudget
{
    public decimal Amount { get; set; }
    public decimal Expenses { get; set; }
}