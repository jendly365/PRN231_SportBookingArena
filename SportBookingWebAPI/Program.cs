using AutoMapper;
using BusinessObject.Models;
using DataAccess;
using DataAccess.Repository.impl;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SportBookingWebAPI.Mappers;
using System.Text;

namespace SportBookingWebAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

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

			// C?u hình AutoMapper
			var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new MapperConfig()));
			IMapper mapper = mapperConfig.CreateMapper();
			builder.Services.AddSingleton(mapper);

			// C?u hình DI cho Repository
			builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

			builder.Services.AddControllers();
			// C?u hình Database
			builder.Services.AddDbContext<EXE201_Rental_Sport_Field1Context>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("value")));

			// C?u hình Swagger v?i JWT Authentication
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo
				{
					Title = "SportBooking API",
					Version = "v1",
					Description = "API for Sport Booking System"
				});
				// Thêm c?u hình b?o m?t JWT vào Swagger UI
				c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Name = "Authorization",
					Type = SecuritySchemeType.Http,
					Scheme = "Bearer",
					BearerFormat = "JWT",
					In = ParameterLocation.Header,
					Description = "Nh?p 'Bearer' [space] + token vào ô bên d??i.\n\nVí d?: **Bearer your_token_here**"
				});

				c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

			// C?u hình CORS
			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowAllOrigins",
					policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
			});

			var app = builder.Build();

			// Middleware cho Swagger
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}
            app.UseStaticFiles(); // ph?i có dòng này

            app.UseCors("AllowAllOrigins");
			app.UseHttpsRedirection();

			// Middleware Authentication & Authorization
			app.UseAuthentication();
			app.UseAuthorization();
			app.MapControllers();
			app.Run();
		}
	}
}
