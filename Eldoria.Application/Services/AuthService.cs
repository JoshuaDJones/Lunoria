using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Eldoria.Application.Services
{
    public class AuthService : IAuthService
    {
        private IUserRepository _userRepository;
        private IPasswordHasher<User> _passwordHasher;
        private IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public async Task<Result<AuthenticationTokenDto>> AuthenticateUser(string email, string password, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return Result<AuthenticationTokenDto>.Fail(new Error("User.LoginFailure", "Invalid email or password."));

            var normalizedEmail = NormalizeEmail(email);
            var user = await _userRepository.GetByEmail(normalizedEmail, ct);

            var invalid = Result<AuthenticationTokenDto>.Fail(new Error("User.LoginFailure", "Invalid email or password."));

            if (user is null || user.IsDeleted || user.IsLocked)
                return invalid;

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (verificationResult == PasswordVerificationResult.Failed)
                return invalid;

            if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, password);
                user.UpdateDate = DateTime.UtcNow;
                await _userRepository.SaveChangesAsync(ct);
            }

            var token = IssueJwt(user.Id.ToString(), user.Email, _configuration);
            return Result<AuthenticationTokenDto>.Ok(token);
        }

        public async Task<Result<AuthenticationTokenDto>> RegisterUser(string email, string password, CancellationToken ct)
        {
            var normalizedEmail = NormalizeEmail(email);

            var isEmailValid = IsEmailValid(normalizedEmail);
            var isPasswordValid = IsPasswordValid(password, out var passwordError);

            if (!isEmailValid)
                return Result<AuthenticationTokenDto>.Fail(new Error("User.InvalidEmail", $"{email} is not a valid email address."));

            if (!isPasswordValid)
                return Result<AuthenticationTokenDto>.Fail(new Error("User.InvalidPassword", passwordError));

            var emailExists = await _userRepository.EmailExists(normalizedEmail, ct);
            if (emailExists)
                return Result<AuthenticationTokenDto>.Fail(new Error("User.EmailExists", $"{email} is already tied to an existing account."));

            var now = DateTime.UtcNow;

            var user = new User()
            {
                Email = normalizedEmail,
                CreateDate = now,
                UpdateDate = now
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            try
            {
                await _userRepository.AddAsync(user, ct);
                await _userRepository.SaveChangesAsync(ct);
            }
            catch (DbUpdateException)
            {
                return Result<AuthenticationTokenDto>.Fail(new Error("User.EmailExists", $"{email} is already tied to an existing account."));
            }

            var maybeNewUser = await _userRepository.GetByEmail(normalizedEmail, ct);

            if (maybeNewUser == null)
                return Result<AuthenticationTokenDto>.Fail(new Error("User.RegisterFail", $"Something went wrong and your account was not created."));

            var authenticationTokenDto = IssueJwt(maybeNewUser.Id.ToString(), maybeNewUser.Email, _configuration);

            return Result<AuthenticationTokenDto>.Ok(authenticationTokenDto);
        }

        private static string NormalizeEmail(string email)
        {
            return (email ?? string.Empty).Trim().ToLowerInvariant();
        }

        public static bool IsEmailValid(string email)
        {
            return new EmailAddressAttribute().IsValid(email);
        }

        private static bool IsPasswordValid(string password, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "Password cannot be empty.";
                return false;
            }

            if (password.Length < 8)
            {
                errorMessage = "Password must be at least 8 characters long.";
                return false;
            }

            if (!password.Any(char.IsUpper))
            {
                errorMessage = "Password must contain at least one uppercase letter.";
                return false;
            }

            if (!password.Any(char.IsLower))
            {
                errorMessage = "Password must contain at least one lowercase letter.";
                return false;
            }

            if (!password.Any(char.IsDigit))
            {
                errorMessage = "Password must contain at least one digit.";
                return false;
            }

            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                errorMessage = "Password must contain at least one special character.";
                return false;
            }

            return true;
        }

        private static AuthenticationTokenDto IssueJwt(string userId, string userEmail, IConfiguration config)
        {
            var issuer = config["Jwt:Issuer"]!;
            var audience = config["Jwt:Audience"]!;
            var keyBytes = Encoding.UTF8.GetBytes(config["Jwt:Key"]!);
            var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

            var now = DateTime.UtcNow;
            var expires = now.AddDays(10);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Email, userEmail),
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: creds);

            var raw = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthenticationTokenDto { AccessToken = raw, ExpiresAtUtc = expires };
        }
    }
}
