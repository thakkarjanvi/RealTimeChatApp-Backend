using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RealTimeChatApp.DAL.Context;
using RealTimeChatApp.DAL.Repository;
using RealTimeChatApp.DAL.Services;
using RealTimeChatApp.Domain.Interfaces;
using RealTimeChatApp.Domain.Models;
using RealTimeChatApp.Hubs;
using RealTimeChatApp.Middleware;
using System.Text;

namespace RealTimeChatApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddIdentity<User, IdentityRole>()
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();

            //builder.Services.AddDataProtection()
            //    .PersistKeysToDbContext<ApplicationDbContext>()
            //    .SetApplicationName("RealTimeChatApp");

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IMessageService, MessageService>();
            builder.Services.AddScoped<IGenericRepository, GenericRepository>();
            builder.Services.AddSingleton<List<Message>>();
            builder.Services.AddScoped<ILogService, LogService>();
            // Add SignalR Service
            builder.Services.AddSignalR();



            builder.Services.AddDbContext<ApplicationDbContext>
            (options => options.UseSqlServer(builder.Configuration.GetConnectionString("MinimalChatApp")));

            //builder.Services.AddCors(p => p.AddPolicy("corspolicy", build =>
            //{
            //    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
            //}));

        //    builder.Services.AddCors(p => p.AddPolicy("corspolicy", build =>
        //    {
        //        build.WithOrigins("http://localhost:4200") // Allow requests only from this origin
        //        .AllowAnyHeader()
        //        .AllowAnyMethod()
        //        .AllowCredentials();
        //}));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder
                        .WithOrigins("http://localhost:4200") // Allow requests only from this origin
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()); // Allow credentials (e.g., cookies, authorization headers)
            });




            // Configures authentication services with JWT Bearer authentication.
            ConfigurationManager Configuration = builder.Configuration;
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            //Adds JWT Bearer authentication options to the authentication services.
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["JwtSettings:Audience"],
                    ValidIssuer = Configuration["JwtSettings:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSettings:Key"]))
                };
            });

            // Define and configure Swagger documentation settings for API.
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "IdentityApi", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please Enter a valid Token!",
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
                        new string[]{}
                    }
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            // app.UseCors("corspolicy");
            app.UseCors("AllowSpecificOrigin");

            // Configure CORS headers and policies here
            app.UseCors(builder => builder
                .WithOrigins("https://example.com")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());

            // Set Cross-Origin-Opener-Policy and Cross-Origin-Embedder-Policy headers
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Cross-Origin-Opener-Policy", "same-origin-allow-popups");
                context.Response.Headers.Add("Cross-Origin-Embedder-Policy", "require-corp");
                await next.Invoke();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            // Request Logging Middleware
            app.UseMiddleware<RequestLoggingMiddleware>();

            app.MapControllers();

            app.MapHub<ChatHub>("/chathub");

            app.Run();
        }
    }
}