using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using buoi2_PhamDucThang_2122110019.Data;
using buoi2_PhamDucThang_2122110019.Models;

namespace buoi2_PhamDucThang_2122110019.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CustomersController(AppDbContext context)
        {
            _context = context;
        }

        // --- Chức năng cũ (CRUD) ---

        // GET: api/customers
        [HttpGet]
        public IActionResult GetCustomers()
        {
            var customers = _context.Customers.ToList();
            return Ok(customers);
        }

        // GET: api/customers/{id}
        [HttpGet("{id}")]
        public IActionResult GetCustomer(int id)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerID == id && !c.IsDeleted);
            if (customer == null)
                return NotFound();
            return Ok(customer);
        }

        // POST: api/customers
        [HttpPost]
        public IActionResult CreateCustomer([FromBody] Customer customer)
        {
            customer.IsDeleted = false;
            _context.Customers.Add(customer);
            _context.SaveChanges();
            return Ok(customer);
        }

        // PUT: api/customers/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateCustomer(int id, [FromBody] Customer updatedCustomer)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerID == id && !c.IsDeleted);
            if (customer == null)
                return NotFound();

            customer.FullName = updatedCustomer.FullName;
            customer.Email = updatedCustomer.Email;
            customer.Phone = updatedCustomer.Phone;
            customer.Address = updatedCustomer.Address;

            _context.Customers.Update(customer);
            _context.SaveChanges();
            return Ok(customer);
        }

        // DELETE: api/customers/{id} – Thực hiện soft delete
        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerID == id && !c.IsDeleted);
            if (customer == null)
                return NotFound();

            customer.IsDeleted = true;
            _context.Customers.Update(customer);
            _context.SaveChanges();
            return Ok(new { Message = "Customer soft-deleted successfully." });
        }

        // --- Chức năng xem và khôi phục bản ghi đã soft delete ---

        // GET: api/customers/deleted
        [HttpGet("deleted")]
        public IActionResult GetDeletedCustomers()
        {
            var deletedCustomers = _context.Customers
                                           .IgnoreQueryFilters()
                                           .Where(c => c.IsDeleted)
                                           .ToList();
            return Ok(deletedCustomers);
        }

        // POST: api/customers/restore/{id}
        [HttpPost("restore/{id}")]
        public IActionResult RestoreCustomer(int id)
        {
            var customer = _context.Customers
                                   .IgnoreQueryFilters()
                                   .FirstOrDefault(c => c.CustomerID == id && c.IsDeleted);
            if (customer == null)
                return NotFound(new { Message = "Customer not found or is not soft-deleted." });
            customer.IsDeleted = false;
            _context.Customers.Update(customer);
            _context.SaveChanges();
            return Ok(new { Message = "Customer restored successfully." });
        }
    }
}
