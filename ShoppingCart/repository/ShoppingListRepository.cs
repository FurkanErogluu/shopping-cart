using Microsoft.EntityFrameworkCore;

public class ShoppingListRepository: IShoppingListRepository
{
    private readonly AppDbContext _context;

    public ShoppingListRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ShoppingList?> GetByIdAsync(int id)
    {
        return await _context.ShoppingLists
            .Include(list => list.Items)           
                .ThenInclude(item => item.Product) 
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<ShoppingListMember?> GetMemberAsync(int shoppingListId, int userId)
    {
        return await _context.ShoppingListMembers
            .FirstOrDefaultAsync(m => m.ShoppingListId == shoppingListId && m.UserId == userId);
    }

    

    public async Task<ShoppingList> CreateAsync(ShoppingList shoppingList)
    {
        _context.ShoppingLists.Add(shoppingList);
        await _context.SaveChangesAsync();
        return shoppingList;
    }

    public async Task UpdateAsync(ShoppingList shoppingList)
    {
        _context.ShoppingLists.Update(shoppingList);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ShoppingList shoppingList)
    {
        _context.ShoppingLists.Remove(shoppingList);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<ShoppingList>> GetAllByUserIdAsync(int userId)
    {
        return await _context.ShoppingLists

            .Include(x => x.Items)
                .ThenInclude(x => x.Product)
            

            .Include(x => x.Members) 



            .Where(list => list.Members.Any(member => member.UserId == userId)) 
            
            .ToListAsync();
    }

    public async Task AddMemberToListAsync(ShoppingListMember member)
    {
        _context.ShoppingListMembers.Add(member);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveMemberFromListAsync(ShoppingListMember member)
    {
        _context.ShoppingListMembers.Remove(member);
        await _context.SaveChangesAsync();
    }
}