using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eCommerce.Data;
using eCommerce.Models;

namespace eCommerce.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderItemsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public OrderItemsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/orderitems
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems()
    {
        return await _context.OrderItems
            .Include(oi => oi.Order)
            .ToListAsync();
    }

    // GET: api/orderitems/5
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderItem>> GetOrderItem(int id)
    {
        var orderItem = await _context.OrderItems
            .Include(oi => oi.Order)
            .FirstOrDefaultAsync(oi => oi.Id == id);

        if (orderItem == null)
        {
            return NotFound();
        }

        return orderItem;
    }

    // GET: api/orderitems/order/5
    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItemsByOrder(int orderId)
    {
        var orderItems = await _context.OrderItems
            .Where(oi => oi.OrderId == orderId)
            .ToListAsync();

        return orderItems;
    }

    // POST: api/orderitems
    [HttpPost]
    public async Task<ActionResult<OrderItem>> PostOrderItem(CreateOrderItemDto dto)
    {
        // Verify order exists
        var orderExists = await _context.Orders.AnyAsync(o => o.Id == dto.OrderId);
        if (!orderExists)
        {
            return BadRequest("Order not found");
        }

        var orderItem = new OrderItem
        {
            OrderId = dto.OrderId,
            ProductName = dto.ProductName,
            Quantity = dto.Quantity,
            UnitPrice = dto.UnitPrice
        };

        _context.OrderItems.Add(orderItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOrderItem), new { id = orderItem.Id }, orderItem);
    }

    // PATCH: api/orderitems/5
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchOrderItem(int id, UpdateOrderItemDto dto)
    {
        var orderItem = await _context.OrderItems.FindAsync(id);

        if (orderItem == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(dto.ProductName))
            orderItem.ProductName = dto.ProductName;

        if (dto.Quantity.HasValue)
            orderItem.Quantity = dto.Quantity.Value;

        if (dto.UnitPrice.HasValue)
            orderItem.UnitPrice = dto.UnitPrice.Value;

        _context.OrderItems.Update(orderItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/orderitems/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrderItem(int id)
    {
        var orderItem = await _context.OrderItems.FindAsync(id);

        if (orderItem == null)
        {
            return NotFound();
        }

        _context.OrderItems.Remove(orderItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public class CreateOrderItemDto
{
    public int OrderId { get; set; }
    public required string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class UpdateOrderItemDto
{
    public string? ProductName { get; set; }
    public int? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
}
