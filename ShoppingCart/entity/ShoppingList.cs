public class ShoppingList
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsCompleted { get; set; } = false;
    public virtual ICollection<ShoppingListMember> Members { get; set; } = new List<ShoppingListMember>();
    public virtual ICollection<ShoppingListProduct> Items { get; set; } = new List<ShoppingListProduct>();
}
