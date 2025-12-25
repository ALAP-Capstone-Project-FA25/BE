using ALAP.BLL.Helper;
using ALAP.BLL.BackgroundServices;
using ALAP.DAL.Database;
using ALAP.Entity.Models.Enums;
using EventZ.API.MiddleWare;
using EventZ.Mappings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PayOS;
using Serilog;
using System.Security.Claims;
using System.Text;
using ALAP.BLL.BackgroundServices;
using ALAP.BLL.Helper;
using ALAP.DAL.Database;
using ALAP.Entity.Models.Enums;
using static QRCoder.PayloadGenerator;
namespace EventZ.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Serilog.Debugging.SelfLog.Enable(msg =>
            {
                File.AppendAllText("serilog-selflog.txt", msg);
            });

            var builder = WebApplication.CreateBuilder(args);

            // === Serilog ===
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();
            builder.Host.UseSerilog();

            // === VNPAY ===
            var vnPaySection = builder.Configuration.GetSection("VNPayConfig");

            // === DbContext ===
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<BaseDBContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                    b => b.MigrationsAssembly("Main.API")));

            // === Dependency Injection ===
            DependencyConfig.DependencyConfig.Register(builder.Services);

            // === AutoMapper ===
            builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            // === CORS ===
            var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy
                        .WithOrigins(allowedOrigins ?? new string[] { })
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            // === SMTP ===
            builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SMTP"));

            // === Background Services Configuration ===
            builder.Services.Configure<EventSchedulerOptions>(builder.Configuration.GetSection(EventSchedulerOptions.SectionName));
            builder.Services.Configure<EmailWorkerOptions>(builder.Configuration.GetSection(EmailWorkerOptions.SectionName));

            // === Background Services ===
            builder.Services.AddHostedService<EventStatusSchedulerService>();
            builder.Services.AddHostedService<EmailSenderWorkerService>();

            // === Swagger + JWT Support ===
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "ALAP", Version = "v1" });

                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Nh?p token theo ??nh d?ng: Bearer {token}",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            // === Authentication Configuration ===
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!)
                    )
                };
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
            })
            .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                var googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");

                options.ClientId = googleAuthNSection["ClientId"];
                options.ClientSecret = googleAuthNSection["ClientSecret"];
                options.CallbackPath = "/signin-google";

                options.Scope.Add("profile");
                options.ClaimActions.MapJsonKey("picture", "picture", "url");
                options.SaveTokens = true;

                options.Events.OnCreatingTicket = ctx =>
                {
                    var picture = ctx.User.GetProperty("picture").GetString();
                    ctx.Identity?.AddClaim(new Claim("picture", picture ?? ""));
                    return Task.CompletedTask;
                };
            });

            builder.Services.AddSingleton(sp =>
            {
                var cfg = builder.Configuration.GetSection("PayOS");
                return new PayOsClient(
                    apiMerchantHost: cfg["ApiMerchantHost"]!,
                    clientId: cfg["ClientId"]!,
                    apiKey: cfg["ApiKey"]!,
                    checksumKey: cfg["ChecksumKey"]!
                );
            });

            builder.Services.AddSingleton(sp =>
            {
                var clientId = builder.Configuration["PayOS:ClientId"];
                var apiKey = builder.Configuration["PayOS:ApiKey"];
                var checksumKey = builder.Configuration["PayOS:ChecksumKey"];
                return new PayOS.PayOSClient(clientId, apiKey, checksumKey);
            });

            builder.Services.Configure<EmailWorkerOptions>(
            builder.Configuration.GetSection(EmailWorkerOptions.SectionName));

            // === HttpClient for Meet API ===
            builder.Services.AddHttpClient<ALAP.API.Controllers.MeetController>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("User-Agent", "ALAP-MeetGenerator/1.0");
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
            {
                AllowAutoRedirect = false // Quan tr?ng: không t? ??ng follow redirect
            });

            // === MVC + Swagger ===
            builder.Services.AddControllers();
            builder.Services.AddSignalR();
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            app.UseSerilogRequestLogging();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseCors("CorsPolicy");

            // === Static Files ===
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                if (context.Request.Method == "OPTIONS")
                {
                    context.Response.StatusCode = 200;
                    await context.Response.CompleteAsync();
                    return;
                }
                await next();
            });

            // === Test logging to Telegram ===
            app.MapGet("/test-telegram-error", () =>
            {
                Log.Error("?ây là l?i th? nghi?m g?i v? Telegram");
                throw new Exception("?ây là Exception test g?i v? Telegram");
            });

            app.MapControllers();
            app.MapHub<ALAP.API.Hubs.ChatHub>("/hubs/chat");

            app.Run();
        }
    }
}
