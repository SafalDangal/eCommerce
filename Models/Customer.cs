namespace eCommerce.Models;

public class Customer
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation property
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
