﻿using URLShortener.ModelHelpers;
using URLShortener.Models;

namespace URLShortener.Service.Url
{
    public interface IUrlService
    {
        IEnumerable<UrlResponseDto> GetAllUrls();
        URL GetById(int id, int userId, bool isAdmin);
        string ShortenUrl(string originalUrl, string tokens, string description);
        void DeleteUrl(int id, int userId);
        void UpdateUrl(int id, string description, int userId);
        int GetTotalUrls(int userId);
        int GetTotalClicks(int userId);
        IEnumerable<object> GetTopLinks(int count, int userId);
        IEnumerable<object> GetRecentLinks(int count, int userId);
    }

}
