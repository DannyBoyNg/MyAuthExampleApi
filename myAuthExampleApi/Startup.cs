using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using myAuthExampleApi.Models;
using myAuthExampleApi.Repositories;
using Ng.Services;
using Services.UserServ;
using System.Globalization;
using System.Text;

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
                });
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
            //Configure JWT Token Auth
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
            services.AddJwtTokenService(options =>
            {
                options.SecurityAlgorithm = SecurityAlgorithm.HS256;
                options.AccessTokenExpirationInMinutes = int.Parse(Configuration["JwtSettings:AccessTokenExpirationInMinutes"], CultureInfo.InvariantCulture); //Default: 60
                options.RefreshTokenExpirationInHours = int.Parse(Configuration["JwtSettings:RefreshTokenExpirationInHours"], CultureInfo.InvariantCulture); //Default: 2
                options.TokenValidationParameters = tokenValidationParameters;
            });
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => options.TokenValidationParameters = tokenValidationParameters);
            //Add Support for controllers
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "myAuthExampleApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "myAuthExampleApi v1"));
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
