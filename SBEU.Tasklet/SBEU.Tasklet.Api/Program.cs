using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using SBEU.Tasklet.Api.Models;
using SBEU.Tasklet.Api.Service;

using System.Text;
using AutoMapper.Internal;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using SBEU.Tasklet.Api.Hubs;
using SBEU.Tasklet.Api.Service.Interface;
using SBEU.Tasklet.DataLayer.DataBase;
using SBEU.Tasklet.DataLayer.DataBase.Entities;
using StackExchange.Redis;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton(FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("./firetasklet.json")
}));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<UserProfile>();
    cfg.Internal().MethodMappingEnabled = false;
});
builder.Services.AddSignalR();
var name = Environment.GetEnvironmentVariable("DATABASENAME");
var user = Environment.GetEnvironmentVariable("DATABASEUSER");
var password = Environment.GetEnvironmentVariable("DATABASEPASSWORD");
var host = Environment.GetEnvironmentVariable("DATABASEHOST");
var port = Environment.GetEnvironmentVariable("DATABASEPORT");
var connectionString = $"User ID={user};Password={password};Host={host};Port={port};Database={name};";
Console.WriteLine(connectionString);
builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.Configure<JwtConfig>(config =>
{
    config.ExpiryTimeFrame = TimeSpan.Parse(Environment.GetEnvironmentVariable("EXPIRYTIMEFRAME"));
    config.Secret = Environment.GetEnvironmentVariable("SECRETJWT");
});

var tempbuilder = new DbContextOptionsBuilder<ApiDbContext>();
tempbuilder.UseNpgsql(connectionString);
var tempdb = new ApiDbContext(tempbuilder.Options);
await tempdb.Database.MigrateAsync();

var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("SECRETJWT"));
var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    RequireExpirationTime = false,

    // Allow to use seconds for expiration of token
    // Required only when token lifetime less than 5 minutes
    // THIS ONE
    ClockSkew = TimeSpan.Zero
};
builder.Services.AddSingleton(tokenValidationParameters);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
{
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = tokenValidationParameters;
    jwt.Events = new JwtBearerEvents()
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            var path = context.HttpContext.Request.Path;
            if(!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/taskhub") || path.StartsWithSegments("/chathub")))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = long.MaxValue; // if don't set default value is: 30 MB
});
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
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

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            //policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            policy.WithOrigins("https://fasttasks.sbeusilent.space").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
        });
});
builder.Services.AddDefaultIdentity<XIdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApiDbContext>();
builder.Services.AddSingleton<DeadLiner>();
var chost = Environment.GetEnvironmentVariable("CACHEHOST");
var cport = Environment.GetEnvironmentVariable("CACHEPORT");

builder.Services.AddStackExchangeRedisCache(op =>
{
    op.ConfigurationOptions = ConfigurationOptions.Parse(chost+":"+cport);
});
builder.Services.AddScoped<ICCache, CustomCache>();
var app = builder.Build();

//app.Urls.Add("https://0.0.0.0:54543");
app.Urls.Add("http://0.0.0.0:54542");
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseCors();
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();
app.MapHub<ChatHub>("/chathub");
app.MapHub<TaskHub>("/taskhub");
var d= app.Services.GetRequiredService<DeadLiner>();
d.Start();
app.Run();
