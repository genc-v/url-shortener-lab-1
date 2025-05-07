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
            try
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

                _context.Logs.Add(new Log
                {
                    Note = "Retrieved all users with their URLs",
                    CreatedAt = DateTime.UtcNow
                });
                _context.SaveChanges();

                return usersWithUrls;
            }
            catch (Exception ex)
            {
                _context.Logs.Add(new Log
                {
                    Note = $"Error retrieving all users: {ex.Message}",
                    CreatedAt = DateTime.UtcNow
                });
                _context.SaveChanges();
                throw;
            }
        }

        public UserUrls? GetUserById(int id)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == id);

                if (user != null)
                {
                    _context.Logs.Add(new Log
                    {
                        Note = $"Retrieved user by ID: {id}",
                        CreatedAt = DateTime.UtcNow
                    });
                    _context.SaveChanges();
                }
                else
                {
                    _context.Logs.Add(new Log
                    {
                        Note = $"Attempted to retrieve non-existent user with ID: {id}",
                        CreatedAt = DateTime.UtcNow
                    });
                    _context.SaveChanges();
                }

                return user != null ? new UserUrls
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    CreatedAt = user.CreatedAt
                } : null;
            }
            catch (Exception ex)
            {
                _context.Logs.Add(new Log
                {
                    Note = $"Error retrieving user by ID {id}: {ex.Message}",
                    CreatedAt = DateTime.UtcNow
                });
                _context.SaveChanges();
                throw;
            }
        }

        public UserUrls? GetUserWithUrls(int id)  
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    _context.Logs.Add(new Log
                    {
                        Note = $"Attempted to retrieve URLs for non-existent user with ID: {id}",
                        CreatedAt = DateTime.UtcNow
                    });
                    _context.SaveChanges();
                    return null;
                }

                var userUrls = _context.Urls.Where(url => url.UserId == id).ToList();

                _context.Logs.Add(new Log
                {
                    Note = $"Retrieved user with URLs for user ID: {id}",
                    CreatedAt = DateTime.UtcNow
                });
                _context.SaveChanges();

                return new UserUrls
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    CreatedAt = user.CreatedAt,
                    Urls = userUrls
                };
            }
            catch (Exception ex)
            {
                _context.Logs.Add(new Log
                {
                    Note = $"Error retrieving user with URLs for ID {id}: {ex.Message}",
                    CreatedAt = DateTime.UtcNow
                });
                _context.SaveChanges();
                throw;
            }
        }

        public LoginTokens? Login(LoginModel req)
        {
            try
            {
                var user = _context.Users
                    .FirstOrDefault(user => user.Email == req.Email);

                if (user == null)
                {
                    _context.Logs.Add(new Log
                    {
                        Note = $"Failed login attempt for email: {req.Email} (user not found)",
                        CreatedAt = DateTime.UtcNow
                    });
                    _context.SaveChanges();
                    return null;
                }

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);

                if (!isPasswordValid)
                {
                    _context.Logs.Add(new Log
                    {
                        Note = $"Failed login attempt for user ID: {user.Id} (invalid password)",
                        CreatedAt = DateTime.UtcNow
                    });
                    _context.SaveChanges();
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
                
                _context.Logs.Add(new Log
                {
                    Note = $"Successful login for user ID: {user.Id}",
                    CreatedAt = DateTime.UtcNow
                });
                _context.SaveChanges();

                return new LoginTokens { token = token, refreshToken = refreshToken.Guid.ToString()};
            }
            catch (Exception ex)
            {
                _context.Logs.Add(new Log
                {
                    Note = $"Error during login for email {req.Email}: {ex.Message}",
                    CreatedAt = DateTime.UtcNow
                });
                _context.SaveChanges();
                throw;
            }
        }

        public string SilentLogin(string rt, string t, int id)
        {
            try
            {
                var now = DateTime.UtcNow;
                var refreshToken = _context.RefreshToken.FirstOrDefault(e => e.UserId == id && e.Guid.ToString() == rt);
                var jwtHandler = new JwtSecurityTokenHandler();

                if (!jwtHandler.CanReadToken(t))
                {
                    _context.Logs.Add(new Log
                    {
                        Note = $"Failed silent login for user ID: {id} (invalid token)",
                        CreatedAt = DateTime.UtcNow
                    });
                    _context.SaveChanges();
                    return null;
                }

                var jwtToken = jwtHandler.ReadJwtToken(t);
                var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "iat")?.Value;

                if (expClaim != null && DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim)).UtcDateTime < refreshToken.CreationDate.ToDateTime(TimeOnly.MinValue))
                {
                    _context.Logs.Add(new Log
                    {
                        Note = $"Failed silent login for user ID: {id} (token expired)",
                        CreatedAt = DateTime.UtcNow
                    });
                    _context.SaveChanges();
                    return null;
                }

                if (refreshToken == null)
                {
                    _context.Logs.Add(new Log
                    {
                        Note = $"Failed silent login for user ID: {id} (refresh token not found)",
                        CreatedAt = DateTime.UtcNow
                    });
                    _context.SaveChanges();
                    return null;
                }

                var refreshTokenCreated = refreshToken.CreationDate.ToDateTime(TimeOnly.MinValue);
                var refreshTokenExpiry = refreshToken.ExpiryDate.ToDateTime(TimeOnly.MinValue);

                if (refreshTokenExpiry < now)
                {
                    _context.Logs.Add(new Log
                    {
                        Note = $"Failed silent login for user ID: {id} (refresh token expired)",
                        CreatedAt = DateTime.UtcNow
                    });
                    _context.SaveChanges();
                    return null;
                }

                var user = _context.Users.FirstOrDefault(e => e.Id == id);

                _context.Logs.Add(new Log
                {
                    Note = $"Successful silent login for user ID: {id}",
                    CreatedAt = DateTime.UtcNow
                });
                _context.SaveChanges();

                return TokenService.GenerateToken(user.Id, user.Email, user.PasswordHash, user.isAdmin);
            }
            catch (Exception ex)
            {
                _context.Logs.Add(new Log
                {
                    Note = $"Error during silent login for user ID {id}: {ex.Message}",
                    CreatedAt = DateTime.UtcNow
                });
                _context.SaveChanges();
                throw;
            }
        }

        public void Logout(int id, string rt)
        {
            try
            {
                var refreshToken = _context.RefreshToken.FirstOrDefault(e => e.UserId == id && e.Guid.ToString() == rt);
                if (refreshToken != null)
                {
                    _context.RefreshToken.Remove(refreshToken);
                    _context.Logs.Add(new Log
                    {
                        Note = $"User ID: {id} logged out successfully",
                        CreatedAt = DateTime.UtcNow
                    });
                    _context.SaveChanges();
                }
                else
                {
                    _context.Logs.Add(new Log
                    {
                        Note = $"Attempted logout for user ID: {id} with invalid refresh token",
                        CreatedAt = DateTime.UtcNow
                    });
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _context.Logs.Add(new Log
                {
                    Note = $"Error during logout for user ID {id}: {ex.Message}",
                    CreatedAt = DateTime.UtcNow
                });
                _context.SaveChanges();
                throw;
            }
        }

        public SignUpModel? AddUser(SignUpModel request) 
        {
            try
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                var existingUser = _context.Users.FirstOrDefault(u => u.Email == request.Email);
                if (existingUser != null)
                {
                    _context.Logs.Add(new Log
                    {
                        Note = $"Attempted to add user with existing email: {request.Email}",
                        CreatedAt = DateTime.UtcNow
                    });
                    _context.SaveChanges();
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
                _context.Logs.Add(new Log
                {
                    Note = $"Added new user with email: {request.Email}",
                    CreatedAt = DateTime.UtcNow
                });
                _context.SaveChanges();

                return new SignUpModel
                {
                    Email = newUser.Email,
                    FullName = newUser.FullName,
                    Password = string.Empty
                };
            }
            catch (Exception ex)
            {
                _context.Logs.Add(new Log
                {
                    Note = $"Error adding user with email {request.Email}: {ex.Message}",
                    CreatedAt = DateTime.UtcNow
                });
                _context.SaveChanges();
                throw;
            }
        }

        public UserUpdate? UpdateUser(int id, UserUpdate userInput)
        {
            try
            {
                if (userInput == null)
                {
                    throw new ArgumentNullException(nameof(userInput));
                }

                var userToUpdate = _context.Users.FirstOrDefault(u => u.Id == id);
                if (userToUpdate == null)
                {
                    _context.Logs.Add(new Log
                    {
                        Note = $"Attempted to update non-existent user with ID: {id}",
                        CreatedAt = DateTime.UtcNow
                    });
                    _context.SaveChanges();
                    return null;
                }

                if (!string.IsNullOrEmpty(userInput.Password))
                {
                    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(userInput.Password, userToUpdate.PasswordHash);
                    if (!isPasswordValid)
                    {
                        _context.Logs.Add(new Log
                        {
                            Note = $"Failed password verification for user ID: {userToUpdate.Id}",
                            CreatedAt = DateTime.UtcNow
                        });
                        _context.SaveChanges();
                        return null;
                    }
                }

                if (userInput.Email != null)
                {
                    userToUpdate.Email = userInput.Email;
                }

                if (userInput.FullName != null)
                {
                    userToUpdate.FullName = userInput.FullName;
                }

                // Handle password change if new password is provided
                if (!string.IsNullOrEmpty(userInput.oldPassword))
                {
                    if (string.IsNullOrEmpty(userInput.Password))
                    {
                        // Require old password to set new password
                        _context.Logs.Add(new Log
                        {
                            Note = $"Attempted password change without old password for user ID: {id}",
                            CreatedAt = DateTime.UtcNow
                        });
                        _context.SaveChanges();
                        return null;
                    }
                    userToUpdate.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userInput.oldPassword);
                }

                _context.Logs.Add(new Log
                {
                    Note = $"Updated user with ID: {id}",
                    CreatedAt = DateTime.UtcNow
                });
                _context.SaveChanges();

                return new UserUpdate
                {
                    Email = userToUpdate.Email,
                    FullName = userToUpdate.FullName,
                };
            }
            catch (Exception ex)
            {
                _context.Logs.Add(new Log
                {
                    Note = $"Error updating user with ID {id}: {ex.Message}",
                    CreatedAt = DateTime.UtcNow
                });
                _context.SaveChanges();
                throw;
            }
        }
        public void DeleteUser(int id)
        {
            try
            {
                var userToDelete = _context.Users.FirstOrDefault(u => u.Id == id);
                if (userToDelete != null)
                {
                    var userUrls = _context.Urls.Where(url => url.UserId == id);
                    _context.Urls.RemoveRange(userUrls);

                    var refreshTokens = _context.RefreshToken.Where(rt => rt.UserId == id);
                    _context.RefreshToken.RemoveRange(refreshTokens);

                    _context.Users.Remove(userToDelete);

                    _context.Logs.Add(new Log
                    {
                        Note = $"Deleted user with ID: {id} and all associated data",
                        CreatedAt = DateTime.UtcNow
                    });
                    _context.SaveChanges();
                }
                else
                {
                    _context.Logs.Add(new Log
                    {
                        Note = $"Attempted to delete non-existent user with ID: {id}",
                        CreatedAt = DateTime.UtcNow
                    });
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _context.Logs.Add(new Log
                {
                    Note = $"Error deleting user with ID {id}: {ex.Message}",
                    CreatedAt = DateTime.UtcNow
                });
                _context.SaveChanges();
                throw;
            }
        }
    }
}