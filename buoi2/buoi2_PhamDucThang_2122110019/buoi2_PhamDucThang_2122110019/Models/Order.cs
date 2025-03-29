namespace buoi2_PhamDucThang_2122110019.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public decimal Total { get; set; }

        // Quan hệ 1 - N với OrderDetail
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
