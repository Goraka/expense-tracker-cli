public class Expense : IEntity
{
    public Guid ID { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public required Account Account { get; set; }
    public DateTime CreatedDate { get; set; }
}