using Services.EmailService;
using Services.JwtTokenService;
using Services.SimpleTokenService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using myAuthExampleApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using Services.PasswordHashingService;
using Services.UserService;
using myAuthExampleApi.Repositories;
using System.Threading;

namespace myAuthExampleApi.Controllers
{
    [Authorize]
    [Route("api/user/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IUserService userService;
        private readonly IJwtTokenService jwtTokenService;
        private readonly ISimpleTokenService simpleTokenService;
        private readonly IPasswordHashingService hashingService;
        private readonly IEmailService emailService;
        private readonly IRefreshTokenRepository refreshTokenRepo;

        public AuthController(
            IConfiguration configuration,
            IUserService userService,
            IJwtTokenService jwtTokenService,
            ISimpleTokenService simpleTokenService,
            IPasswordHashingService hashingService,
            IEmailService emailService,
            IRefreshTokenRepository refreshTokenRepo)
        {
            this.configuration = configuration;
            this.userService = userService;
            this.jwtTokenService = jwtTokenService;
            this.simpleTokenService = simpleTokenService;
            this.hashingService = hashingService;
            this.emailService = emailService;
            this.refreshTokenRepo = refreshTokenRepo;
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
            Thread.Sleep(3000);
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

            if (jwtTokenService.IsRefreshTokenExpired(refreshToken)) return Unauthorized("Refresh token expired");
            //validate refresh token (Optionally delete refreshToken after validation)
            if(!refreshTokenRepo.IsValid(userId, refreshToken)) return Unauthorized("Invalid refresh token");
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
            var errors = new List<string>();
            if (!userService.IsEmailUnique(email)) errors.Add("Email already exists");
            if (!userService.IsNameUnique(username)) errors.Add("Username already exists");
            if (!emailService.IsEmailValid(email)) errors.Add("Email is not valid");
            if (errors.Any()) return BadRequest(errors);

            var passwordHash = hashingService.HashPassword(password);
            var user = new Users
            {
                UserName = username,
                Email = email,
                PasswordHash = passwordHash,
                Active = true,
            };
            userService.Create(user as IUsers);
            var simpleToken = simpleTokenService.Generate();
            simpleTokenService.StoreToken(user.Id, simpleToken);
            var domain = emailService.Settings.Domain;
            var body = $@"Welcome,

Your account has been created. Click on this link to confirm your email: http://localhost:4200/confirmemail/{user.Id}/{simpleToken}";
            emailService.SendEmail(new MailAddress($"no-reply@{domain}"), new string[] { email }, "New account", body);
            return NoContent();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ResetPassword(int userId, string simpleToken, string password, string confirmPassword)
        {
            //Input validation
            if (password == null) throw new ArgumentNullException(nameof(password));
            //Logic
            var user = userService.GetById(userId);
            if (user == null) return BadRequest();
            if (user.Active == false) return BadRequest("Account is not active.");
            if (!simpleTokenService.IsValid(userId, simpleToken)) return BadRequest("Invalid token");
            if (password.Length < 8) return BadRequest("Password is not at least 8 characters long.");
            if (password != confirmPassword) return BadRequest("Password is not equal to confirm password.");
            userService.UpdatePassword(userId, hashingService.HashPassword(password));
            return NoContent();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotUsername(string email)
        {
            var domain = configuration["EmailSettings:Domain"];
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
            var domain = configuration["Domain"];
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
            if (simpleTokenService.IsExpired(token)) return BadRequest("Expired token");
            if (!simpleTokenService.IsValid(userId, token)) return BadRequest("Invalid token");
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
