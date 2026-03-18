using api.Data;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RfidController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RfidController(AppDbContext context)
        {
            _context = context;
        }

        private static readonly List<object> RealTimeQueue = new List<object>();

        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn([FromBody] RfidRequest request)
        {
            var user = await _context.Users
                .Include(u => u.Subscriptions)
                .FirstOrDefaultAsync(u => u.RfidCardGuid == request.CardUid && u.IsActive);

            if (user == null)
            {
                return NotFound(new { status = "Denied", message = "Thẻ không tồn tại!" });
            }

            var activeSubscription = user.Subscriptions?
                .FirstOrDefault(s => s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now);

            bool accessGranted = activeSubscription != null;

            var history = new CheckInHistory
            {
                UserId = user.Id,
                CheckInTime = DateTime.Now,
                AccessGranted = accessGranted
            };
            _context.CheckInHistories.Add(history);
            await _context.SaveChangesAsync();

            lock (RealTimeQueue)
            {
                RealTimeQueue.Add(new
                {
                    Id = history.Id,
                    UserName = user.FullName,
                    Status = accessGranted ? "Thành công" : "Từ chối",
                    CheckInTime = history.CheckInTime.ToString("HH:mm:ss")
                });
            }

            if (accessGranted)
            {
                return Ok(new
                {
                    status = "Success",
                    name = user.FullName,
                    message = "Mời vào!"
                });
            }

            return BadRequest(new
            {
                status = "Denied",
                message = "Gói tập đã hết hạn!"
            });
        }

        [HttpGet("get-new-logs")]
        public IActionResult GetNewLogs()
        {
            lock (RealTimeQueue)
            {
                var logs = RealTimeQueue.ToList();
                RealTimeQueue.Clear(); 
                return Ok(logs);
            }
        }

        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetMemberHistory(int userId)
        {
            var history = await _context.CheckInHistories
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.CheckInTime)
                .Select(h => new {
                    h.CheckInTime,
                    Status = h.AccessGranted ? "Thành công" : "Từ chối"
                }).ToListAsync();
            return Ok(history);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetAllHistory()
        {
            var history = await _context.CheckInHistories
                .Include(h => h.User)
                .OrderByDescending(h => h.CheckInTime)
                .Select(h => new {
                    h.CheckInTime,
                    UserName = h.User != null ? h.User.FullName : "Thẻ lạ",
                    CardUid = h.User != null ? h.User.RfidCardGuid : "N/A",
                    Status = h.AccessGranted ? "Thành công" : "Từ chối"
                })
                .ToListAsync();

            return Ok(history);
        }
    }

    public class RfidRequest
    {
        public string CardUid { get; set; } = string.Empty;
    }
}