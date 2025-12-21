public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly JwtService _jwtService;

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        JwtService jwtService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            throw new BusinessException("EMAIL_EXISTS", "Email already registered");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Email = request.Email,
            PasswordHash = passwordHash
        };

        await _userRepository.CreateAsync(user);

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new BusinessException("INVALID_CREDENTIALS", "Invalid email or password");
        }

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

        if (token == null || token.ExpiryDate < DateTime.UtcNow)
        {
            throw new BusinessException("INVALID_REFRESH_TOKEN", "Invalid or expired refresh token");
        }

        var user = await _userRepository.GetByIdAsync(token.UserId);
        if (user == null)
        {
            throw new BusinessException("USER_NOT_FOUND", "User not found");
        }

        await _refreshTokenRepository.RevokeAsync(token);

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<UserDto> GetUserByIdAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new BusinessException("USER_NOT_FOUND", "User not found");
        }

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FollowId = user.FollowId,
            CreatedAt = user.CreatedAt
        };
    }

    private async Task<AuthResponse> GenerateAuthResponseAsync(User user)
    {
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();
        var refreshTokenExpiry = _jwtService.GetRefreshTokenExpiry();

        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiryDate = refreshTokenExpiry
        };

        await _refreshTokenRepository.CreateAsync(refreshTokenEntity);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiry = _jwtService.GetAccessTokenExpiry(),
            RefreshTokenExpiry = refreshTokenExpiry,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FollowId = user.FollowId,
                CreatedAt = user.CreatedAt
            }
        };
    }
}
