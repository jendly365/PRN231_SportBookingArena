using AutoMapper;
using BusinessObject.Models;
using DataAccess.Repository.impl;
using DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SportBookingWebClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
			builder.Services.AddHttpClient();
            builder.Services.AddSession();
            builder.Services.AddHttpContextAccessor();
            // ??c giá tr? t? appsettings.json
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
			var secretKey = jwtSettings["SecretKey"];

			// C?u hình JWT Authentication
			builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						ValidIssuer = jwtSettings["Issuer"],
						ValidAudience = jwtSettings["Audience"],
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
					};
				});

	

			// C?u hình DI cho Repository
			builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

			builder.Services.AddControllers();

			// C?u hình Database
			builder.Services.AddDbContext<EXE201_Rental_Sport_Field1Context>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("value")));

			// C?u hình Swagger
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
			builder.Services.AddDistributedMemoryCache();
			builder.Services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromMinutes(30); // Th?i gian t?n t?i session
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});
			// C?u hình CORS
			builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });

            var app = builder.Build();
         

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
			app.UseCors(policy =>
	policy.AllowAnyOrigin()
		  .AllowAnyMethod()
		  .AllowAnyHeader());
			app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();
            app.UseCors("AllowAll");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}