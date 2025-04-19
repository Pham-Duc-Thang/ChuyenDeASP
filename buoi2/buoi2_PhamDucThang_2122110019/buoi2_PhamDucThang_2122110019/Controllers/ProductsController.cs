using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using buoi2_PhamDucThang_2122110019.Data;
using buoi2_PhamDucThang_2122110019.Models;

namespace buoi2_PhamDucThang_2122110019.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/products
        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = _context.Products.ToList();
            return Ok(products);
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductID == id && !p.IsDeleted);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        // POST: api/products
        [HttpPost]
        public IActionResult CreateProduct([FromBody] Product product)
        {
            product.IsDeleted = false;
            _context.Products.Add(product);
            _context.SaveChanges();
            return Ok(product);
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductID == id && !p.IsDeleted);
            if (product == null)
                return NotFound();

            product.ProductName = updatedProduct.ProductName;
            product.ProductImage = updatedProduct.ProductImage;
            product.ProductPrice = updatedProduct.ProductPrice;
            product.CategoryID = updatedProduct.CategoryID;

            _context.Products.Update(product);
            _context.SaveChanges();
            return Ok(product);
        }

        // DELETE: api/products/{id} – Soft delete
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductID == id && !p.IsDeleted);
            if (product == null)
                return NotFound();

            product.IsDeleted = true;
            _context.Products.Update(product);
            _context.SaveChanges();
            return Ok(new { Message = "Product soft-deleted successfully." });
        }

        // GET: api/products/deleted – Xem danh sách sản phẩm đã xoá mềm
        [HttpGet("deleted")]
        public IActionResult GetDeletedProducts()
        {
            var deleted = _context.Products
                                  .IgnoreQueryFilters()
                                  .Where(p => p.IsDeleted)
                                  .ToList();
            return Ok(deleted);
        }

        // POST: api/products/restore/{id} – Khôi phục sản phẩm đã xoá mềm
        [HttpPost("restore/{id}")]
        public IActionResult RestoreProduct(int id)
        {
            var product = _context.Products
                                  .IgnoreQueryFilters()
                                  .FirstOrDefault(p => p.ProductID == id && p.IsDeleted);
            if (product == null)
                return NotFound(new { Message = "Product not found or not soft-deleted." });

            product.IsDeleted = false;
            _context.Products.Update(product);
            _context.SaveChanges();
            return Ok(new { Message = "Product restored successfully." });
        }
    }
}
