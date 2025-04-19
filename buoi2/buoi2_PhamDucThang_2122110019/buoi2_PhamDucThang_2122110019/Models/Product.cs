using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace buoi2_PhamDucThang_2122110019.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductID { get; set; }

        [Required]
        public string ProductName { get; set; }

        public string ProductImage { get; set; }
        public decimal ProductPrice { get; set; }

        // Quan hệ với Category
        public int CategoryID { get; set; }
        public Category Category { get; set; }

        // Thuộc tính dùng cho soft delete
        public bool IsDeleted { get; set; } = false;
    }
}
