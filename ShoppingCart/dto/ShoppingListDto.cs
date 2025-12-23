public class ShoppingListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public bool IsCompleted { get; set; }
    public List<ShoppingListProductDto> Items { get; set; }
}