using System;
using System.Collections.Generic;
public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string FollowId { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<ShoppingListMember> ShoppingListMemberships { get; set; } = new List<ShoppingListMember>();
    
    public virtual ICollection<UserConnection> ConnectionsInitiated { get; set; } = new List<UserConnection>();
    public virtual ICollection<UserConnection> ConnectionsReceived { get; set; } = new List<UserConnection>();
}