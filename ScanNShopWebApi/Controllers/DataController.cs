using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScanNShopWebApi.Data;


namespace ScanNShopWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   [Authorize] // 🔒 NUR MIT JWT-TOKEN ZUGÄNGLICH
    public class DataController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DataController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("test-database")]
        public async Task<IActionResult> TestDatabase()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                if (canConnect)
                {
                    return Ok("✅ Verbindung zur Datenbank erfolgreich!");
                }
                return StatusCode(500, "❌ Verbindung fehlgeschlagen.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ Fehler: {ex.Message}");
            }
        }

    }
}
