using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BankingApplication.Models;

namespace BankingApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignInController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly APIDbContext _context;

        public SignInController(IConfiguration configuration, APIDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        private string GenerateJwtToken(string username)
        {
            // Define security key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])); // Replace with your secret key

            // Create credentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create claims
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
            };

            // Create token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(5),
                signingCredentials: creds
            );

            // Return the generated token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public class SignInRequest
        {
            public string Username { get; set; } = "";
            public string Password { get; set; } = "";
        }

        private bool AuthenticateUser(string email, string password)
        {
            try
            {
                // For simplicity, let's assume you have an instance of your DbContext called _context
                var user = _context.Users.SingleOrDefault(u => u.Email == email);

                if (user != null && user.Password == password)
                {
                    // Authentication successful
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"Error during authentication: {ex.Message}");
            }

            // Authentication failed
            return false;
        }

        [HttpPost("SignIn")]
        public IActionResult SignIn([FromBody] SignInRequest request)
        {
            var isAuthenticated = AuthenticateUser(request.Username, request.Password);

            if (isAuthenticated)
            {
                var token = GenerateJwtToken(request.Username);
                return Ok(new { Token = token });
            }

            return Unauthorized("Invalid credentials");
        }
    }
}
