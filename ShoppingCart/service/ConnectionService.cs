public class ConnectionService : IConnectionService
{
    private readonly IUserRepository _userRepository;
    private readonly IConnectionRepository _connectionRepository;

    public ConnectionService(
        IUserRepository userRepository,
        IConnectionRepository connectionRepository)
    {
        _userRepository = userRepository;
        _connectionRepository = connectionRepository;
    }

    public async Task<string> GetUserFollowIdAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new BusinessException("USER_NOT_FOUND", "User not found");
        }

        return user.FollowId;
    }

    public async Task<ConnectionDto> ConnectUsersAsync(int userId, string targetFollowId)
    {
        var targetUser = await _userRepository.GetByFollowIdAsync(targetFollowId);
        if (targetUser == null)
        {
            throw new BusinessException("USER_NOT_FOUND", "No user found with this FollowId");
        }

        if (targetUser.Id == userId)
        {
            throw new BusinessException("SELF_CONNECTION", "You cannot connect to yourself");
        }

        var existingConnection = await _connectionRepository
            .GetConnectionBetweenUsersAsync(userId, targetUser.Id);

        if (existingConnection != null)
        {
            throw new BusinessException("ALREADY_CONNECTED", "You are already connected with this user");
        }

        var connection = new UserConnection
        {
            User1Id = Math.Min(userId, targetUser.Id),
            User2Id = Math.Max(userId, targetUser.Id)
        };

        await _connectionRepository.CreateAsync(connection);

        return new ConnectionDto
        {
            ConnectionId = connection.Id,
            ConnectedUser = new UserDto
            {
                Id = targetUser.Id,
                Email = targetUser.Email,
                FollowId = targetUser.FollowId,
                CreatedAt = targetUser.CreatedAt
            },
            ConnectedAt = connection.CreatedAt
        };
    }

    public async Task<List<ConnectionDto>> GetUserConnectionsAsync(int userId)
    {
        var connections = await _connectionRepository.GetUserConnectionsAsync(userId);

        return connections.Select(c => new ConnectionDto
        {
            ConnectionId = c.Id,
            ConnectedUser = c.User1Id == userId
                ? new UserDto
                {
                    Id = c.User2.Id,
                    Email = c.User2.Email,
                    FollowId = c.User2.FollowId,
                    CreatedAt = c.User2.CreatedAt
                }
                : new UserDto
                {
                    Id = c.User1.Id,
                    Email = c.User1.Email,
                    FollowId = c.User1.FollowId,
                    CreatedAt = c.User1.CreatedAt
                },
            ConnectedAt = c.CreatedAt
        }).ToList();
    }

    public async Task DisconnectUsersAsync(int userId, int connectionId)
    {
        var connection = await _connectionRepository.GetByIdAsync(connectionId);

        if (connection == null || (connection.User1Id != userId && connection.User2Id != userId))
        {
            throw new BusinessException("CONNECTION_NOT_FOUND", "Connection not found");
        }

        await _connectionRepository.DeleteAsync(connection);
    }
}