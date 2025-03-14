using BusinessLayer.Helpers;
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTO;

namespace AddressBookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly JwtTokenGenerator _jwtTokenGenerator; //Inject JWT Helper
        private readonly IEmailService _emailService;

        public AuthController(IUserService userService, JwtTokenGenerator jwtTokenGenerator, IEmailService emailService)
        {
            _userService = userService;
            _jwtTokenGenerator = jwtTokenGenerator;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserDTO userDTO)
        {
            try
            {
                var user = _userService.RegisterUser(userDTO);
                return Ok(new { message = "User registered successfully!", user });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                var user = _userService.LoginUser(loginDTO.Email, loginDTO.Password);
                var token = _jwtTokenGenerator.GenerateToken(user.Email);
                return Ok(new { message = "Login successful!", user, token });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("secure-data")]
        public IActionResult GetSecureData()
        {
            return Ok("This is a secured API endpoint.");
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgetPasswordDTO forgotPasswordDTO)
        {
            var user = _userService.GetUserByEmail(forgotPasswordDTO.Email);
            if (user == null)
            {
                return NotFound(new { message = "User not found!" });
            }

            // Reset Token Generate Karo
            string resetToken = JwtTokenGenerator.GenerateResetToken();
            user.ResetToken = resetToken;
            user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(15); // Token valid for 15 min 

            _userService.UpdateUser(user);

            // Email Send Karo
            string resetLink = $"https://yourfrontend.com/reset-password?token={resetToken}";
            _emailService.SendEmail(user.Email, "Password Reset", $"Click here to reset your password: {resetLink}");

            return Ok(new { message = "Password reset link sent to your email!" });
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            var user = _userService.GetUserByResetToken(resetPasswordDTO.Token);
            if (user == null || user.ResetTokenExpiry < DateTime.UtcNow)
            {
                return BadRequest(new { message = "Invalid or expired reset token!" });
            }

            //Hash new password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordDTO.NewPassword);
            user.ResetToken = null;  // Token ko invalidate kar do
            user.ResetTokenExpiry = null;

            _userService.UpdateUser(user);

            return Ok(new { message = "Password reset successfully!" });
        }


    }
}
