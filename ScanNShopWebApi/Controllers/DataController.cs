using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScanNShopWebApi.Data;
using ScanNShopWebApi.DTO;
using ScanNShopWebApi.Models;
using System.Diagnostics;


namespace ScanNShopWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   [Authorize] //  NUR MIT JWT-TOKEN ZUGÄNGLICH
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

        // Benutzer anhand seines Usernames abrufen (gibt UserDto zurück)
        [HttpGet("users/by-username/{username}")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var user = await _context.Users
                .Where(u => u.Username == username)
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound($"❌ Benutzer '{username}' nicht gefunden.");
            }

            return Ok(user);
        }

        // Listen eines bestimmten Benutzers anhand seiner User-ID abrufen (gibt ListDto zurück)
        [HttpGet("users/{userId:int}/lists")]
        public async Task<IActionResult> GetListsByUserId(int userId)
        {
            var listIds = await _context.RelatListUsers
                .Where(r => r.Relat_UserId == userId)
                .Select(r => r.Relat_ListId)
                .ToListAsync();

            if (listIds == null || !listIds.Any())
            {
                return NotFound($"❌ Keine Listen für Benutzer mit ID {userId} gefunden.");
            }

            var lists = await _context.Lists
                .Where(l => listIds.Contains(l.ListId))
                .Select(l => new ListDto
                {
                    ListId = l.ListId,
                    Name = l.Name
                })
                .ToListAsync();

            return Ok(lists);
        }

        // NEUE METHODE: Produkte anhand mehrerer ListIDs abrufen
        [HttpPost("products/by-list-ids")]
        public async Task<IActionResult> GetProductsByListIds([FromBody] List<int> listIds)
        {
            if (listIds == null || !listIds.Any())
            {
                return BadRequest("❌ Keine ListIDs angegeben.");
            }

            var products = await _context.Products
                .Where(p => listIds.Contains(p.ListId.Value))
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    ListId = p.ListId,
                    Name = p.Name,
                    Quantity = p.Quantitiy,
                    IsChecked = p.isChecked
                })
                .ToListAsync();

            if (!products.Any())
            {
                return NotFound("❌ Keine Produkte für die angegebenen ListIDs gefunden.");
            }

            return Ok(products);
        }

        [HttpPost("lists")]
        public async Task<IActionResult> CreateList([FromBody] CreateListDto listDto)
        {
            if (string.IsNullOrWhiteSpace(listDto.Name) || listDto.UserId <= 0)
                return BadRequest("❌ Listenname oder UserId ungültig.");

            var newList = new Models.List
            {
                Name = listDto.Name
            };
            _context.Lists.Add(newList);
            await _context.SaveChangesAsync();

            // Verknüpfung mit Benutzer speichern
            var relation = new RelatListUser
            {
                Relat_UserId = listDto.UserId,
                Relat_ListId = newList.ListId
            };
            _context.RelatListUsers.Add(relation);
            await _context.SaveChangesAsync();

            return Ok(new { ListId = newList.ListId });
        }


        [HttpPost("products")]
        public async Task<IActionResult> InsertProducts([FromBody] List<ProductDto> products)
        {
            if (products == null || !products.Any())
                return BadRequest("❌ Keine Produkte angegeben.");

            foreach (var dto in products)
            {
                if (!dto.ListId.HasValue)
                    return BadRequest("❌ Produkt hat keine ListId.");

                var product = new Product
                {
                    ListId = dto.ListId,
                    Name = dto.Name,
                    Quantitiy = dto.Quantity,
                    isChecked = dto.IsChecked
                };
                _context.Products.Add(product);
            }

            await _context.SaveChangesAsync();
            return Ok("✅ Produkte erfolgreich eingefügt.");
        }


        [HttpDelete("lists/{listId}")]
        public async Task<IActionResult> DeleteList(int listId)
        {
            var list = await _context.Lists.FindAsync(listId);
            if (list == null)
                return NotFound("❌ Liste nicht gefunden.");

            var products = _context.Products.Where(p => p.ListId == listId);
            
            var relats = await _context.RelatListUsers
    .Where(r => r.Relat_ListId == listId)
    .ToListAsync();

        

            


            _context.Products.RemoveRange(products);
            _context.RelatListUsers.RemoveRange(relats);
            _context.Lists.Remove(list);

            await _context.SaveChangesAsync();
            return Ok("✅ Liste und zugehörige Daten wurden gelöscht.");
        }

        [HttpPut("products/update")]
        public async Task<IActionResult> UpdateProducts([FromBody] List<ProductDto> productDtos)
        {
            if (productDtos == null || !productDtos.Any())
                return BadRequest("❌ Keine Produkte übergeben.");

            foreach (var dto in productDtos)
            {
                if (dto.ProductId <= 0)
                {
                    Debug.WriteLine("❌ Ungültige ProductId empfangen.");
                    continue;
                }

                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == dto.ProductId && p.ListId == dto.ListId);

                if (product == null)
                {
                    Debug.WriteLine($"❌ Produkt mit ID {dto.ProductId} und ListId {dto.ListId} nicht gefunden.");
                    continue;
                }

                Debug.WriteLine($"✅ Produkt gefunden – Name: {product.Name}, Menge: {product.Quantitiy}, Checked: {product.isChecked}");

                product.Name = dto.Name;
                product.Quantitiy = dto.Quantity;
                product.isChecked = dto.IsChecked;

                Debug.WriteLine($"➡️ Aktualisiert – Name: {product.Name}, Menge: {product.Quantitiy}, Checked: {product.isChecked}");
            }

            await _context.SaveChangesAsync();
            return Ok("✅ Produkte wurden aktualisiert.");
        }

        [HttpPost("products/single")]
        public async Task<IActionResult> InsertSingleProduct([FromBody] ProductDto dto)
        {
            if (!dto.ListId.HasValue || string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("❌ Ungültiges Produkt.");

            var newProduct = new Product
            {
                ListId = dto.ListId,
                Name = dto.Name,
                Quantitiy = dto.Quantity,
                isChecked = dto.IsChecked
            };

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            return Ok(new { productId = newProduct.ProductId });
        }


    }
}
