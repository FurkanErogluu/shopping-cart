using ShoppingCart.Enums;

public interface IShoppingListService
{
    Task<ShoppingListDto> GetShoppingListByIdAsync(int id);
    Task<ShoppingListDto> CreateShoppingListAsync(ShoppingList shoppingList,int userId);
    Task UpdateShoppingListAsync(int id, string name, bool isCompleted);
    Task DeleteShoppingListAsync(int id);


    Task AddItemToShoppingListAsync(int shoppingListId, int productId, decimal quantity);

    Task RemoveItemFromShoppingListAsync(int shoppingListId, int productId);
    Task UpdateItemQuantityAsync(int shoppingListId, int productId, decimal quantity);

    Task<List<ShoppingListDto>> GetMyShoppingListsAsync(int userId);
}