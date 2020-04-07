using System.Text;
using Ng.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Services.UserServ;
using myAuthExampleApi.Repositories;
using myAuthExampleApi.Models.DbModels;
using System.Globalization;

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
            /////////////////////
            //Add own services
            /////////////////////
            
            services
                .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>()
                .AddScoped<ISimpleTokenRepository, SimpleTokenRepository>()
                .AddScoped<IUserRepository, UserRepository>();
            //Add services with their own ServiceCollection extensions
            services
                .AddUserService()
                .AddPasswordHashingService()
                .AddEmailService(options =>
                {
                    options.Host = Configuration["EmailSettings:Host"];
                })
                .AddSimpleTokenService(options => options.TokenExpirationInMinutes = 1440);

            ////////////////////////////
            //Add external services
            ////////////////////////////

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
            //Add JWT Token Services and settings
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Configuration["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = Configuration["JwtSettings:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSettings:Key"])),
                ValidateLifetime = true,
                SaveSigninToken = true,
            };
            services
                .AddJwtTokenService(options => {
                    options.SecurityAlgorithm = SecurityAlgorithm.HS256;
                    options.AccessTokenExpirationInMinutes = int.Parse(Configuration["JwtSettings:AccessTokenExpirationInMinutes"], CultureInfo.InvariantCulture); //Default: 60
                    options.RefreshTokenExpirationInHours = int.Parse(Configuration["JwtSettings:RefreshTokenExpirationInHours"], CultureInfo.InvariantCulture); //Default: 2
                    options.TokenValidationParameters = tokenValidationParameters;
                });
            //Configure JWT Authentication
            services
                .AddAuthentication(options => {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options => options.TokenValidationParameters = tokenValidationParameters);
            //Add Support for controllers
            services
                .AddControllers()
                .SetCompatibilityVersion(CompatibilityVersion.Latest);
        }

        //HTTP request pipeline
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
