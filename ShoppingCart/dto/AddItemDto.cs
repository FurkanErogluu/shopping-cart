namespace ShoppingCart.dto
{
    public class AddItemDto
    {
        public int ShoppingListId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
    }
}
