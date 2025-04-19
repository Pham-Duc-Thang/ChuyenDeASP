namespace buoi2_PhamDucThang_2122110019.Models
{
    public class RegisterModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        // Role được nhập theo định dạng chuỗi (chỉ cho phép "Admin" hoặc "Customer")
        public string Role { get; set; }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    // Dùng để cập nhật thông tin tài khoản bởi Admin
    public class UpdateUserModel
    {
        // Ví dụ, ta cho phép cập nhật Email (có thể mở rộng thêm các thuộc tính khác như FullName, Phone, …)
        public string Email { get; set; }
        // Nếu muốn cập nhật password thì cần xử lý riêng
        // public string NewPassword { get; set; }
    }
}
