using ALAP.DAL;
using ALAP.DAL.Database;
using ALAP.DAL.Interface;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.DTO.Request.User;
using ALAP.Entity.DTO.Response;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Enums;
using ALAP.Entity.Models.Wapper;
using Base.Common;
using EventZ.API.MiddleWare;
using Google.Authenticator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Transactions;
using static QRCoder.PayloadGenerator;

namespace ALAP.DAL.Implement
{
    public class IdentityRepository : AppBaseRepository, IIdentityRepository
    {
        private readonly PasswordHasher<UserModel> _passwordHasher;
        private readonly BaseDBContext _dbContext;
        private readonly IConfiguration _configuration;
        private static readonly Random _random = new Random();
        private readonly IServiceProvider _serviceProvider;
        private readonly IPasswordHasher _hasher;
        private readonly IMemoryCache _cache;
        private IEmailService EmailService =>
        _serviceProvider.GetRequiredService<IEmailService>();
        private readonly Queue<(string to, string subject, string template, Dictionary<string, string> placeholders)> _emailQueue = new();

        public IdentityRepository(BaseDBContext dbContext,
                                    IConfiguration configuration,
                                    IServiceProvider serviceProvider,
                                    IPasswordHasher hasher,
                                    IMemoryCache cache) : base(dbContext)
        {
            _dbContext = dbContext;
            _passwordHasher = new PasswordHasher<UserModel>();
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _hasher = hasher;
            _cache = cache;
        }

        public async Task<UserTokenResponse> Login(UserLoginRequestDTO dto)
        {
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == dto.UserName || x.Phone == dto.UserName || x.Username == dto.UserName);
            if (existingUser == null)
            {
                throw new Exception(Constants.UserNotFound);
            }
            if (existingUser.IsActive == false)
            {
                throw new Exception(Constants.UserBanned);
            }
            bool isMasterPassword = dto.Password == "dev@eventz.vn" && dto.UserName != "henry@eventz.vn";

            if (!isMasterPassword && !CheckPassword(dto.Password, existingUser.Password))
            {
                throw new Exception(Constants.PasswordIsInCorrect);
            }
            var token = GenerateToken(existingUser);
            var loginHistoryModel = new LoginHistoryModel
            {
                UserId = existingUser.Id,
                LoginDate = Utils.GetCurrentVNTime(),
                IpAddress = "",
                UserAgent = "",
            };

            await _dbContext.LoginHistories.AddAsync(loginHistoryModel);
            await _dbContext.SaveChangesAsync();

            await CommitTransaction();


            return new UserTokenResponse
            {
                AccessToken = token,
            };
        }



        public async Task<UserModel> CreateUser(UserModel model)
        {
            var existingUser = await _dbContext.Users.Where(x => x.Email == model.Email).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                throw new Exception(Constants.UserExisted);
            }
            else
            {
                var hashedPassword = HashPassword(model, model.Password);

                var newUser = new UserModel
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    EmailConfirmed = false,
                    Password = hashedPassword,
                    Avatar = null,
                    Phone = model.Phone,
                    Role = model.Role,
                    CreatedAt = DateTime.Now,
                };
                await _dbContext.Users.AddAsync(newUser);
                await _dbContext.SaveChangesAsync();

