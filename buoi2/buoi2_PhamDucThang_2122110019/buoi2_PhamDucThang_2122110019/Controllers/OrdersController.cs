using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using buoi2_PhamDucThang_2122110019.Data;
using buoi2_PhamDucThang_2122110019.Models;

namespace buoi2_PhamDucThang_2122110019.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // --- Chức năng cũ (CRUD) ---

        // GET: api/orders
        [HttpGet]
        public IActionResult GetOrders()
        {
            var orders = _context.Orders
                                 .Include(o => o.OrderDetails)
                                 .ToList();
            return Ok(orders);
        }

        // GET: api/orders/{id}
        [HttpGet("{id}")]
        public IActionResult GetOrder(int id)
        {
            var order = _context.Orders
                                .Include(o => o.OrderDetails)
                                .FirstOrDefault(o => o.OrderID == id && !o.IsDeleted);
            if (order == null)
                return NotFound();
            return Ok(order);
        }

        // POST: api/orders
        [HttpPost]
        public IActionResult CreateOrder([FromBody] Order order)
        {
            order.IsDeleted = false;
            if (order.OrderDate == default)
                order.OrderDate = DateTime.Now;
            _context.Orders.Add(order);
            _context.SaveChanges();
            return Ok(order);
        }

        // PUT: api/orders/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateOrder(int id, [FromBody] Order updatedOrder)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderID == id && !o.IsDeleted);
            if (order == null)
                return NotFound();

            order.OrderDate = updatedOrder.OrderDate;
            order.CustomerName = updatedOrder.CustomerName;
            order.Total = updatedOrder.Total;
            // Nếu cần cập nhật OrderDetails, xử lý thêm ở đây

            _context.Orders.Update(order);
            _context.SaveChanges();
            return Ok(order);
        }

        // DELETE: api/orders/{id} – Soft delete
        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderID == id && !o.IsDeleted);
            if (order == null)
                return NotFound();

            order.IsDeleted = true;
            _context.Orders.Update(order);
            _context.SaveChanges();
            return Ok(new { Message = "Order soft-deleted successfully." });
        }

        // --- Chức năng xem và khôi phục ---

        // GET: api/orders/deleted
        [HttpGet("deleted")]
        public IActionResult GetDeletedOrders()
        {
            var deletedOrders = _context.Orders
                                        .IgnoreQueryFilters()
                                        .Where(o => o.IsDeleted)
                                        .Include(o => o.OrderDetails)
                                        .ToList();
            return Ok(deletedOrders);
        }

        // POST: api/orders/restore/{id}
        [HttpPost("restore/{id}")]
        public IActionResult RestoreOrder(int id)
        {
            var order = _context.Orders
                                .IgnoreQueryFilters()
                                .FirstOrDefault(o => o.OrderID == id && o.IsDeleted);
            if (order == null)
                return NotFound(new { Message = "Order not found or is not soft-deleted." });
            order.IsDeleted = false;
            _context.Orders.Update(order);
            _context.SaveChanges();
            return Ok(new { Message = "Order restored successfully." });
        }
    }
}
