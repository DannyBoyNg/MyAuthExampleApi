using Services.EmailService;
using Services.JwtTokenService;
using Services.SimpleTokenService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using Services.PasswordHashingService;
using Services.UserService;
using System.Threading;

namespace myAuthExampleApi.Controllers
{
    [Authorize]
    [Route("api/user/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IJwtTokenService jwtTokenService;
        private readonly ISimpleTokenService simpleTokenService;
        private readonly IPasswordHashingService hashingService;
        private readonly IEmailService emailService;

        public AuthController(
            IUserService userService,
            IJwtTokenService jwtTokenService,
            ISimpleTokenService simpleTokenService,
            IPasswordHashingService hashingService,
            IEmailService emailService)
        {
            this.userService = userService;
            this.jwtTokenService = jwtTokenService;
            this.simpleTokenService = simpleTokenService;
            this.hashingService = hashingService;
            this.emailService = emailService;
        }

        [Route("/Startup")]
        [AllowAnonymous]
        public ActionResult Startup()
        {
            return Ok("Dev Server Started");
        }

        [Route("/Token")]
        [AllowAnonymous]
        public ActionResult Token(string username, string password)
        {
            //sleep timer to demonstrate the loading bar
            Thread.Sleep(2000);
            //get user from data store
            var user = userService.GetByName(username);
            //validate password
            if (user == null) return BadRequest("Invalid credentials");
            if (user.Active != true) return BadRequest("Account not active");
            if (hashingService.VerifyHashedPassword(user.PasswordHash, password) == PasswordVerificationResult.Failed) return BadRequest("Invalid credentials");
            //create claims
            var claims = new List<Claim> { new Claim("uid", user.Id.ToString(), ClaimValueTypes.Integer) };
            //create tokens
            var accessToken = jwtTokenService.GenerateAccessToken(user.UserName, claims);
            var refreshToken = jwtTokenService.GenerateRefreshToken();
            //store refresh token
            jwtTokenService.StoreRefreshToken(user.Id, refreshToken);
            return Ok(new JwtToken { AccessToken = accessToken, RefreshToken = refreshToken, TokenType = "bearer" });
        }

        [Route("/Refresh")]
        [AllowAnonymous]
        public ActionResult Refresh(string accessToken, string refreshToken)
        {
            //get userId from access token
            var claimsPrincipal = jwtTokenService.GetPrincipalFromExpiredAccessToken(accessToken);
            var uid = jwtTokenService.GetClaim(claimsPrincipal, "uid");
            if (!int.TryParse(uid, out int userId)) return Unauthorized("Invalid access token");
            //validate refresh token (Optionally delete refreshToken after validation)
            jwtTokenService.ValidateRefreshToken(userId, refreshToken);
            //get user from data store
            var user = userService.GetById(userId);
            //create new tokens
            var claims = new List<Claim> { new Claim("uid", uid) };
            var newAccessToken = jwtTokenService.GenerateAccessToken(user.UserName, claims);
            var newRefreshToken = jwtTokenService.GenerateRefreshToken();
            //store refresh token in data store
            jwtTokenService.StoreRefreshToken(userId, newRefreshToken);
            return Ok(new JwtToken { AccessToken = newAccessToken, RefreshToken = newRefreshToken, TokenType = "bearer" });
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(string username, string email, string password)
        {
            //Input validation
            var errors = new List<string>();
            if (!userService.IsEmailUnique(email)) errors.Add("Email already exists");
            if (!userService.IsNameUnique(username)) errors.Add("Username already exists");
            if (!emailService.IsEmailValid(email)) errors.Add("Email is not valid");
            if (errors.Any()) return BadRequest(errors);
            //Create new user
            var passwordHash = hashingService.HashPassword(password);
            IUser user = new User
            {
                UserName = username,
                Email = email,
                PasswordHash = passwordHash,
                Active = true,
            };
            userService.Create(user);
            var simpleToken = simpleTokenService.Generate();
            simpleTokenService.StoreToken(user.Id, simpleToken);
            //Don't send email if no email host settings are defined
            if (string.IsNullOrWhiteSpace(emailService.Settings.Host)) return NoContent();
            var domain = emailService.Settings.Domain;
            var body = $@"Welcome,

Your account has been created. Click on this link to confirm your email: http://localhost:4200/confirmemail/{user.Id}/{simpleToken}";
            emailService.SendEmail(new MailAddress($"no-reply@{domain}"), new string[] { email }, "New account", body);
            return NoContent();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ResetPassword(int userId, string token, string password, string confirmPassword)
        {
            //Input validation
            if (password == null) throw new ArgumentNullException(nameof(password));
            //Logic
            simpleTokenService.Validate(userId, token);
            var user = userService.GetById(userId);
            if (user == null) return BadRequest();
            if (user.Active == false) return BadRequest("Account is not active.");
            if (password != confirmPassword) return BadRequest("Password is not equal to confirm password.");
            userService.UpdatePassword(userId, hashingService.HashPassword(password));
            return NoContent();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotUsername(string email)
        {
            //Don't send email if no email host settings are defined
            if (string.IsNullOrWhiteSpace(emailService.Settings.Host)) return BadRequest("Unable to send email. Email host setting is not set.");
            //Logic
            var domain = emailService.Settings.Domain;
            var user = userService.GetByEmail(email);
            if (user == null) return NotFound();
            if (user.Active == false) return BadRequest("Unable to retrieve username. Account is not active.");
            if (!emailService.IsEmailValid(email)) return BadRequest("Email is not a valid email address");
            var token = simpleTokenService.Generate();
            simpleTokenService.StoreToken(user.Id, token);
            var body = $@"Your username is: {user.UserName}";
            emailService.SendEmail(new MailAddress($"no-reply@{domain}"), new string[] { email }, "Username", body);
            return NoContent();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotPassword(string email)
        {
            //Don't send email if no email host settings are defined
            if (string.IsNullOrWhiteSpace(emailService.Settings.Host)) return BadRequest("Unable to send email. Email host setting is not set.");
            //Logic
            var domain = emailService.Settings.Domain;
            var user = userService.GetByEmail(email);
            if (user == null) return NotFound();
            if (user.Active == false) return BadRequest("Cannot reset password. Account is not active.");
            if (!emailService.IsEmailValid(email)) return BadRequest("Email is not a valid email address");
            var token = simpleTokenService.Generate();
            simpleTokenService.StoreToken(user.Id, token);
            var body = $@"A password reset was requested for this account. Click on the following link to reset your password: http://localhost:4200/resetpassword/{user.Id}/{token}";
            emailService.SendEmail(new MailAddress($"no-reply@{domain}"), new string[] { email }, "Password reset", body);
            return NoContent();
        }

        [HttpGet("{userId}/{token}")]
        [AllowAnonymous]
        public ActionResult ConfirmEmail(int userId, string token)
        {
            if (userService.IsEmailConfirmed(userId)) return BadRequest("Your email has already been confirmed");
            simpleTokenService.Validate(userId, token);
            userService.SetEmailConfirmed(userId);
            return NoContent();
        }

        [HttpPost]
        public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var user = userService.GetByName(User.Identity.Name);
            if (newPassword != confirmPassword) return BadRequest("New password and confirm password are not equal.");
            if (user == null) return BadRequest("User not found.");
            if (!user.Active) return BadRequest("User is not active.");
            if (hashingService.VerifyHashedPassword(user.PasswordHash, currentPassword) == PasswordVerificationResult.Failed) return BadRequest("Current password is not correct.");
            userService.UpdatePassword(user.Id, hashingService.HashPassword(newPassword));
            return NoContent();
        }

    }
}
