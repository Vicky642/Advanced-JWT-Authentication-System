using Advanced_JWT_Authentication_System.Interfaces;
using Advanced_JWT_Authentication_System.Models.Authentication;
using Advanced_JWT_Authentication_System.Models.Db;
using Advanced_JWT_Authentication_System.Models.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Advanced_JWT_Authentication_System.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly authenticationContext _context;        
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;
        public UserRepository(authenticationContext context, IPasswordHasher<User> passwordHasher, IConfiguration configuration) : base(context)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<ExecutionResult<User>> LoginAsync(LoginModel model)
        {
            var user = await GetByUsernameAsync(model.Username);

            // Check if the user is null or the password verification fails
            if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Failed)
            {
                return new ExecutionResult<User>("Invalid username or password.");
            }

            var token = GenerateJwtToken(user);
            return new ExecutionResult<User>(true, "Login successful", user, token);
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            // Add other claims as needed
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<ExecutionResult<User>> RegisterAsync(RegistrationModel model)
        {
            // Check if the username or email already exists
            var existingUserByEmail = await GetByEmailAsync(model.Email);
            var existingUserByUsername = await GetByUsernameAsync(model.UserName);

            if (existingUserByEmail != null || existingUserByUsername != null)
            {
                return new ExecutionResult<User>("Email or Username is already in use.");
            }

            // Hash the password
            var hashedPassword = _passwordHasher.HashPassword(null, model.Password);

            // Create the new user
            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PasswordHash = hashedPassword,
                PhoneNumber = model.PhoneNumber,
                UserName = model.UserName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Add the user to the repository
            await AddAsync(user);

            return new ExecutionResult<User>(true, "User registered successfully!", user);
        }

    } 
}
