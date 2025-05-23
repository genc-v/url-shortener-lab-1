using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using URLShortener.Database;
using URLShortener.Service;

namespace URLShortener.Controllers.Logs
{
    [EnableCors]
    [ApiController]
    public class LogsController : Controller
    {
        private UrlShortenerDbContext _context;

        public LogsController(UrlShortenerDbContext context)
        {
            _context = context;
        }

        [HttpGet("api/logs")]
        [Authorize]
        public IActionResult getLogs()
        {
            // does this work??
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            bool isAdmin = Authentication.IsAdminFromToken(token);

            if (!isAdmin) {
                return BadRequest(new { message = "User is not admin" });
            }

            return Ok(_context.Logs.ToList());

        }
    }
}
