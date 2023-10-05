using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RealTimeChatApp.Domain.DTO;
using RealTimeChatApp.Domain.Interfaces;
using RealTimeChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChatApp.DAL.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public UserService(UserManager<User> userManager, SignInManager<User> signInManager,IConfiguration configuration)
        {
            _userManager = userManager;   
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<UserDto> RegisterAsync(Register register)
        {
            var existingUser = await _userManager.FindByEmailAsync(register.Email);
            if (existingUser != null)
            {
                return null; // Handle duplicate email error
            }

            var newUser = new User
            {
                Email = register.Email,
                UserName = register.Email,
                FullName = register.Name
            };

            var result = await _userManager.CreateAsync(newUser, register.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(newUser, isPersistent: false);

                return new UserDto
                {
                    Id = newUser.Id,
                    Email = newUser.Email,
                    Name = register.Name
                };
            }
            else
            {
                return null; // Handle user creation failure
            }
        }

        public async Task<UserDto> AuthenticateAsync(Login login)
        {
            var user = await _userManager.FindByEmailAsync(login.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, login.Password))
            {
                return null; // Login failed due to incorrect credentials
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.FullName
            };

            return userDto;
        }

        public string GenerateJwtToken(UserDto user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                 new Claim(ClaimTypes.Name, user.Name)
            };

            var token = new JwtSecurityToken(
                _configuration["JwtSettings:Issuer"],
                _configuration["JwtSettings:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:DurationInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
