using BookingSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BookingSystem.Services
{
    public class UserService
    {
        MainDBContext _context = new MainDBContext();
        private readonly IConfiguration _configuration;

        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // Method to register a new user
        public bool RegisterUser(RegisterModel model)
        {
            try
            {
                // Check if the user already exists
                if (_context.Users.Any(u => u.UserName == model.Username || u.Email == model.Email))
                {
                    return false; // User already exists
                }

                // Hash the password for secure storage
                var hashedPassword = HashPassword(model.Password);

                // Create the new user
                var user = new User
                {
                    Id = Guid.NewGuid(),  // This would be auto-generated in a real DB
                    UserName = model.Username,
                    Email = model.Email,
                    PasswordHash = hashedPassword,
                    CountryCode = model.CountryCode

                };

                _context.Entry(user).State = EntityState.Added;
                _context.SaveChanges();

                if (SendVerifyEmail(model.Email))
                {                   
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
           
        }

        public User GetRegisterUser(string username, string email)
        {
            User user = new User();
            try
            {
                user = _context.Users.Where(u => u.UserName == username && u.Email == email).FirstOrDefault();
                return user;
            }
            catch (Exception ex)
            {
                return user;
            }
        }

        public async Task<User> GetUserId(string id)
        {
            User user = new User();
            try
            {
                user = _context.Users.Where(u => u.Id == Guid.Parse(id)).FirstOrDefault();
                return user;
            }
            catch (Exception ex)
            {
                return user;
            }
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            User user = new User();
            try
            {
                user = _context.Users.Where(u => u.Id == Guid.Parse(changePasswordDto.UserId)).FirstOrDefault();
                if(user != null)
                {
                    user.PasswordHash = HashPassword(changePasswordDto.NewPassword);
                    _context.Entry(user).State = EntityState.Modified;
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            User user = new User();
            try
            {
                user = _context.Users.Where(u => u.Id == Guid.Parse(resetPasswordDto.UserId)).FirstOrDefault();
                if (user != null)
                {
                    user.PasswordHash = HashPassword(resetPasswordDto.NewPassword);
                    _context.Entry(user).State = EntityState.Modified;
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        // Simple method to hash passwords (for demonstration purposes, use a proper library in production)
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        public bool SendVerifyEmail(string email)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == email);
                if (user != null)
                {
                    user.IsEmailVerified = true;
                    _context.Entry(user).State = EntityState.Modified;
                    _context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
           

            return true;
        }

        public User Login(LoginModel model)
        {
            var user = _context.Users.AsEnumerable().FirstOrDefault(u => u.UserName == model.Username
                                                        && VerifyPassword(u.PasswordHash, model.Password));

            if (user != null && user.IsEmailVerified)
            {
                // Successful login logic (e.g., generate JWT token)
                return user;
            }
            return user;
        }

        private bool VerifyPassword(string storedHash, string password)
        {
            return storedHash == HashPassword(password);
        }

        public string GetBearerToken(string userid)
        {
            string bearertoken = "";
            try
            {
                // Generate JWT token after successful basic auth
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userid) }),
                    Expires = DateTime.Now.AddMinutes(30),
                    Issuer = null,
                    Audience = null,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);


                return bearertoken = tokenString;

            }
            catch (Exception ex)
            {
                return bearertoken;
            }

        }


    }
}
