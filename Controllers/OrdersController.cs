using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eCommerce.Data;
using eCommerce.Models;

namespace eCommerce.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/orders
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .ToListAsync();
    }

    // GET: api/orders/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrder(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        return order;
    }

    // GET: api/orders/customer/5
    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByCustomer(int customerId)
    {
        var orders = await _context.Orders
            .Where(o => o.CustomerId == customerId)
            .Include(o => o.OrderItems)
            .ToListAsync();

        return orders;
    }

    // POST: api/orders
    [HttpPost]
    public async Task<ActionResult<Order>> PostOrder(CreateOrderDto dto)
    {
        // Verify customer exists
        var customerExists = await _context.Customers.AnyAsync(c => c.Id == dto.CustomerId);
        if (!customerExists)
        {
            return BadRequest("Customer not found");
        }

        var order = new Order
        {
            CustomerId = dto.CustomerId,
            OrderDate = DateTime.UtcNow,
            TotalAmount = dto.TotalAmount,
            Status = dto.Status
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }

    // PATCH: api/orders/5
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchOrder(int id, UpdateOrderDto dto)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        if (dto.TotalAmount.HasValue)
            order.TotalAmount = dto.TotalAmount.Value;

        if (!string.IsNullOrEmpty(dto.Status))
            order.Status = dto.Status;

        _context.Orders.Update(order);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/orders/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public class CreateOrderDto
{
    public int CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public required string Status { get; set; }
}

public class UpdateOrderDto
{
    public decimal? TotalAmount { get; set; }
    public string? Status { get; set; }
}
