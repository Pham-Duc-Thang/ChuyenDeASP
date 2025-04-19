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
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/categories
        [HttpGet]
        public IActionResult GetCategories()
        {
            var categories = _context.Categories.ToList();
            return Ok(categories);
        }

        // GET: api/categories/{id}
        [HttpGet("{id}")]
        public IActionResult GetCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryID == id && !c.IsDeleted);
            if (category == null)
                return NotFound();
            return Ok(category);
        }

        // POST: api/categories
        [HttpPost]
        public IActionResult CreateCategory([FromBody] Category category)
        {
            category.IsDeleted = false;
            _context.Categories.Add(category);
            _context.SaveChanges();
            return Ok(category);
        }

        // PUT: api/categories/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateCategory(int id, [FromBody] Category updatedCategory)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryID == id && !c.IsDeleted);
            if (category == null)
                return NotFound();

            category.CategoryName = updatedCategory.CategoryName;
            // Nếu có cập nhật thêm thuộc tính khác, xử lý ở đây.

            _context.Categories.Update(category);
            _context.SaveChanges();
            return Ok(category);
        }

        // DELETE: api/categories/{id} – Soft delete
        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryID == id && !c.IsDeleted);
            if (category == null)
                return NotFound();

            category.IsDeleted = true;
            _context.Categories.Update(category);
            _context.SaveChanges();
            return Ok(new { Message = "Category soft-deleted successfully." });
        }

        // GET: api/categories/deleted – Xem danh sách các category đã xoá mềm
        [HttpGet("deleted")]
        public IActionResult GetDeletedCategories()
        {
            var deleted = _context.Categories
                                  .IgnoreQueryFilters()
                                  .Where(c => c.IsDeleted)
                                  .ToList();
            return Ok(deleted);
        }

        // POST: api/categories/restore/{id} – Khôi phục category đã xoá mềm
        [HttpPost("restore/{id}")]
        public IActionResult RestoreCategory(int id)
        {
            var category = _context.Categories
                                   .IgnoreQueryFilters()
                                   .FirstOrDefault(c => c.CategoryID == id && c.IsDeleted);
            if (category == null)
                return NotFound(new { Message = "Category not found or not soft-deleted." });

            category.IsDeleted = false;
            _context.Categories.Update(category);
            _context.SaveChanges();
            return Ok(new { Message = "Category restored successfully." });
        }
    }
}
