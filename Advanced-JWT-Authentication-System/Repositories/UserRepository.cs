using Advanced_JWT_Authentication_System.Interfaces;
using Advanced_JWT_Authentication_System.Models.Authentication;
using Advanced_JWT_Authentication_System.Models.Db;
using Advanced_JWT_Authentication_System.Models.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advanced_JWT_Authentication_System.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly authenticationContext _context;        
        private readonly IPasswordHasher<User> _passwordHasher;
        public UserRepository(authenticationContext context, IPasswordHasher<User> passwordHasher) : base(context)
        {
            _context = context;
            _passwordHasher = passwordHasher;

        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
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
