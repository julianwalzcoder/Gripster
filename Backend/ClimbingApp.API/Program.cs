using ClimbingApp.Model.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ClimbRepository>();
builder.Services.AddScoped<GymRepository>();
builder.Services.AddScoped<AdminRepository>();
builder.Services.AddScoped<GradeRepository>();
builder.Services.AddScoped<SessionRepository>();
builder.Services.AddScoped<SessionRouteRepository>();
builder.Services.AddScoped<AuthRepository>();
builder.Services.AddScoped<UserRouteRepository>();
builder.Services.AddScoped(sp => new UserSessionRepository(sp.GetRequiredService<IConfiguration>()));

var key = builder.Configuration["Jwt:Key"];
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});
builder.Services.AddScoped<AuthRepository, AuthRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(policy =>
{
    policy.AllowAnyHeader();
    policy.AllowAnyMethod();
    policy.AllowAnyOrigin();
});

// 1. Authentication (validates JWT)
app.UseAuthentication();

// 2. Authorization (checks [Authorize] attributes)
app.UseAuthorization();

// 3. Controllers should run AFTER auth
app.MapControllers();

//4. Fallback support
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.Run();

