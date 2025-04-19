using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace buoi2_PhamDucThang_2122110019.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryID { get; set; }

        [Required]
        public string CategoryName { get; set; }

        // Quan hệ với sản phẩm – khởi tạo danh sách rỗng để tránh lỗi khi không có dữ liệu
        public List<Product> Products { get; set; } = new List<Product>();

        // Thuộc tính dùng cho soft delete
        public bool IsDeleted { get; set; } = false;
    }
}