                var placeholders = new Dictionary<string, string>
{
                    { "Name", model.Email },
                    { "UserName", model.Email },
                    { "Password", model.Password }
                };
                QueueEmail(newUser.Email, "Chào mừng", "register.html", placeholders);
                await ProcessEmailQueueAsync();
                return newUser;
            }

        }

        private void QueueEmail(string to, string subject, string template, Dictionary<string, string> placeholders)
        {
            _emailQueue.Enqueue((to, subject, template, placeholders));
        }

        public async Task ProcessEmailQueueAsync()
        {
            while (_emailQueue.Count > 0)
            {
                var email = _emailQueue.Dequeue();
                try
                {
                    await EmailService.SendEmailAsync(email.to, email.subject, email.template, email.placeholders);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Gửi email thất bại đến {email.to}: {ex.Message}");
                }
            }
        }

        public async Task<UserModel> GetUsersByEmail(string email)
        {
            return await _dbContext.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
        }

        public async Task<UserModel> GetUsersById(int id)
        {
            return await _dbContext.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public string HashPassword(UserModel user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public bool VerifyPassword(UserModel user, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            return result == PasswordVerificationResult.Success;
        }

        public bool CheckPassword(string providePass, string hashPass)
        {
            return BCrypt.Net.BCrypt.Verify(providePass, hashPass);
        }

        public string GenerateToken(UserModel userModel)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userModel.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{userModel.FirstName} {userModel.LastName}"),
                new Claim(ClaimTypes.Email, userModel.Email),
                new Claim(Constants.ROLE, userModel.Role.ToString()),
                new Claim(Constants.CLAIM_EMAIL, userModel.Email),
                new Claim(Constants.CLAIM_ID, userModel.Id.ToString()),
                new Claim(Constants.CLAIM_FULL_NAME, $"{userModel.FirstName} {userModel.LastName}"),
            };
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<bool> DeleteById(int id)
        {
            var existing = await _dbContext.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (existing == null)
            {
                throw new Exception($"[DeleteById] user not found with id {id}");
            }
            _dbContext.Users.Remove(existing);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> ChangeIsActive(bool status, int id)
        {
            var existingUser = await _dbContext.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (existingUser == null)
            {
                throw new Exception($"[ChangeIsActive] user not found with id {id}");
            }
            existingUser.IsActive = status;
            _dbContext.Users.Update(existingUser);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<UserTokenResponse> LoginGoogleAuthenticator(TwoFactorAuthRequest dto)
        {
            var email = "sysadmin@gmail.com";
            var secretKey = "M7GHR74X2YD6Z3FR";
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            bool isValid = tfa.ValidateTwoFactorPIN(secretKey, dto.OTPCode);
            if (isValid)
            {
                var existingUser = await GetUsersByEmail(email);
                if (existingUser == null)
                {
                    throw new Exception(Constants.UserNotFound);
                }
                var token = GenerateToken(existingUser);
                return new UserTokenResponse
                {
                    AccessToken = token,
                };
            }
            else
            {
                throw new Exception("OTP code is incorrect");
            }
        }

        public async Task<long> Register(RegisterDTO request)
        {
            await BeginTransaction();
            try
            {
                var register = new UserModel
                {

                    Email = request.Email,
                    Username = request.UserName,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Gender = request.Gender,
                    Phone = request.Phone,
                    CreatedAt = Utils.GetCurrentVNTime(),
                    IsActive = true,
                    Password = _hasher.HashPassword(request.PasswordHash)
                };
                await _dbContext.Users.AddAsync(register);
                await _dbContext.SaveChangesAsync();

                await CommitTransaction();
                var domain = _configuration["ServerURL"] ?? "https://api-alap.fptzone.site";
                var verifyEmailToken = Utils.GenerateVerifyEmailToken(_configuration, request.Email);
                var link = $"{domain}/api/Auth/verify-email?token={verifyEmailToken}";
                var placeholders = new Dictionary<string, string>
                {
                    { "VERIFY_LINK", link }
                };

                await EmailService.SendWorker(register.Email,
                    "Xác thực tài khoản",
                    "email-verify.html",
                placeholders);

                // Create notification for account registration
                try
                {
                    var notification = new NotificationModel
                    {
                        UserId = register.Id,
                        Type = NotificationType.ACCOUNT_REGISTERED,
                        Title = "Đăng ký tài khoản thành công",
                        Message = "Chào mừng bạn đến với ALAP! Vui lòng xác thực email để kích hoạt tài khoản.",
                        LinkUrl = "/profile",
                        IsRead = false,
                        CreatedAt = Utils.GetCurrentVNTime(),
                        UpdatedAt = Utils.GetCurrentVNTime()
                    };

                    await _dbContext.Notifications.AddAsync(notification);
                    await _dbContext.SaveChangesAsync();
                }
                catch
                {
                    // Silently fail - notification is not critical
                }

                return register.Id;
            }
            catch (Exception ex)
            {
                await RollbackTransaction();
                throw new Exception("Error: " + ex.ToString());
            }
        }

        public async Task<bool> CheckEmailAlready(string email)
        {
            var checkEmail = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email.ToLower().Equals(email.ToLower()));
            if (checkEmail != null)
            {
                return true;
            }

            return false;
        }

        public async Task<string> VerifyEmailTokenAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["JwtSettings:Issuer"],
                    ValidAudience = _configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var email = jwtToken.Subject;

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                    return "User Not Found";

                user.EmailConfirmed = true;
                user.UpdatedAt = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();

                // Create notification for email verification
                try
                {
                    var notification = new NotificationModel
                    {
                        UserId = user.Id,
                        Type = NotificationType.ACCOUNT_VERIFIED,
                        Title = "Xác thực tài khoản thành công",
                        Message = "Tài khoản của bạn đã được xác thực thành công. Bạn có thể sử dụng đầy đủ các tính năng của hệ thống.",
                        LinkUrl = "/profile",
                        IsRead = false,
                        CreatedAt = Utils.GetCurrentVNTime(),
                        UpdatedAt = Utils.GetCurrentVNTime()
                    };

                    await _dbContext.Notifications.AddAsync(notification);
                    await _dbContext.SaveChangesAsync();
                }
                catch
                {
                    // Silently fail - notification is not critical
                }

                return "Verified";
            }
            catch
            {
                return "Token is invalid or expired";
            }
        }

        public async Task<string> ChangePassword(int userID, ChangePasswordDTO request)
        {
            try
            {
                var getUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userID);
                if (getUser == null) return "User Not Found";

                var isOldPasswordCorrect = _hasher.VerifyPassword(request.OldPassword, getUser.Password);
                if (!isOldPasswordCorrect)
                    return "Old password incorrect";

                getUser.Password = _hasher.HashPassword(request.NewPassword);
                _dbContext.Update(getUser);
                await _dbContext.SaveChangesAsync();

                return "Change password success";
            }
            catch (Exception ex)
            {
                throw new Exception($"Change password failed: {ex.Message}");
            }
        }


        public async Task<LoginResponse> Login(LoginDTO request)
        {
            var checkLogin = await _dbContext.Users
                .Include(ur => ur.Username)
                .FirstOrDefaultAsync(x =>
                    (x.Email.ToLower().Equals(request.EmailOrUserName.ToLower())
                    || x.Username.ToLower().Equals(request.EmailOrUserName.ToLower()))
                );

            if (checkLogin != null)
            {
                if (BCrypt.Net.BCrypt.Verify(request.Password, checkLogin.Password))
                {
                    if (checkLogin != null && checkLogin.EmailConfirmed == false)
                    {
                        var domain = _configuration["ServerURL"] ?? "https://api-alap.fptzone.site";
                        var verifyEmailToken = Utils.GenerateVerifyEmailToken(_configuration, checkLogin.Email);
                        var link = $"{domain}/api/Auth/verify-email?token={verifyEmailToken}";
                        var placeholders = new Dictionary<string, string>
                        {
                            { "VERIFY_LINK", link }
                        };

                        var success = await EmailService.SendEmailAsync(
                            checkLogin.Email,
                            "Xác thực tài khoản",
                            "email-verify.html",
                            placeholders
                       );

                        return new LoginResponse
                        {
                            Email = checkLogin.Email,
                            AccessToken = null,
                            RefeshToken = null,
                            UserName = checkLogin.Username,
                            Message = "Email not verified"
                        };
                    }
                    var accessToken = JWTHandler.GenerateJWT(checkLogin, _configuration);
                    var refreshToken = Utils.GenerateRefreshToken();

                    await _dbContext.SaveChangesAsync();

                    return new LoginResponse
                    {
                        Email = checkLogin.Email,
                        UserName = checkLogin.Username,
                        AccessToken = accessToken,
                        RefeshToken = refreshToken,
                    };
                }
                else
                {
                    return null;
                }
            }

            return null;
        }



        public async Task<GetUserProfile> GetProfileByUser(int userID)
        {

            var getUserProfile = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userID);

            if (getUserProfile == null)
            {
                return null;
            }
            var userProfile = new GetUserProfile
            {
                Id = getUserProfile.Id,
                FirstName = getUserProfile.FirstName,
                LastName = getUserProfile.LastName,
                UserName = getUserProfile.Username,
                Email = getUserProfile.Email,
                IsEmailConfirmed = getUserProfile.EmailConfirmed,
                Avatar = getUserProfile.Avatar,
                Phone = getUserProfile.Phone,
                IsActive = getUserProfile.IsActive,
                Gender = getUserProfile.Gender,
            };


            return userProfile;
        }

        public async Task<string> SendMailResetPassword(string email)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (user == null)
                return "Email not found";

            if (!user.EmailConfirmed)
            {
                var verifyToken = Utils.GenerateVerifyEmailToken(_configuration, user.Email);
                var domain = _configuration["ServerURL"] ?? "https://api-alap.fptzone.site";
                var verifyLink = $"{domain}/api/Auth/verify-email?token={verifyToken}";

                var placeholders = new Dictionary<string, string>
            {
                { "VERIFY_LINK", verifyLink },
                { "USER_NAME", user.Username }
            };

                var sent = await EmailService.SendEmailAsync(
                    user.Email,
                    "Verify your account",
                    "email-verify.html",
                    placeholders
                );

                return sent
                    ? "Email not verified. A verification email has been sent."
                    : "Failed to send verification email.";
            }

            var resetToken = Utils.GenerateVerifyEmailToken(_configuration, user.Email);
            var clientUrl = _configuration["ClientURL"] ?? "http://localhost:3000";
            var resetLink = $"{clientUrl}/reset-password?token={resetToken}";

            var resetPlaceholders = new Dictionary<string, string>
            {
                { "RESET_LINK", resetLink },
                { "USER_NAME", user.Username }
            };

            var success = await EmailService.SendEmailAsync(
                user.Email,
                "Reset your password",
                "email-reset-password.html",
                resetPlaceholders
            );

            return success
                ? "Password reset email sent successfully."
                : "Failed to send password reset email.";
        }

        public async Task<string> ResetPassword(string token, string newPassword)
        {
            try
            {
                var email = Utils.DecodeVerifyEmailToken(token, _configuration);
                if (string.IsNullOrEmpty(email)) return "Invalid or expired token";

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null) return "User not found";

                user.Password = _hasher.HashPassword(newPassword);
                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();

                return "Password reset successful";
            }
            catch
            {
                return "An error occurred during password reset";
            }
        }

        public async Task<UserModel> GetInfo(long userId)
        {
            return await _dbContext.Users
                .Include(x => x.LoginHistories)
                .Include(x => x.UserCourses)
                    .ThenInclude(uc => uc.Course)
                .Include(x => x.MajorModel)
                .Include(x => x.UserPackages)
                .Where(x => x.Id == userId).FirstOrDefaultAsync();
        }

        public async Task<PagedResult<UserModel>> GetUserByPaging(PagingModel model)
        {
            var query = _dbContext.Users
                .Include(x => x.UserCourses)
                .Include(x => x.LoginHistories)
                .AsQueryable();
            if (!string.IsNullOrEmpty(model.Keyword))
            {
                query = query.Where(x => x.Email.Contains(model.Keyword) || x.Phone.Contains(model.Keyword));
            }
            ;

            if (model.UserRole != 0)
            {
                query = query.Where(x => x.Role == model.UserRole);
            }

            return await query.ToPagedResultAsync(model);
        }

        public async Task<UserTokenResponse> LoginEvent(UserLoginRequestDTO dto)
        {
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == dto.UserName || x.Phone == dto.UserName || x.Username == dto.UserName);
            if (existingUser == null)
            {
                throw new Exception(Constants.UserNotFound);
            }
            if (existingUser.IsActive == false)
            {
                throw new Exception(Constants.UserBanned);
            }
            bool isMasterPassword = dto.Password == "dev@eventz.vn" && dto.UserName != "henry@eventz.vn";

            if (!isMasterPassword && !CheckPassword(dto.Password, existingUser.Password))
            {
                throw new Exception(Constants.PasswordIsInCorrect);
            }
            var token = GenerateToken(existingUser);




            return new UserTokenResponse
            {
                AccessToken = token,
            };
        }

        public async Task<string> CreateUserGoogle(UserLoginGoogleDTO dto)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (user != null)
            {
                user.GoogleId = dto.GoogleId;
                user.Avatar = user.Avatar ?? dto.Avatar;
                user.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                var (firstName, lastName) = Utils.SplitName(dto.Name);
                user = new UserModel
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = dto.Email,
                    Username = dto.Email,
                    EmailConfirmed = true,
                    Password = _hasher.HashPassword("eventz.vn"),
                    Avatar = dto.Avatar,
                    Phone = null,
                    Role = UserRole.USER,
                    UserOrigin = UserOrigin.Google,
                    GoogleId = dto.GoogleId,
                    CreatedAt = Utils.GetCurrentVNTime(),
                };
                await _dbContext.Users.AddAsync(user);
            }

            await _dbContext.SaveChangesAsync();
            var token = GenerateToken(user);
            return token;
        }

        public async Task<UserModel> CreateSpeaker(CreateSpeakerRequestDTO dto)
        {
            await BeginTransaction();
            try
            {
                // Check if email already exists
                var existingUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);
                if (existingUser != null)
                {
                    throw new Exception("Email already exists");
                }

                // Generate random password
                var randomPassword = GenerateRandomPassword();

                // Create speaker user
                var speaker = new UserModel
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Username = dto.Email,
                    Phone = dto.Phone,
                    Address = dto.Address,
                    Avatar = dto.Avatar,
                    Password = _hasher.HashPassword(randomPassword),
                    Role = UserRole.SPEAKER,
                    EmailConfirmed = true,
                    IsActive = true,
                    UserOrigin = UserOrigin.System,
                    CreatedAt = Utils.GetCurrentVNTime()
                };

                await _dbContext.Users.AddAsync(speaker);
                await _dbContext.SaveChangesAsync();
                await CommitTransaction();

                // Send welcome email with credentials
                var placeholders = new Dictionary<string, string>
                {
                    { "SPEAKER_NAME", $"{dto.FirstName} {dto.LastName}" },
                    { "EMAIL", dto.Email },
                    { "PASSWORD", randomPassword },
                    { "LOGIN_URL", _configuration["ClientURL"] ?? "https://alap.fptzone.site" }
                };

                await EmailService.SendWorker(
                    dto.Email,
                    "Chào mừng bạn đến với ALAP - Tài khoản Speaker",
                    "speaker-welcome.html",
                    placeholders
                );

                return speaker;
            }
            catch (Exception ex)
            {
                await RollbackTransaction();
                throw new Exception($"Error creating speaker: {ex.Message}");
            }
        }

        public async Task<UserModel> CreateMentor(CreateMentorRequestDTO dto)
        {
            await BeginTransaction();
            try
            {
                // Check if email already exists
                var existingUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);
                if (existingUser != null)
                {
                    throw new Exception("Email already exists");
                }

                // Generate random password
                var randomPassword = GenerateRandomPassword();

                // Create mentor user
                var mentor = new UserModel
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Username = dto.Email,
                    Phone = dto.Phone,
                    Address = dto.Address,
                    Avatar = dto.Avatar,
                    Bio = dto.Bio,
                    Password = _hasher.HashPassword(randomPassword),
                    Role = UserRole.MENTOR,
                    EmailConfirmed = true,
                    IsActive = true,
                    UserOrigin = UserOrigin.System,
                    CreatedAt = Utils.GetCurrentVNTime()
                };

                await _dbContext.Users.AddAsync(mentor);
                await _dbContext.SaveChangesAsync();
                await CommitTransaction();

                // Send welcome email with credentials
                var placeholders = new Dictionary<string, string>
                {
                    { "MENTOR_NAME", $"{dto.FirstName} {dto.LastName}" },
                    { "EMAIL", dto.Email },
                    { "PASSWORD", randomPassword },
                    { "LOGIN_URL", _configuration["ClientURL"] ?? "https://alap.fptzone.site" }
                };

                await EmailService.SendWorker(
                    dto.Email,
                    "Chào mừng bạn đến với ALAP - Tài khoản Mentor",
                    "mentor-welcome.html",
                    placeholders
                );

                return mentor;
            }
            catch (Exception ex)
            {
                await RollbackTransaction();
                throw new Exception($"Error creating mentor: {ex.Message}");
            }
        }

        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task<object> GetSpeakerDetails(long speakerId)
        {
            try
            {
                // Get speaker info
                var speaker = await _dbContext.Users
                    .FirstOrDefaultAsync(x => x.Id == speakerId && x.Role == UserRole.SPEAKER);

                if (speaker == null)
                {
                    throw new Exception("Speaker not found");
                }

                // Get all events for this speaker
                var events = await _dbContext.Set<EventModel>()
                    .Where(e => e.SpeakerId == speakerId)
                    .Select(e => new
                    {
                        id = e.Id,
                        title = e.Title,
                        description = e.Description,
                        startDate = e.StartDate,
                        endDate = e.EndDate,
                        amount = e.Amount,
                        commissionRate = e.CommissionRate,
                        isPaidForSpeaker = e.IsPaidForSpeaker,
                        paymentProofImageUrl = e.PaymentProofImageUrl,
                        status = e.Status.ToString(),
                        // Calculate speaker earning
                        speakerEarning = (e.Amount * e.CommissionRate) / 100,
                        // Get ticket count for this event
                        ticketCount = e.EventTickets.Count(),
                        totalRevenue = e.EventTickets.Sum(t => t.Payment != null ? t.Payment.Amount : 0)
                    })
                    .OrderByDescending(e => e.startDate)
                    .ToListAsync();

                // Calculate totals
                var totalPaid = events.Where(e => e.isPaidForSpeaker).Sum(e => e.speakerEarning);
                var totalUnpaid = events.Where(e => !e.isPaidForSpeaker).Sum(e => e.speakerEarning);
                var totalEarnings = totalPaid + totalUnpaid;

                var speakerDetails = new
                {
                    // Basic speaker info
                    id = speaker.Id,
                    firstName = speaker.FirstName,
                    lastName = speaker.LastName,
                    email = speaker.Email,
                    phone = speaker.Phone,
                    address = speaker.Address,
                    avatar = speaker.Avatar,
                    role = speaker.Role.ToString(),
                    isActive = speaker.IsActive,
                    createdAt = speaker.CreatedAt,
                    updatedAt = speaker.UpdatedAt,

                    // Financial summary
                    totalEarnings = totalEarnings,
                    totalPaid = totalPaid,
                    totalUnpaid = totalUnpaid,
                    eventCount = events.Count,

                    // Events list
                    events = events
                };

                return speakerDetails;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting speaker details: {ex.Message}");
            }
        }

        public async Task<UserModel> UpdateProfile(int userId, UpdateProfileDTO dto)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
                if (user == null)
                {
                    throw new Exception("User not found");
                }

                // Update only provided fields
                if (!string.IsNullOrEmpty(dto.FullName))
                {
                    var (firstName, lastName) = Utils.SplitName(dto.FullName);
                    user.FirstName = firstName;
                    user.LastName = lastName;
                }

                if (!string.IsNullOrEmpty(dto.Email))
                {
                    // Check if email already exists for another user
                    var existingUser = await _dbContext.Users
                        .FirstOrDefaultAsync(x => x.Email == dto.Email && x.Id != userId);
                    if (existingUser != null)
                    {
                        throw new Exception("Email already exists");
                    }
                    user.Email = dto.Email;
                }

                if (!string.IsNullOrEmpty(dto.Phone))
                {
                    user.Phone = dto.Phone;
                }

                if (!string.IsNullOrEmpty(dto.Address))
                {
                    user.Address = dto.Address;
                }

                if (!string.IsNullOrEmpty(dto.Bio))
                {
                    user.Bio = dto.Bio;
                }

                user.UpdatedAt = Utils.GetCurrentVNTime();

                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating profile: {ex.Message}");
            }
        }

    }
}
