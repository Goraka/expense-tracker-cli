public class Account : IEntity
{   
    public Guid ID {get;set;}
    public string AccountType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Balance { get; set; }

    // int IEntity.ID => throw new NotImplementedException();
}