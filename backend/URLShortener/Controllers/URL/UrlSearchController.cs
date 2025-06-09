using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using URLShortener.Database;
using URLShortener.ModelHelpers;
using URLShortener.Models;
using URLShortener.Service;

namespace URLShortener.Controllers;


[ApiController]
[Route("api/search")]
[EnableCors]
public class URLSearchController : ControllerBase
{

    private readonly UrlShortenerDbContext _context;

    public URLSearchController(UrlShortenerDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    [Authorize]
    public IActionResult SearchUrl(string UrlName, int pageNumber = 1, int pageSize = 10)
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        int userId = Authentication.GetUserIdFromToken(token);
        var searchQuery = UrlName.Trim().ToLower();

        if (string.IsNullOrEmpty(UrlName))
        {
            return BadRequest("Please type something...");
        }

        IQueryable<URL> query = _context.Urls;

        bool admin = Authentication.IsAdminFromToken(token);

        if (admin)
        {
            var resultsAdmin = query.Where(url => url.OriginalUrl.ToLower().Contains(searchQuery) || url.ShortUrl.ToLower().Contains(searchQuery) || url.Description.ToLower().Contains(searchQuery))
                .OrderByDescending(url => url.DateCreated)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList()
                .Select(url => new UrlResponseDto()
                {
                    UserId = url.UserId,
                    OriginalUrl = url.OriginalUrl,
                    ShortUrl = url.ShortUrl,
                    DateCreated = url.DateCreated,
                    Description = url.Description
                })
                .ToList();

            return Ok(new
            {
                Urls = resultsAdmin,
                TotalPages = (int)Math.Ceiling((double)query.Count() / pageSize)
            });
        }

        var results = query.Where(url => url.UserId == userId && (url.OriginalUrl.ToLower().Contains(searchQuery) || url.ShortUrl.ToLower().Contains(searchQuery) || url.Description.ToLower().Contains(searchQuery)))
            .OrderByDescending(url => url.DateCreated)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList()
            .Select(url => new UrlResponseDto()
            {
                UserId = url.UserId,
                OriginalUrl = url.OriginalUrl,
                ShortUrl = url.ShortUrl,
                Description = url.Description,
                DateCreated = url.DateCreated
            })
            .ToList();

        return Ok(new
        {
            Urls = results,
            TotalPages = (int)Math.Ceiling((double)query.Count(url => url.UserId == userId) / pageSize)
        });
    }
}