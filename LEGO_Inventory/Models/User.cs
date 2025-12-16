namespace LEGO_Inventory;

public class User
{
    public Guid UserId { get; set; }
    
    public string Email { get; set; }
    
    public string Password { get; set; }
    
    public Guid Salt { get; set; }
    
    public DateTime CreatedAt { get; set; }
}