namespace buoi2_PhamDucThang_2122110019.Models
{
    public class OrderDetail
    {
        public int OrderDetailID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        // Quan hệ với Order và Product
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
