using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using URLShortener.Database;
using URLShortener.Models;
using URLShortener.Service;

namespace URLShortener.Controllers
{

    [ApiController]
    [Route("/")]
    [EnableCors]    
    public class URLRedirectController : ControllerBase
    {
        private UrlShortenerDbContext _context;
        public URLRedirectController(UrlShortenerDbContext context)
        {
            _context = context;
        }

        [HttpGet("/api/{shortUrl}")]
        public IActionResult RedirectShortUrl(string shortUrl)
        {

            try
            {
                var urlMapping = _context.Urls.FirstOrDefault(u => u.ShortUrl == shortUrl);

                if (urlMapping != null)
                {
                    urlMapping.NrOfClicks++;
                    _context.SaveChanges();

                    return Ok(new { url = urlMapping.OriginalUrl});
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}