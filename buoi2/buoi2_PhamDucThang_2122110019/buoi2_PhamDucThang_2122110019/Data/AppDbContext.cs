using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using buoi2_PhamDucThang_2122110019.Models;

namespace buoi2_PhamDucThang_2122110019.Data
{
    // Lớp AppDbContext kế thừa từ IdentityDbContext<ApplicationUser> để tích hợp ASP.NET Identity
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Định nghĩa các DbSet cho các entity
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Đảm bảo rằng các bảng của Identity được cấu hình trước
            base.OnModelCreating(modelBuilder);

            // Global Query Filters cho soft delete:
            // Mọi truy vấn đến Category và Product sẽ tự động chỉ lấy các bản ghi chưa bị xóa.
            modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
            // Nếu bạn đã thêm thuộc tính IsDeleted cho Customer, Order, OrderDetail, có thể thêm:
            // modelBuilder.Entity<Customer>().HasQueryFilter(cu => !cu.IsDeleted);
            // modelBuilder.Entity<Order>().HasQueryFilter(o => !o.IsDeleted);
            // modelBuilder.Entity<OrderDetail>().HasQueryFilter(od => !od.IsDeleted);

            // Cấu hình mối quan hệ giữa Product và Category:
            // Một Product có thuộc tính CategoryID (FK) có thể rỗng, và nếu Category bị xóa thì FK sẽ được đặt null.
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryID)
                .OnDelete(DeleteBehavior.SetNull);

            // Cấu hình mối quan hệ giữa OrderDetail và Order:
            // Một Order có nhiều OrderDetail, xóa Order sẽ xóa luôn các OrderDetail liên quan.
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderID)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình mối quan hệ giữa OrderDetail và Product:
            // Một OrderDetail liên kết với một Product, xóa Product sẽ xóa luôn các OrderDetail.
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany()  // Nếu Product không có navigation property cho OrderDetail
                .HasForeignKey(od => od.ProductID)
                .OnDelete(DeleteBehavior.Cascade);

            // Nếu sau này bạn muốn định nghĩa mối quan hệ giữa Customer và Order, bạn cần có một khóa ngoại trong Order (ví dụ: CustomerID).
            // Hiện tại, Order chỉ có CustomerName, nên mối quan hệ này chưa được định nghĩa.
        }
    }
}
