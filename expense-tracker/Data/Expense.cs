public class Expense : IEntity
{
    public Guid ID { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public required Account Account { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class ExpenseDTO
{
    public int UserIdentifier { get; set; }
    public Guid ID { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public Guid AccountID { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class ExpenseIdentifier
{
    public Guid ID { get; set; }
    public string FileName { get; set; } = string.Empty;
}