using M06C07_security.Models;
using M06C07_security.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ITokenService,TokenManager>();
builder.Services.AddDbContextPool<dbHealthContext>(op => {
    op.UseSqlServer(builder.Configuration
        .GetConnectionString("DefaultConnection")
     );
    op.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});
//m => m.MigrationsAssembly("LightHouse_Lib")
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(op =>
{
   op.Password.RequiredLength = 7;
    op.Password.RequireUppercase = false;
    op.User.RequireUniqueEmail = true;
    op.Password.RequireDigit = false;
}).AddEntityFrameworkStores<dbHealthContext>()
                .AddDefaultTokenProviders();



builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
                .AddJwtBearer(op =>
                {
                    op.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "https://localhost:7039",
                        ValidAudience = "https://localhost:7039",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"))
                    };

                });



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
