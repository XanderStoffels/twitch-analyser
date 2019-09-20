using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StatApi.Data;

namespace StatApi.Controllers
{
    [Route("api/{controller}")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly TwitchDbContext _context;

        public UserController(TwitchDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> UserInformation(string id)
        {
            var usernames = await _context.Usernames
                .Where(u => u.UserId == id)
                .Select(u => new {u.Username, u.FirstSeen})
                .ToListAsync();
            return Ok(usernames);
        }
        
        [HttpGet("lookup/{username}")]
        public async Task<IActionResult> LookupUserId(string username)
        {
            var selection = await _context.Usernames
                .Where(u => u.Username == username)
                .OrderByDescending(u => u.FirstSeen)
                .Select(u => u.UserId)
                .ToListAsync();
            
            if (selection.Any())
                return Ok(new {CurrentUser = selection.First(), PreviousUsers = selection.Skip(1)});

            return NotFound(new {Message = "A user with the given username can not be found."});
        }
    }
}