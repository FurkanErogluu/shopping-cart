using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);
            return Ok(ApiResponse<AuthResponse>.Ok(response, 201));
        }
        catch (BusinessException ex)
        {
            return ex.Code switch
            {
                "EMAIL_EXISTS" => Conflict(ApiResponse<AuthResponse>.Fail(ex.Code, ex.Message, 409)),
                _ => BadRequest(ApiResponse<AuthResponse>.Fail(ex.Code, ex.Message, 400))
            };
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(ApiResponse<AuthResponse>.Ok(response));
        }
        catch (BusinessException ex)
        {
            return Unauthorized(ApiResponse<AuthResponse>.Fail(ex.Code, ex.Message, 401));
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var response = await _authService.RefreshTokenAsync(request.RefreshToken);
            return Ok(ApiResponse<AuthResponse>.Ok(response));
        }
        catch (BusinessException ex)
        {
            return Unauthorized(ApiResponse<AuthResponse>.Fail(ex.Code, ex.Message, 401));
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetCurrentUser()
    {
        try
        {
            var userId = GetCurrentUserId();
            var user = await _authService.GetUserByIdAsync(userId);
            return Ok(ApiResponse<UserDto>.Ok(user));
        }
        catch (BusinessException ex)
        {
            return NotFound(ApiResponse<UserDto>.Fail(ex.Code, ex.Message, 404));
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim!);
    }
}
