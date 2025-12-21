using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConnectionController : ControllerBase
{
    private readonly IConnectionService _connectionService;

    public ConnectionController(IConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    [HttpGet("my-follow-id")]
    public async Task<ActionResult<ApiResponse<string>>> GetMyFollowId()
    {
        try
        {
            var userId = GetCurrentUserId();
            var followId = await _connectionService.GetUserFollowIdAsync(userId);
            return Ok(ApiResponse<string>.Ok(followId));
        }
        catch (BusinessException ex)
        {
            return NotFound(ApiResponse<string>.Fail(ex.Code, ex.Message, 404));
        }
    }

    [HttpPost("connect")]
    public async Task<ActionResult<ApiResponse<ConnectionDto>>> ConnectUser([FromBody] ConnectUserRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var connection = await _connectionService.ConnectUsersAsync(userId, request.FollowId);
            return Ok(ApiResponse<ConnectionDto>.Ok(connection, 201));
        }
        catch (BusinessException ex)
        {
            return ex.Code switch
            {
                "USER_NOT_FOUND" => NotFound(ApiResponse<ConnectionDto>.Fail(ex.Code, ex.Message, 404)),
                "SELF_CONNECTION" => BadRequest(ApiResponse<ConnectionDto>.Fail(ex.Code, ex.Message, 400)),
                "ALREADY_CONNECTED" => Conflict(ApiResponse<ConnectionDto>.Fail(ex.Code, ex.Message, 409)),
                _ => BadRequest(ApiResponse<ConnectionDto>.Fail(ex.Code, ex.Message, 400))
            };
        }
    }

    [HttpGet("my-connections")]
    public async Task<ActionResult<ApiResponse<List<ConnectionDto>>>> GetMyConnections()
    {
        var userId = GetCurrentUserId();
        var connections = await _connectionService.GetUserConnectionsAsync(userId);
        return Ok(ApiResponse<List<ConnectionDto>>.Ok(connections));
    }

    [HttpDelete("disconnect/{connectionId}")]
    public async Task<ActionResult<ApiResponse<object>>> DisconnectUser(int connectionId)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _connectionService.DisconnectUsersAsync(userId, connectionId);
            return Ok(ApiResponse<object>.Ok(new { message = "Connection removed successfully" }));
        }
        catch (BusinessException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Code, ex.Message, 404));
        }
    }


    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim!);
    }
}