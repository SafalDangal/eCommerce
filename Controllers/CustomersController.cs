using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eCommerce.Data;
using eCommerce.Models;

namespace eCommerce.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CustomersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/customers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
    {
        return await _context.Customers.ToListAsync();
    }

    // GET: api/customers/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> GetCustomer(int id)
    {
        var customer = await _context.Customers
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer == null)
        {
            return NotFound();
        }

        return customer;
    }

    // POST: api/customers
    [HttpPost]
    public async Task<ActionResult<Customer>> PostCustomer(CreateCustomerDto dto)
    {
        var customer = new Customer
        {
            FullName = dto.FullName,
            Email = dto.Email,
            CreatedAt = DateTime.UtcNow
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
    }

    // PATCH: api/customers/5
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchCustomer(int id, UpdateCustomerDto dto)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(dto.FullName))
            customer.FullName = dto.FullName;

        if (!string.IsNullOrEmpty(dto.Email))
            customer.Email = dto.Email;

        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/customers/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer == null)
        {
            return NotFound();
        }

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public class CreateCustomerDto
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
}

public class UpdateCustomerDto
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
}

