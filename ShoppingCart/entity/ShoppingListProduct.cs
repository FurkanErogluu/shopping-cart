using ShoppingCart.Entities;

public class ShoppingListProduct
{
    public int ShoppingListId { get; set; }
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }

    public bool IsChecked { get; set; } = false;

    public virtual ShoppingList ShoppingList { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}