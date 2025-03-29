namespace buoi2_PhamDucThang_2122110019.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }

        // Quan hệ 1 - N với Product
        public ICollection<Product> Products { get; set; }
    }
}
