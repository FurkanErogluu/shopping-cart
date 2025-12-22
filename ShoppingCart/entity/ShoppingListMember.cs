public class ShoppingListMember
{
    public int UserId { get; set; }
    public int ShoppingListId { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public virtual User User { get; set; } = null!;
    public virtual ShoppingList ShoppingList { get; set; } = null!;
}