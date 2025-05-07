using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using URLShortener.Database;
using URLShortener.DTOs;
using URLShortener.DTOs.User;
using URLShortener.ModelHelpers;
using URLShortener.Models;

namespace URLShortener.Service.User
{
    public class UserService : IUserService
    {
        private readonly UrlShortenerDbContext _context;

        public UserService(UrlShortenerDbContext context)
        {
            _context = context;
        }

        public IEnumerable<UserUrls> GetAllUsers()
        {
            var usersWithUrls = _context.Users
                .GroupJoin(_context.Urls,
                    user => user.Id,
                    url => url.UserId,
                    (user, urls) => new UserUrls
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FullName = user.FullName,
                        CreatedAt = user.CreatedAt,
                        Urls = urls.ToList()
                    })
                .ToList();

            return usersWithUrls;
        }

        public UserUrls? GetUserById(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            return user != null ? new UserUrls
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                CreatedAt = user.CreatedAt
            } : null;
        }

        public UserUrls? GetUserWithUrls(int id)  
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return null;
            }

            var userUrls = _context.Urls.Where(url => url.UserId == id).ToList();
            return new UserUrls
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                CreatedAt = user.CreatedAt,
                Urls = userUrls
            };
        }

        public LoginTokens? Login(LoginModel req)
        {

            var user = _context.Users
                .FirstOrDefault(user => user.Email == req.Email);

            if (user == null)
            {
                return null;
            }
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                return null;
            }

            var token = TokenService.GenerateToken(user.Id, user.Email, user.PasswordHash, user.isAdmin);

            var refreshToken = new Models.RefreshToken
            {
                UserId = user.Id,
                CreationDate = DateOnly.FromDateTime(DateTime.UtcNow),
                ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)),
                Guid = Guid.NewGuid()
            };

            _context.RefreshToken.Add(refreshToken);
            _context.SaveChanges();

            return new LoginTokens { token = token, refreshToken = refreshToken.Guid.ToString()};
        }

        public string SilentLogin(string rt, string t, int id)
        {
            var now = DateTime.UtcNow;
            var refreshToken = _context.RefreshToken.FirstOrDefault(e => e.UserId == id && e.Guid.ToString() == rt);
            var jwtHandler = new JwtSecurityTokenHandler();

            if (!jwtHandler.CanReadToken(t)) return null;

            var jwtToken = jwtHandler.ReadJwtToken(t);
            var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "iat")?.Value;

            if (expClaim != null && DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim)).UtcDateTime < refreshToken.CreationDate.ToDateTime(TimeOnly.MinValue)) return null;

            if (refreshToken == null) return null;

            var refreshTokenCreated = refreshToken.CreationDate.ToDateTime(TimeOnly.MinValue);
            var refreshTokenExpiry = refreshToken.ExpiryDate.ToDateTime(TimeOnly.MinValue);

            if (refreshTokenExpiry < now) return null;

            var user = _context.Users.FirstOrDefault(e => e.Id == id);

            return TokenService.GenerateToken(user.Id, user.Email, user.PasswordHash, user.isAdmin);
        }

        public void Logout(int id, string rt)
        {
            var refreshToken = _context.RefreshToken.FirstOrDefault(e => e.UserId == id && e.Guid.ToString() == rt);
            if (refreshToken != null)
            {
                _context.RefreshToken.Remove(refreshToken);
                _context.SaveChanges();
            }
        }

        public SignUpModel? AddUser(SignUpModel request) 
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == request.Email);
            if (existingUser != null)
            {
                return null;
            }
            var hashedPassowrd = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var newUser = new Models.User
            {
                Email = request.Email,
                FullName = request.FullName,
                PasswordHash = hashedPassowrd,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return new SignUpModel
            {
                Email = newUser.Email,
                FullName = newUser.FullName,
                Password = string.Empty
            };
        }

        public UserUpdate? UpdateUser(int id, UserUpdate userInput)  
        {
            if (userInput == null)
            {
                throw new ArgumentNullException(nameof(userInput));
            }

            var userToUpdate = _context.Users.FirstOrDefault(u => u.Id == id);
            
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(userInput.Password, userToUpdate.PasswordHash);
            if (!isPasswordValid)
            {
                return null;
            }
           
            if (userToUpdate == null)
            {
                return null;
            }
            var hashedPassowrd = BCrypt.Net.BCrypt.HashPassword(userInput.Password);
            userToUpdate.Email = userInput.Email;
            userToUpdate.FullName = userInput.FullName;
            userToUpdate.PasswordHash = hashedPassowrd;
            _context.SaveChanges();

            return new UserUpdate
            {
                Email = userToUpdate.Email,
                FullName = userToUpdate.FullName,
            };
        }

        public void DeleteUser(int id)
        {
            var userToDelete = _context.Users.FirstOrDefault(u => u.Id == id);
            if (userToDelete != null)
            {
                var userUrls = _context.Urls.Where(url => url.UserId == id);
                _context.Urls.RemoveRange(userUrls);

                var refreshTokens = _context.RefreshToken.Where(rt => rt.UserId == id);
                _context.RefreshToken.RemoveRange(refreshTokens);

                _context.Users.Remove(userToDelete);
                _context.SaveChanges();
            }
        }
    }
}