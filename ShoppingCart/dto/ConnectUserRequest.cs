using System.ComponentModel.DataAnnotations;

public class ConnectUserRequest
{
    [Required]
    [StringLength(8, MinimumLength = 8)]
    public string FollowId { get; set; } = string.Empty;
}
