public interface IConnectionService
{
    Task<string> GetUserFollowIdAsync(int userId);
    Task<ConnectionDto> ConnectUsersAsync(int userId, string targetFollowId);
    Task<List<ConnectionDto>> GetUserConnectionsAsync(int userId);
    Task DisconnectUsersAsync(int userId, int connectionId);
}