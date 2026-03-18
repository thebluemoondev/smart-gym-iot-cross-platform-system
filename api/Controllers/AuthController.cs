using api.Data;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username && u.Password == request.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Tài khoản hoặc mật khẩu không chính xác!" });
            }

            if (!user.IsActive)
            {
                return BadRequest(new { message = "Tài khoản đã bị khóa!" });
            }

            return Ok(new
            {
                message = "Đăng nhập thành công!",
                userId = user.Id,
                fullName = user.FullName,
                role = user.Role
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Users user)
        {
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                return BadRequest(new { message = "Username đã tồn tại!" });

            user.IsActive = true;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Tạo tài khoản thành công!" });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}