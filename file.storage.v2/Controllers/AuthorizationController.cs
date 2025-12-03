using file.storage.v2.Domain.Entities.Business;
using file.storage.v2.Domain.Interfaces;
using file.storage.v2.Domain.Services;
using file_storage.DAL;
using file_storage.Domain.Dto;
using file_storage.Domain.Entities.Business;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace file_storage.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly FileStorageDbContext _context;
        private readonly ILogger<AuthorizationController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;

        public AuthorizationController(FileStorageDbContext context, ILogger<AuthorizationController> logger, IConfiguration configuration, ITokenService tokenService)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto user)
        {
            try
            {
                if (!ModelState.IsValid) 
                {
                     return BadRequest(ModelState);
                }
                var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == user.Email);

                if (existingUser != null)
                {
                    return BadRequest(new { message = "Пользователь с таким email уже существует" });
                }
                UserEntity newUser = UserEntity.CreateNewUser(user.Name, user.Email, user.Password);

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                var (accessToken, refreshToken) = _tokenService.GenerateTokens(newUser);

                _context.Tokens.Add(refreshToken);
                await _context.SaveChangesAsync();

                Response.Cookies.Append("refreshToken", refreshToken.Token, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddDays(Convert.ToInt32(_configuration["Jwt:RefreshTokenExpiresInDays"]))
                });

                return Ok(newUser.ToAuthUserDto(accessToken));
            }
            catch(Exception ex) 
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDto user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == user.Email);

                if (existingUser == null)
                {
                    return BadRequest(new { message = "Пользователя с таким email не существует!" });
                }

                if(this.ValidatePassword(user.Password, existingUser.Password))
                {
                    return BadRequest(new { message = "Пароль неверный!" });
                }

                var (accessToken, refreshToken) = _tokenService.GenerateTokens(existingUser);

                _context.Tokens.Add(refreshToken);
                await _context.SaveChangesAsync();

                Response.Cookies.Append("refreshToken", refreshToken.Token, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddDays(Convert.ToInt32(_configuration["Jwt:RefreshTokenExpiresInDays"]))
                });

                return Ok(existingUser.ToAuthUserDto(accessToken));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];

                if (refreshToken.IsNullOrEmpty())
                {
                    return BadRequest(new { message = "Пользователь не авторизован!" });
                }

                var userToken = await _context.Tokens.FirstOrDefaultAsync(x => x.Token == refreshToken);
                if(userToken == null)
                {
                    return BadRequest(new { message = "Пользователь не авторизован!" });
                }
                _context.Tokens.Remove(userToken);
                await _context.SaveChangesAsync();
                Response.Cookies.Delete("refreshToken", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });
                await Response.Body.FlushAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex });
            }
        }

        [HttpGet("refresh")]
        public async Task<IActionResult> Refresh()
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];
                var accessToken = Request.Headers.Authorization.ToString();
                var user = await _context.Users.Include(x => x.RefreshToken).FirstOrDefaultAsync(x => x.RefreshToken.Token == refreshToken);

                if(user.RefreshToken?.TokenExpires < DateTime.Now)
                {
                    _context.Tokens.Remove(user.RefreshToken);
                    await _context.SaveChangesAsync();
                    Response.Cookies.Delete("refreshToken", new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    });
                    await Response.Body.FlushAsync();

                    return BadRequest(new { message = "Пользователь не авторизован!" });
                }
                var newAccessToken = _tokenService.GenerateAccessToken(user);

                Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddDays(Convert.ToInt32(_configuration["Jwt:RefreshTokenExpiresInDays"]))
                });



                return Ok(user.ToAuthUserDto(newAccessToken));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        private bool ValidatePassword(string password, string savedPassword)
        {
            var isPasswordsEquals = savedPassword == BCrypt.Net.BCrypt.HashPassword(password);
            return isPasswordsEquals;
        }
    }
}
