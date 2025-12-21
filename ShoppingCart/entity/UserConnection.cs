public class UserConnection
{
    public int Id { get; set; }
    public int User1Id { get; set; }
    public int User2Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual User User1 { get; set; } = null!;
    public virtual User User2 { get; set; } = null!;
}