using BusinessLayer.Helpers;
using BusinessLayer.Interface;
using BusinessLayer.Service;
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
        private readonly JwtTokenGenerator _jwtTokenGenerator;
        private readonly IEmailService _emailService;
        private readonly RabbitMQService _rabbitMQService;

        public AuthController(IUserService userService, JwtTokenGenerator jwtTokenGenerator, IEmailService emailService, RabbitMQService rabbitMQService)
        {
            _userService = userService;
            _jwtTokenGenerator = jwtTokenGenerator;
            _emailService = emailService;
            _rabbitMQService = rabbitMQService;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userDTO">User registration details.</param>
        /// <returns>Returns a success message along with the registered user details.</returns>
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserDTO userDTO)
        {
            try
            {
                var user = _userService.RegisterUser(userDTO);
                _rabbitMQService.PublishMessage("UserRegisteredQueue", $"New user registered: {userDTO.Email}");

                return Ok(new { message = "User registered successfully!", user });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Logs in a user and generates a JWT token.
        /// </summary>
        /// <param name="loginDTO">Login credentials.</param>
        /// <returns>Returns a JWT token upon successful login.</returns>
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

        /// <summary>
        /// Retrieves secure data. Requires authentication.
        /// </summary>
        /// <returns>Returns secured data if the user is authenticated.</returns>
        [Authorize]
        [HttpGet("secure-data")]
        public IActionResult GetSecureData()
        {
            return Ok("This is a secured API endpoint.");
        }

        /// <summary>
        /// Initiates the forgot password process.
        /// </summary>
        /// <param name="forgotPasswordDTO">Email of the user who wants to reset their password.</param>
        /// <returns>Returns a message indicating whether the reset email was sent.</returns>
        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgetPasswordDTO forgotPasswordDTO)
        {
            var user = _userService.GetUserByEmail(forgotPasswordDTO.Email);
            if (user == null)
            {
                return NotFound(new { message = "User not found!" });
            }

            string resetToken = JwtTokenGenerator.GenerateResetToken();
            user.ResetToken = resetToken;
            user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);

            _userService.UpdateUser(user);

            string resetLink = $"https://yourfrontend.com/reset-password?token={resetToken}";
            _emailService.SendEmail(user.Email, "Password Reset", $"Click here to reset your password: {resetLink}");

            return Ok(new { message = "Password reset link sent to your email!" });
        }

        /// <summary>
        /// Resets the user's password using a valid reset token.
        /// </summary>
        /// <param name="resetPasswordDTO">Reset token and new password.</param>
        /// <returns>Returns a message indicating whether the password was reset successfully.</returns>
        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            var user = _userService.GetUserByResetToken(resetPasswordDTO.Token);
            if (user == null || user.ResetTokenExpiry < DateTime.UtcNow)
            {
                return BadRequest(new { message = "Invalid or expired reset token!" });
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordDTO.NewPassword);
            user.ResetToken = null;
            user.ResetTokenExpiry = null;

            _userService.UpdateUser(user);

            return Ok(new { message = "Password reset successfully!" });
        }
    }
}
