using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using URLShortener.Database;
using URLShortener.ModelHelpers;
using URLShortener.Models;
using URLShortener.Service;
using URLShortener.Service.Url;

namespace URLShortener.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class URLController : Controller
    {
        private readonly IUrlService _urlService;
        private readonly IUrlValidationService _urlValidationService;
        public URLController(IUrlService urlService, IUrlValidationService urlValidationService)
        {
            _urlService = urlService;
            _urlValidationService = urlValidationService;
        }
        // TODO make the things be able to be edited from admin

        [HttpGet]
        [Authorize]
        public IActionResult GetUrls()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            bool isAdmin = Authentication.IsAdminFromToken(token);
            if(!isAdmin) 
            {
                return BadRequest(new { meassage = "User is not admin" });
            }

            var allUrls = _urlService.GetAllUrls();
            if (allUrls.Any())
            {
                return Ok(allUrls);
            }
            return NotFound("Database is empty");
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetUrl(int id)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            int userId = Authentication.GetUserIdFromToken(token);
            bool isAdmin = Authentication.IsAdminFromToken(token);

            var url = _urlService.GetById(id, userId, isAdmin);
            if(url == null)
            {
                return NotFound("Couldn't find url with the specified id: " + id);
            }
            return Ok(url);
        }


        [HttpPost]
        [Authorize]
        public IActionResult ShortenUrl([FromBody] UrlShorten urlToShort)
        {

            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var verifedToken = TokenService.VerifyToken(token);

            if (verifedToken == null)
            {
                return BadRequest(new { message = "Token not valid" });
            }

            if (!_urlValidationService.IsValidUrl(urlToShort.url))
            {
                return BadRequest("Invalid URL format Please provide a valid URL starting with 'http://' or 'https://'.");
            }

            try
            {
                var shortUrl = _urlService.ShortenUrl(urlToShort.url, token, urlToShort.description);
                return Ok(new { shortedUrl =  shortUrl});
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Remove(int id)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userId = Authentication.GetUserIdFromToken(token);
            if (userId == null)
            {
                return BadRequest(new { message = "Bad token or you do not have premission" });
            } 
            _urlService.DeleteUrl(id, userId);
            return Ok(new {message = "URL Deleted Succesfully"});
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUrl(int id,[FromBody] string description)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userId = Authentication.GetUserIdFromToken(token);

            try
            {
                _urlService.UpdateUrl(id, description, userId);
                return Ok( new {message ="URL updated Successfully"});
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpGet("analytics")]
        public IActionResult GetAnalytics()
        {

            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userId = Authentication.GetUserIdFromToken(token);
            if (userId == null)
            {
                return BadRequest(new { message = "Bad token or you do not have premission" });
            } 

            try
            {
                var totalUrls = _urlService.GetTotalUrls();
                var totalClicks = _urlService.GetTotalClicks();
                var topLinks = _urlService.GetTopLinks(3);
                var recentLinks = _urlService.GetRecentLinks(5);

                return Ok(new
                {
                    TotalUrls = totalUrls,
                    TotalClicks = totalClicks,
                    TopLinks = topLinks,
                    RecentLinks = recentLinks
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
