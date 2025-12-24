using Microsoft.EntityFrameworkCore;

public class ConnectionRepository : IConnectionRepository
{
    private readonly AppDbContext _context;

    public ConnectionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserConnection?> GetByIdAsync(int id)
    {
        return await _context.UserConnections.FindAsync(id);
    }

    public async Task<UserConnection?> GetConnectionBetweenUsersAsync(int user1Id, int user2Id)
    {
        return await _context.UserConnections
            .FirstOrDefaultAsync(c =>
                (c.User1Id == user1Id && c.User2Id == user2Id) ||
                (c.User1Id == user2Id && c.User2Id == user1Id)
            );
    }

    public async Task<List<UserConnection>> GetUserConnectionsAsync(int userId)
    {
        return await _context.UserConnections
            .Where(c => c.User1Id == userId || c.User2Id == userId)
            .Include(c => c.User1)
            .Include(c => c.User2)
            .ToListAsync();
    }

    public async Task<bool> AreUsersConnectedAsync(int user1Id, int user2Id)
    {
        return await _context.UserConnections 
            .AnyAsync(c =>
                (c.User1Id == user1Id && c.User2Id == user2Id) ||
                (c.User1Id == user2Id && c.User2Id == user1Id)
            );
    }

    public async Task<UserConnection> CreateAsync(UserConnection connection)
    {
        _context.UserConnections.Add(connection);
        await _context.SaveChangesAsync();
        return connection;
    }

    public async Task DeleteAsync(UserConnection connection)
    {
        _context.UserConnections.Remove(connection);
        await _context.SaveChangesAsync();
    }
}