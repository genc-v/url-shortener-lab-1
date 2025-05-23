using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using URLShortener.Database;
using URLShortener.DTOs;
using URLShortener.DTOs.User;
using URLShortener.ModelHelpers;
using URLShortener.Models;
using URLShortener.Service;
using URLShortener.Service.User;

namespace URLShortener.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetUsers()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            bool isAdmin = Authentication.IsAdminFromToken(token);


            if (!isAdmin)
            {
                return BadRequest( new {message = "Bad token"});
            }

            var users = _userService.GetAllUsers();
            if (users.Any())
            {
                return Ok(users);
            }
            return NotFound("No user exists in the database");
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetUser(int id)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            bool isAdmin = Authentication.IsAdminFromToken(token);
            var userId = Authentication.GetUserIdFromToken(token);

            if(userId == null)
            {
                return BadRequest("Token not found");
            }

            if(userId == id)
            {
                var user = _userService.GetUserById(id);
                return Ok(user);
            } 
            else if (isAdmin)
            {
                var user = _userService.GetUserById(id);
                return Ok(user);
            }

            return NotFound($"User with ID {id} not found");
        }

        [HttpGet("Urls/{pageNumber}/{pageSize}")]
        [Authorize]
        public IActionResult GetUserWithUrls(int pageNumber = 1, int pageSize = 10)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            int userId = Authentication.GetUserIdFromToken(token);

            if (userId == null)
            {
                return BadRequest("Token not found");
            }

            var userUrls = _userService.GetUserWithUrls(userId);
            if (userUrls == null || userUrls.Urls == null)
            {
                return NotFound("No URLs found for the user");
            }

            var totalUrls = userUrls.Urls.Count();
            var totalPages = (int)Math.Ceiling((double)totalUrls / pageSize);
            
            var pagedUrls = userUrls.Urls
                .OrderByDescending(url => url.DateCreated) 
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(url => new 
                {
                    id = url.Id,
                    shortUrl = url.ShortUrl,
                    originalUrl = url.OriginalUrl,
                    description = url.Description,
                    nrOfClicks = url.NrOfClicks,
                    dateCreated = url.DateCreated
                })
                .ToList();

            var response = new
            {
                Urls = pagedUrls,
                TotalPages = totalPages
            };

            return Ok(response);
        }

        [HttpPost("silent-login")]
        [AllowAnonymous]
        public IActionResult SilentLogin([FromBody] string refreshToken)
        {
            var oldToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (oldToken == null || oldToken == string.Empty)
            {
                return BadRequest(new { message = "Token not valid" });
            }

            var id = Authentication.GetUserIdFromToken(oldToken, true);
            var newToken = _userService.SilentLogin(refreshToken, oldToken, id);

            if (newToken == null)
            {
                return BadRequest(new { message = "Token is still valid" });
            }

            return Ok(new {token = newToken});
            }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel request)
        {
            var tokens = _userService.Login(request);

            if (tokens == null)
            {
                return BadRequest(new { message = "UserName or Password is incorrect" });
            }

            return Ok(tokens);
        }


        [HttpDelete("Logout")]
        public IActionResult Logout([FromBody] string refreshToken)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (token == null || token == string.Empty)
            {
                return BadRequest(new { message = "Token not valid" });
            }

            var id = Authentication.GetUserIdFromToken(token);
            if(id == null)
            {
                return BadRequest(new { message = "Token not valid" });
            }
            _userService.Logout(id, refreshToken);
            return Ok(new { message = "Logout successful" });
        }

        [HttpPost("signup")]
        public IActionResult Signup([FromBody] SignUpModel request)
        {

            var newUser = _userService.AddUser(request);

            if (newUser != null)
            {
                return Ok(newUser);
            }

            return Conflict("Email is already taken");

        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateUser(int id, [FromBody] UserUpdate userInput)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userId = Authentication.GetUserIdFromToken(token);
            var isAdmin = Authentication.IsAdminFromToken(token);

            if (isAdmin)
            {
                var updatedUser = _userService.UpdateUser(id, userInput, true);
                if (updatedUser != null)
                {
                    return Ok(updatedUser);
                }
            }
            else
            {
                if (userId == id)
                {
                    var updatedUser = _userService.UpdateUser(userId, userInput);
                    if (updatedUser != null)
                    {
                        return Ok(updatedUser);
                    }
                }
            }
            return NotFound($"User with ID {id} not found");
        }

        [HttpDelete]
        [Authorize]
        public IActionResult DeleteUser(int id)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userId = Authentication.GetUserIdFromToken(token);
            var isAdmin = Authentication.IsAdminFromToken(token);

            if (userId == null)
            {
                return BadRequest("Token not found");
            }

            if (userId == id)
            {
                _userService.DeleteUser(userId);

                return Ok(new { message = $"User with ID {id} deleted successfully" });
            }
            else if (isAdmin)
            {
                _userService.DeleteUser(id);
                return Ok(new { message = $"User with ID {id} deleted successfully" });
            }
            return BadRequest($"User with ID {id} could not be found.");

        }

        [HttpGet("isAdmin")]
        [Authorize]
        public bool isAdmin()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            bool isAdmin = Authentication.IsAdminFromToken(token);
            return isAdmin;

        }
    }
}
