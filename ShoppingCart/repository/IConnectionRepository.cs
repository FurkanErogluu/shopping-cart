public interface IConnectionRepository
{
    Task<UserConnection?> GetByIdAsync(int id);
    Task<UserConnection?> GetConnectionBetweenUsersAsync(int user1Id, int user2Id);
    Task<List<UserConnection>> GetUserConnectionsAsync(int userId);
    Task<bool> AreUsersConnectedAsync(int user1Id, int user2Id);
    Task<UserConnection> CreateAsync(UserConnection connection);
    Task DeleteAsync(UserConnection connection);
}