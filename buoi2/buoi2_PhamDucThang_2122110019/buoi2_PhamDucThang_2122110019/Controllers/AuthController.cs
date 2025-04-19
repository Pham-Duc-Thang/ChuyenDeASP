using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using buoi2_PhamDucThang_2122110019.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace buoi2_PhamDucThang_2122110019.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        // -------------------- Các endpoint đăng ký - đăng nhập - xoá tài khoản của riêng người dùng --------------------

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Các role được phép: chỉ "Admin" và "Customer"
            var allowedRoles = new List<string> { "Admin", "Customer" };
            // Chuẩn hoá role: chữ đầu viết hoa, phần còn lại viết thường
            string desiredRole = char.ToUpper(model.Role[0]) + model.Role.Substring(1).ToLower();
            if (!allowedRoles.Contains(desiredRole))
            {
                return BadRequest(new { Message = "Role không hợp lệ. Chỉ cho phép 'Admin' hoặc 'Customer'." });
            }

            // Kiểm tra xem role đã tồn tại hay chưa
            if (!await _roleManager.RoleExistsAsync(desiredRole))
            {
                return BadRequest(new { Message = $"Role {desiredRole} không tồn tại." });
            }

            // Tạo tài khoản với IsDeleted = false
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = string.IsNullOrWhiteSpace(model.Email) ? string.Empty : model.Email,
                IsDeleted = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Gán role cho user
            var roleResult = await _userManager.AddToRoleAsync(user, desiredRole);
            if (!roleResult.Succeeded)
            {
                // Nếu gán role thất bại, xoá user vừa tạo
                await _userManager.DeleteAsync(user);
                return BadRequest(roleResult.Errors);
            }

            var token = GenerateJwtToken(user);
            return Ok(new { Message = "Đăng ký thành công.", Token = token });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                return Unauthorized(new { Message = "Thông tin đăng nhập không hợp lệ." });
            }

            // Kiểm tra nếu tài khoản đã bị xoá mềm
            if (user.IsDeleted)
            {
                return Unauthorized(new { Message = "Tài khoản đã bị vô hiệu hóa hoặc xoá mềm." });
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
                return Unauthorized(new { Message = "Thông tin đăng nhập không hợp lệ." });

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        // DELETE: api/auth/delete
        // Cho người dùng tự xoá tài khoản của mình (soft delete)
        [HttpDelete("delete")]
        [Authorize] // Yêu cầu người dùng đã đăng nhập
        public async Task<IActionResult> DeleteOwnAccount()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new { Message = "User not authenticated." });

            var user = await _userManager.FindByNameAsync(username);
            if (user == null || user.IsDeleted)
                return NotFound(new { Message = "Tài khoản không tồn tại hoặc đã bị xoá mềm." });

            user.IsDeleted = true;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { Message = "Tài khoản đã được xoá mềm thành công." });
        }

        // -------------------- Các endpoint quản trị tài khoản (Chỉ Admin sử dụng) --------------------
        // Các endpoint dưới đây giúp Admin xem toàn bộ tài khoản, xem tài khoản đã xoá mềm, cập nhật thông tin tài khoản và xoá/khoi phục tài khoản.

        // GET: api/auth/accounts
        [HttpGet("accounts")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAccounts()
        {
            // Lấy tất cả các tài khoản (không xoá mềm)
            var accounts = _userManager.Users.ToList();
            return Ok(accounts);
        }

        // GET: api/auth/accounts/deleted
        [HttpGet("accounts/deleted")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDeletedAccounts()
        {
            // Bỏ qua global query filter nếu có bằng IgnoreQueryFilters()
            var deletedAccounts = await _userManager.Users
                                        .IgnoreQueryFilters()
                                        .Where(u => u.IsDeleted)
                                        .ToListAsync();
            return Ok(deletedAccounts);
        }

        // PUT: api/auth/accounts/{id}
        // Cập nhật thông tin tài khoản bởi Admin (ví dụ: cập nhật Email)
        [HttpPut("accounts/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAccount(string id, [FromBody] UpdateUserModel model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null || user.IsDeleted)
                return NotFound(new { Message = "Tài khoản không tồn tại hoặc đã bị xoá mềm." });

            // Cập nhật thông tin (ví dụ chỉ cập nhật email ở đây)
            user.Email = model.Email;
            // Nếu cần cập nhật các trường khác, bổ sung thêm ở đây

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { Message = "Tài khoản đã được cập nhật thành công." });
        }

        // DELETE: api/auth/accounts/{id}
        // Xoá mềm tài khoản của người dùng khác (Admin)
        [HttpDelete("accounts/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAccountByAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null || user.IsDeleted)
                return NotFound(new { Message = "Tài khoản không tồn tại hoặc đã bị xoá mềm." });

            user.IsDeleted = true;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { Message = "Tài khoản đã được xoá mềm thành công." });
        }

        // POST: api/auth/accounts/restore/{id}
        // Khôi phục tài khoản đã bị xoá mềm (Admin)
        [HttpPost("accounts/restore/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreAccount(string id)
        {
            var user = await _userManager.Users
                                .IgnoreQueryFilters()
                                .FirstOrDefaultAsync(u => u.Id == id && u.IsDeleted);
            if (user == null)
                return NotFound(new { Message = "Tài khoản không tồn tại hoặc không ở trạng thái xoá mềm." });

            user.IsDeleted = false;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { Message = "Tài khoản đã được khôi phục thành công." });
        }

        // Phương thức phát hành JWT token, bao gồm các claim chứa role của user
        private string GenerateJwtToken(ApplicationUser user)
        {
            // Lấy danh sách role của user
            var roles = _userManager.GetRolesAsync(user).Result;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            // Thêm claim chứa role
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
