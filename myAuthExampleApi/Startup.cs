using System.Text;
using Services.JwtTokenService;
using Services.SimpleTokenService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using myAuthExampleApi.Models;
using Services.PasswordHashingService;
using Services.EmailService;
using Services.UserService;
using myAuthExampleApi.Repositories;
using System;

namespace myAuthExampleApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Add services
            services
                .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>()
                .AddScoped<ISimpleTokenRepository, SimpleTokenRepository>()
                .AddScoped<IUserRepository, UserRepository>();
            //Add services with their own ServiceCollection extensions
            services
                .AddUserService()
                .AddPasswordHashingService()
                .AddEmailService(options => {
                    options.Domain = "erasmusmc.nl";
                    options.Host = "mail.erasmusmc.nl";
                })
                .AddSimpleTokenService(options => options.TokenExpirationInMinutes = 1440)
                .AddJwtTokenService(Configuration.GetSection("JwtSettings"));
            //Configure database connection
            services.AddDbContext<MyAuthExampleContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Database_prod")));
            //Configure CORS
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .WithOrigins(new string[] { "http://localhost:4200", "https://localhost:4200" })
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders(new string[] { "Content-Disposition", "WWW-Authenticate" })
                    .AllowCredentials());
            });
            //Configure JWT Auth
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ClockSkew = new TimeSpan(0, 0, 60), //Default is 300. After the token is expired, you have 60 seconds extra time to allow for some clock skew.
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        SaveSigninToken = true,
                        ValidIssuer = Configuration["JwtSettings:Issuer"],
                        ValidAudience = Configuration["JwtSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSettings:Key"])),
                    };
                });
            //Add Support for controllers
            services
                .AddControllers()
                .SetCompatibilityVersion(CompatibilityVersion.Latest);
        }

        //HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler("/error");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
