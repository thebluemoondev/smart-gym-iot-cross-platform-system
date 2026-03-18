using api.Data;
using api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UsersController(AppDbContext context) { _context = context; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.Subscriptions)
                .ThenInclude(s => s.GymPackage)
                .Select(u => new {
                    u.Id,
                    u.FullName,
                    u.Username,
                    u.RfidCardGuid,
                    u.IsActive,
                    ActivePackage = u.Subscriptions
                        .Where(s => s.EndDate >= DateTime.Now)
                        .OrderByDescending(s => s.EndDate)
                        .Select(s => new {
                            PackageName = s.GymPackage.Name,
                            ExpiryDate = s.EndDate
                        })
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] Users updatedUser)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (!string.IsNullOrEmpty(updatedUser.FullName)) user.FullName = updatedUser.FullName;

            if (!string.IsNullOrEmpty(updatedUser.Password)) user.Password = updatedUser.Password;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật thành công!" });
        }

        [HttpPost("toggle-status/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();
            return Ok(new { status = user.IsActive, message = "Đã cập nhật trạng thái!" });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users
                .Include(u => u.Subscriptions).ThenInclude(s => s.GymPackage)
                .Select(u => new {
                    u.Id,
                    u.FullName,
                    ActivePackage = u.Subscriptions
                        .Where(s => s.EndDate >= DateTime.Now)
                        .OrderByDescending(s => s.EndDate)
                        .Select(s => new { PackageName = s.GymPackage.Name, ExpiryDate = s.EndDate })
                        .FirstOrDefault()
                }).FirstOrDefaultAsync(u => u.Id == id);

            return user == null ? NotFound() : Ok(user);
        }
    }
}
