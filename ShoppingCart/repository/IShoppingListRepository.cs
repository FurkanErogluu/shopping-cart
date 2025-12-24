public interface IShoppingListRepository
{
    Task<ShoppingList?> GetByIdAsync(int id);

    Task<ShoppingListMember?> GetMemberAsync(int shoppingListId, int userId);
    Task<ShoppingList> CreateAsync(ShoppingList shoppingList);
    Task UpdateAsync(ShoppingList shoppingList);
    Task DeleteAsync(ShoppingList shoppingList);
    Task SaveChangesAsync();

    Task<List<ShoppingList>> GetAllByUserIdAsync(int userId);

    Task AddMemberToListAsync(ShoppingListMember member);

    Task RemoveMemberFromListAsync(ShoppingListMember member);
}