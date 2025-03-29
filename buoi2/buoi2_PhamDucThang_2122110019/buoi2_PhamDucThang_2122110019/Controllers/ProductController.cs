using Microsoft.AspNetCore.Mvc;

namespace buoi2_PhamDucThang_2122110019.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllProducts()
        {
            // Ví dụ trả về danh sách sản phẩm tĩnh
            return Ok(new[] { "Product A", "Product B" });
        }
    }
}
