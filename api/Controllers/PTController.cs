using api.Data;
using api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PTController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PTController(AppDbContext context) => _context = context;

        [HttpGet("my-members")]
        public async Task<IActionResult> GetMyMembers()
        {
            var members = await _context.Users
                .Where(u => u.Role == "Member")
                .Select(u => new { u.Id, u.FullName, u.Username, u.IsActive })
                .ToListAsync();
            return Ok(members);
        }

        [HttpPost("update-index")]
        public async Task<IActionResult> UpdateBodyIndex([FromBody] BodyIndex index)
        {
            _context.BodyIndices.Add(index);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật chỉ số sức khỏe thành công!" });
        }
    }
}
