public class Config
{
    public MonthlyBudget MonthlyBudget { get; set; }
    public ExpenseInfo ExpenseInfo { get; set; }
}

public class MonthlyBudget
{
    public decimal Amount { get; set; }
    public decimal Expenses { get; set; }
}

public class ExpenseInfo
{
    public Guid LastAddedExpenseID { get; set; }
}