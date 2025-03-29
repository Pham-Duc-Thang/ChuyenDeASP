namespace buoi2_PhamDucThang_2122110019.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        // Quan hệ 1-N với Order
        public ICollection<Order> Orders { get; set; }
    }
}
