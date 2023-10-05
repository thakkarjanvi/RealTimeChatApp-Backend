using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using RealTimeChatApp.Domain.DTO;
using RealTimeChatApp.Domain.Interfaces;
using RealTimeChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            //var existingUser = await _userManager.FindByEmailAsync(register.Email);
            //if (existingUser != null)
            //{
            //    return null; // Handle duplicate email error
            //}

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

    }
}
