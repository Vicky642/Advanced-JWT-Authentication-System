using Advanced_JWT_Authentication_System.Interfaces;
using Advanced_JWT_Authentication_System.Models.Authentication;
using Advanced_JWT_Authentication_System.Models.Db;
using Advanced_JWT_Authentication_System.Models.Results;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RestSharp;
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

        public async Task<ExecutionResult<User>> GoogleSignInAsync(GoogleSignInModel model)
        {
            try
            {
                var client = new RestClient("https://oauth2.googleapis.com");
                var request = new RestRequest("/token", Method.Post);

                // Ensure the parameters are added correctly
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("application/x-www-form-urlencoded",
                    $"code={model.codeModel}&client_id={model.clientId}&client_secret={model.clientSecret}&redirect_uri={model.redirectUri}&grant_type=authorization_code",
                    ParameterType.RequestBody);

                var response = await client.ExecuteAsync(request);

                if (!response.IsSuccessful)
                {
                    return new ExecutionResult<User>($"Failed to get the token from Google: {response.ErrorMessage}");
                }

                var tokenResponse = JsonConvert.DeserializeObject<GoogleTokenResponse>(response.Content);

                if (tokenResponse == null)
                {
                    Console.WriteLine("Deserialization failed. Raw response: " + response.Content);
                    return new ExecutionResult<User>("Failed to deserialize token response.");
                }

                var handler = new JwtSecurityTokenHandler();
                var idToken = handler.ReadJwtToken(tokenResponse.IdToken);
                var email = idToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                var name = idToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;

                // Check if user exists, if not create a new user
                var user = await GetByEmailAsync(email);
                if (user == null)
                {
                    user = new User
                    {
                        UserName = email,
                        Email = email,
                        FullName = name,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await AddAsync(user);
                }

                var token = GenerateJwtToken(user);
                return new ExecutionResult<User>(true, "Google Sign-In successful", user, token);
            }
            catch (Exception ex)
            {
                return new ExecutionResult<User>($"An error occurred during Google Sign-In: {ex.Message}");
            }
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
                FullName = model.FullName,                
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
