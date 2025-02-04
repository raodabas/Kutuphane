using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/users/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        // Veritabanında UserID'ye göre kullanıcıyı bul
        var user = await _context.Users.FindAsync(id);

        // Kullanıcı yoksa 404 dön
        if (user == null)
        {
            return NotFound(new { Message = $"Kullanıcı {id} bulunamadı!" });
        }

        // Kullanıcı bulunduysa bilgileri döndür
        return Ok(user);
    }
}
