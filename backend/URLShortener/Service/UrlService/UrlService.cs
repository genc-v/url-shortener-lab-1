using System;
using URLShortener.Database;
using URLShortener.ModelHelpers;
using URLShortener.Models;
using URLShortener.Controllers;

namespace URLShortener.Service.Url
{

    public class UrlService : IUrlService
    {
        private readonly UrlShortenerDbContext _context;

        public UrlService(UrlShortenerDbContext context)
        {
            _context = context;
        }

        public IEnumerable<UrlResponseDto> GetAllUrls()
        {

            return _context.Urls
                .Select(url => new UrlResponseDto()
                {
                    Id = url.Id,
                    OriginalUrl = url.OriginalUrl,
                    ShortUrl = url.ShortUrl,
                    NrOfClicks = url.NrOfClicks,
                    UserId = url.UserId,
                    Description = url.Description,
                })
            .ToList();
        }


        public URL GetById(int id, int userId, bool isAdmin)
        {
            if(isAdmin) {
                return _context.Urls.FirstOrDefault(url => url.Id == id );
            }
            return _context.Urls.FirstOrDefault(url => url.Id == id &&  url.UserId == userId);
        }

        public string ShortenUrl(string originalUrl, string token, string description)
        {
            int userId = Authentication.GetUserIdFromToken(token);
            if (!_context.Users.Any(u => u.Id == userId))
            {
                throw new ArgumentException("User not found");
            }

            var existingUrl = _context.Urls.FirstOrDefault(u => u.OriginalUrl == originalUrl && u.UserId == userId);
            if (existingUrl != null)
            {
                throw new InvalidOperationException("URL already exists");
            }

            var newUrl = new URL()
            {
                OriginalUrl = originalUrl,
                ShortUrl = GenerateShortUrl(5),
                NrOfClicks = 0,
                UserId = userId,
                DateCreated = DateTime.UtcNow,
                Description = description
            };

            _context.Urls.Add(newUrl);
            _context.SaveChanges();

            return newUrl.ShortUrl;
        }

        public void DeleteUrl(int id, int userId)
        {
            var urlToDelete = _context.Urls.FirstOrDefault(url => url.Id == id && url.UserId == userId) ;
            if (urlToDelete != null)
            {
                _context.Urls.Remove(urlToDelete);
                _context.SaveChanges();
            }
        }

        public void UpdateUrl(int id, string description, int userId)
        {
            var urlToUpdate = _context.Urls.FirstOrDefault(url => url.Id == id && url.UserId == userId);
            if (urlToUpdate != null)
            {
                urlToUpdate.Description = description;
                _context.SaveChanges();
            }
        }

        private string GenerateShortUrl(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            return new string(
                Enumerable.Repeat(chars, length)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray()
            );
        }

        public int GetTotalUrls()
        {
            return _context.Urls.Count();
        }

        public int GetTotalClicks()
        {
            return _context.Urls.Sum(url => url.NrOfClicks);
        }

        public IEnumerable<object> GetTopLinks(int count)
        {
            return _context.Urls
                .OrderByDescending(url => url.NrOfClicks)
                .Take(count)
                .Select(url => new
                {
                    url.Id,
                    url.OriginalUrl,
                    url.ShortUrl,
                    url.NrOfClicks
                })
                .ToList();
        }

        public IEnumerable<object> GetRecentLinks(int count)
        {
            return _context.Urls
                .OrderByDescending(url => url.DateCreated)
                .Take(count)
                .Select(url => new
                {
                    url.Id,
                    url.OriginalUrl,
                    url.ShortUrl,
                    url.DateCreated
                })
                .ToList();
        }
    }
}
