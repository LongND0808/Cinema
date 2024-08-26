using Cinema.Infrastructure.DataContext;
using Cinema.Infrastructure.ImplementRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Cinema.Web.Configuration;
using Cinema.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Cinema.Core.IService;
using Cinema.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Cinema.Core.InterfaceRepository;
using Cinema.Core.Identity;
using Cinema.Core.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Cinema.Core.Converters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

#region Add Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
#endregion

#region Add Converter
builder.Services.AddTransient<UserConverter>();
#endregion

builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var config = new Config(builder.Configuration);

builder.Services.AddIdentityServer()
    .AddInMemoryIdentityResources(config.IdentityResources)
    .AddInMemoryApiScopes(config.ApiScopes)
    .AddInMemoryClients(config.Clients)
    .AddDeveloperSigningCredential()
    .AddAspNetIdentity<User>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,

        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});

var emailConfig = builder.Configuration
    .GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();

builder.Services.AddSingleton(emailConfig);

builder.Services.AddTransient<EmailService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
