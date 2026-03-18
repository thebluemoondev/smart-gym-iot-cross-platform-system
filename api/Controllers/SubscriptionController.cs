using api.Data;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SubscriptionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            var package = await _context.GymPackages.FindAsync(request.GymPackageId);

            if (user == null || package == null)
                return NotFound(new { message = "User hoặc Gói tập không tồn tại!" });

            DateTime startDate = DateTime.Now;
            DateTime endDate = startDate.AddDays(package.DurationDays);

            var subscription = new Subscription
            {
                UserId = request.UserId,
                GymPackageId = request.GymPackageId,
                StartDate = startDate,
                EndDate = endDate
            };

            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Đăng ký gói tập thành công!",
                user = user.FullName,
                package = package.Name,
                expiresOn = endDate
            });
        }

        [HttpDelete("cancel/{userId}")]
        public async Task<IActionResult> CancelSubscription(int userId)
        {
            var subscription = await _context.Subscriptions
                .Where(s => s.UserId == userId && s.EndDate >= DateTime.Now)
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefaultAsync();

            if (subscription == null)
                return NotFound(new { message = "Hội viên này hiện không có gói tập nào để hủy!" });

            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã hủy gói tập thành công!" });
        }
    }

    public class SubscribeRequest
    {
        public int UserId { get; set; }
        public int GymPackageId { get; set; }
    }
}