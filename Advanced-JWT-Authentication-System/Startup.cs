using Advanced_JWT_Authentication_System.Interfaces;
using Advanced_JWT_Authentication_System.Models.Db;
using Advanced_JWT_Authentication_System.Repositories;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Advanced_JWT_Authentication_System
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
            // Configure Entity Framework to use MySQL
            services.AddDbContext<authenticationContext>(options =>
                options.UseMySql(
                    Configuration.GetConnectionString("AuthenticationDefaultConnection"),
                    new MySqlServerVersion(new Version(5, 5, 62)) // Specify the version of MySQL you're using
                ));
            services.AddControllers();
            // Add Razor Pages services
            services.AddRazorPages();

            // JWT Authentication configuration
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key"))
                };
            })
            .AddGoogle(options =>
            {
                options.ClientId = Configuration["Google:ClientId"]; // Get from appsettings.json or environment variables
        options.ClientSecret = Configuration["Google:ClientSecret"]; // Get from appsettings.json or environment variables
        options.CallbackPath = new PathString("/signin-google"); // You can customize this if needed
    });
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Advanced_JWT_Authentication_System", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Advanced_JWT_Authentication_System v1");
                    c.RoutePrefix = "api-docs"; // Swagger will now be available at /swagger instead of the default page
                });
            }

            app.UseHttpsRedirection();


            // Middleware to force redirect root URL ("/") to "/Authentication/RegistrationPage"
            app.Use(async (context, next) =>
            {
                // Redirect only if the request is for the root ("/") or Swagger
                if (context.Request.Path == "/" || context.Request.Path.StartsWithSegments("/swagger"))
                {
                    context.Response.Redirect("/Authentication/RegistrationPage", false);
                    return;
                }
                await next();
            });
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();  // map Razor Pages             
            });
        }
    }
}
