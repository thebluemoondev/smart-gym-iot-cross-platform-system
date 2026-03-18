using api.Data;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class GymPackageController : ControllerBase
{
    private readonly AppDbContext _context;
    public GymPackageController(AppDbContext context) { _context = context; }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GymPackage>>> GetPackages() => await _context.GymPackages.ToListAsync();

    [HttpPost]
    public async Task<ActionResult<GymPackage>> CreatePackage(GymPackage package)
    {
        _context.GymPackages.Add(package);
        await _context.SaveChangesAsync();
        return Ok(package);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePackage(int id, GymPackage package)
    {
        if (id != package.Id) return BadRequest();
        _context.Entry(package).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return Ok(new { message = "Cập nhật thành công!" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePackage(int id)
    {
        var package = await _context.GymPackages.FindAsync(id);
        if (package == null) return NotFound();
        _context.GymPackages.Remove(package);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Đã xóa gói tập!" });
    }
}