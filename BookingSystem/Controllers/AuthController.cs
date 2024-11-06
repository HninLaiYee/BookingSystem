using BookingSystem.Models;
using BookingSystem.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace BookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;
        public AuthController(UserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("token")]
        [Authorize(AuthenticationSchemes = "BasicAuthentication")]
        public IActionResult token()
        {
            try
            {
                TokenModel tokenModel = new TokenModel();
                // Generate JWT token here
                var claims = new[]
                {
                new Claim(ClaimTypes.Name, "codigo")
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "codigo.com",
                    audience: "codigo.com",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds);

                tokenModel.access_token = new JwtSecurityTokenHandler().WriteToken(token);
                tokenModel.expires_in = "1800";

                return Ok(tokenModel);
            }
            catch {
                return Ok(new { message = "Authentication fail." });
            }

            
        }

        [HttpPost("register")]
        [Authorize(AuthenticationSchemes = "BasicAuthentication")]
        public async Task<IActionResult> Register(RegisterModel registrationDto)
        {
            LoginTokenModel loginTokenModel = new LoginTokenModel();
            try
            {
                if (registrationDto == null || string.IsNullOrEmpty(registrationDto.Username) || string.IsNullOrEmpty(registrationDto.Password
               ) || string.IsNullOrEmpty(registrationDto.Email))
                {
                    return BadRequest("Invalid user data.");
                }
                bool isRegistered = _userService.RegisterUser(registrationDto);
                if (!isRegistered)
                {
                    return Conflict("Username or email already exists.");
                }


                User user = new User();
                user = _userService.GetRegisterUser(registrationDto.Username, registrationDto.Email);
                if(user != null)
                {
                    loginTokenModel.userid = user.Id.ToString();
                    loginTokenModel.username = user.UserName.ToString();
                    loginTokenModel.access_token = _userService.GetBearerToken(user.Id.ToString());
                    loginTokenModel.expires_in = "3600";
                    return Ok(loginTokenModel);
                }
                else
                {
                    return BadRequest("User registered fail.");
                }
              

            }
            catch(Exception ex)
            {
                return BadRequest("User registered fail.");
            }
           
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            LoginTokenModel loginTokenModel = new LoginTokenModel();
            try
            {
                User user = new User();

                user = _userService.Login(model);

                if (user == null)
                {
                    return Unauthorized("Invalid credentials or email not verified.");
                }
                else
                {
                    loginTokenModel.userid = user.Id.ToString();
                    loginTokenModel.username = user.UserName.ToString();
                    loginTokenModel.access_token = _userService.GetBearerToken(user.Id.ToString());
                    loginTokenModel.expires_in = "1800";
                }
                return Ok(loginTokenModel);

            }
            catch(Exception ex)
            {
                return BadRequest("Login fail.");
            }
            
        }

     

        [HttpGet("profile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetProfile(string userid)
        {
            try
            {
                var userProfile = await _userService.GetUserId(userid);
                if (userProfile != null)
                {
                    return Ok(userProfile);
                }
                return Ok("There is no user profile.");
            }
            catch (Exception ex)
            {
                return Ok("There is no user profile.");
            }

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool result = await _userService.ChangePasswordAsync(changePasswordDto);

            if (result)
                return Ok("Password changed successfully.");

            return BadRequest("Password changed fail.");
        }


        [HttpPost("reset-password")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            bool result = await _userService.ResetPasswordAsync(resetPasswordDto);

            if (result)
                return Ok("Password has been reset successfully.");

            return BadRequest("Invalid or expired token.");
        }



    }
}
