using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UserService.Application.DTOs;
using UserService.Application.Services;
using UserService.Domain.Entities;
using UserService.Infrastructure.DBContext;

namespace UserServiceAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserDbContext _dbContext;
        private readonly JwtService _jwtService;
        public UsersController(UserDbContext dbContext , JwtService jwtService)
        {
            _dbContext = dbContext;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            RegisterUserRequest request)
        {
            if (_dbContext.Users.Any(x => x.Email == request.Email))
            {
                return BadRequest("User already exists");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,


                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),

                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Users.Add(user);
            var response = new UserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = user.Id },
                response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _dbContext.Users.FindAsync(id);

            if (user == null)
                return NotFound();



            var response = new UserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            return Ok(response);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(
    LoginRequest request)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(
                    x => x.Email == request.Email);

            if (user == null)
            {
                return Unauthorized();
            }

            var validPassword =
                BCrypt.Net.BCrypt.Verify(
                    request.Password,
                    user.PasswordHash);

            if (!validPassword)
            {
                return Unauthorized();
            }

            var token =
                _jwtService.GenerateToken(user);

            return Ok(
                new LoginResponse
                {
                    Token = token
                });
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            var userId =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var email =
                User.FindFirst(ClaimTypes.Email)?.Value;

            return Ok(new
            {
                UserId = userId,
                Email = email
            });
        }
    }
}
