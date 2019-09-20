using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StatApi.Data;

namespace StatApi.Controllers
{
    [Route("api/{controller}")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly TwitchDbContext _context;

        public ChatController(TwitchDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> FilterByUserId(string id, [FromQuery] int page = 0, [FromQuery] int pageLength = 1000)
        {
            if (pageLength == 0) return Ok(new int[] {});
            var messages = await _context.ChatMessages
                .Where(c => c.SenderId == id)
                // Most recent messages first.
                .OrderByDescending(c => c.ReceivedOn)
                
                // Paging
                .Skip(page * pageLength)
                .Take(pageLength)
                
                // Filter out unnecessary information.
                .Select(c => new {c.Message, c.ChannelId, c.ReceivedOn})
                .ToListAsync();

            return Ok(messages);
        }
    }
}