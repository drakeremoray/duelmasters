using Microsoft.EntityFrameworkCore;
using DuelMastersApi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DuelMastersApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var conn = builder.Configuration.GetConnectionString("DefaultConnection")
           ?? "Host=localhost;Port=5432;Database=duelmasters;Username=duelmaster;Password=duelmaster_pwd";

builder.Services.AddDbContext<DuelMastersContext>(opt => opt.UseNpgsql(conn));

// JWT configuration
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"] ?? "replace_this_with_a_secure_random_value";
var issuer = jwtSection["Issuer"] ?? "DuelMasters";
var audience = jwtSection["Audience"] ?? "DuelMastersClients";

var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
        };
    });

builder.Services.AddSingleton<ITokenService, TokenService>();

// Register service layer
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<IDeckService, DeckService>();
builder.Services.AddScoped<IDeckCardService, DeckCardService>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IGameStateService, GameStateService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Ensure DB created for local development (use migrations for prod)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DuelMastersContext>();
    db.Database.EnsureCreated();
}

app.Run();
