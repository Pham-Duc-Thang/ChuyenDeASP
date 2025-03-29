namespace buoi2_PhamDucThang_2122110019.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal ProductPrice { get; set; }

        // Khóa ngoại: liên kết với Category (nếu có)
        public int? CategoryID { get; set; }
        public Category Category { get; set; }
    }
}
