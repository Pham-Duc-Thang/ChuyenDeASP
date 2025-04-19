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
    public class OrderDetailsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public OrderDetailsController(AppDbContext context)
        {
            _context = context;
        }

        // --- Chức năng cũ (CRUD) ---

        // GET: api/orderdetails
        [HttpGet]
        public IActionResult GetOrderDetails()
        {
            var orderDetails = _context.OrderDetails
                                       .Include(od => od.Order)
                                       .Include(od => od.Product)
                                       .ToList();
            return Ok(orderDetails);
        }

        // GET: api/orderdetails/{id}
        [HttpGet("{id}")]
        public IActionResult GetOrderDetail(int id)
        {
            var detail = _context.OrderDetails
                                 .Include(od => od.Order)
                                 .Include(od => od.Product)
                                 .FirstOrDefault(od => od.OrderDetailID == id && !od.IsDeleted);
            if (detail == null)
                return NotFound();
            return Ok(detail);
        }

        // POST: api/orderdetails
        [HttpPost]
        public IActionResult CreateOrderDetail([FromBody] OrderDetail orderDetail)
        {
            orderDetail.IsDeleted = false;
            _context.OrderDetails.Add(orderDetail);
            _context.SaveChanges();
            return Ok(orderDetail);
        }

        // PUT: api/orderdetails/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateOrderDetail(int id, [FromBody] OrderDetail updatedDetail)
        {
            var detail = _context.OrderDetails.FirstOrDefault(od => od.OrderDetailID == id && !od.IsDeleted);
            if (detail == null)
                return NotFound();

            detail.OrderID = updatedDetail.OrderID;
            detail.ProductID = updatedDetail.ProductID;
            detail.Quantity = updatedDetail.Quantity;
            detail.UnitPrice = updatedDetail.UnitPrice;

            _context.OrderDetails.Update(detail);
            _context.SaveChanges();
            return Ok(detail);
        }

        // DELETE: api/orderdetails/{id} – Soft delete
        [HttpDelete("{id}")]
        public IActionResult DeleteOrderDetail(int id)
        {
            var detail = _context.OrderDetails.FirstOrDefault(od => od.OrderDetailID == id && !od.IsDeleted);
            if (detail == null)
                return NotFound();

            detail.IsDeleted = true;
            _context.OrderDetails.Update(detail);
            _context.SaveChanges();
            return Ok(new { Message = "OrderDetail soft-deleted successfully." });
        }

        // --- Chức năng xem và khôi phục ---

        // GET: api/orderdetails/deleted
        [HttpGet("deleted")]
        public IActionResult GetDeletedOrderDetails()
        {
            var deletedDetails = _context.OrderDetails
                                         .IgnoreQueryFilters()
                                         .Where(od => od.IsDeleted)
                                         .Include(od => od.Order)
                                         .Include(od => od.Product)
                                         .ToList();
            return Ok(deletedDetails);
        }

        // POST: api/orderdetails/restore/{id}
        [HttpPost("restore/{id}")]
        public IActionResult RestoreOrderDetail(int id)
        {
            var detail = _context.OrderDetails
                                 .IgnoreQueryFilters()
                                 .FirstOrDefault(od => od.OrderDetailID == id && od.IsDeleted);
            if (detail == null)
                return NotFound(new { Message = "OrderDetail not found or is not soft-deleted." });
            detail.IsDeleted = false;
            _context.OrderDetails.Update(detail);
            _context.SaveChanges();
            return Ok(new { Message = "OrderDetail restored successfully." });
        }
    }
}
