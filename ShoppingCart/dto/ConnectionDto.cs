public class ConnectionDto
{
    public int ConnectionId { get; set; }
    public UserDto ConnectedUser { get; set; } = null!;
    public DateTime ConnectedAt { get; set; }
}