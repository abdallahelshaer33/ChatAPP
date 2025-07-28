using ChatAPI.Data;
using ChatAPI.EndPoints;
using ChatAPI.Hubs;
using ChatAPI.Models;
using ChatAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
    
var builder = WebApplication.CreateBuilder(args);
var sql = builder.Configuration.GetConnectionString("DefaultConnection") ?? Environment.GetEnvironmentVariable("DEFAULT_CONNECTION");   
var jwtsetting = builder.Configuration.GetSection("JWT");
// Add services to the container.

builder.Services.AddDbContext<Appdbcontext>(op => op.UseSqlServer(sql));
builder.Services.AddIdentityCore<APPUser>().AddEntityFrameworkStores<Appdbcontext>().AddDefaultTokenProviders();
builder.Services.AddScoped<TokenService>();
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
{
    jwt.SaveToken = true;
    jwt.RequireHttpsMetadata = false;
    jwt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtsetting.GetSection("SecretKey").Value!)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
    jwt.Events = new JwtBearerEvents
    {
        OnMessageReceived = Context =>
        
        {
            var accesstoken = Context.Request.Query["access_token"];
            var path = Context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accesstoken) && path.StartsWithSegments("/hubs"))
            {
                Context.Token = accesstoken;
            }
            return Task.CompletedTask;

        }
    };
});
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Example",
        Name = "Authorization elshaer",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
        new OpenApiSecurityScheme
        {

            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            },
            In = ParameterLocation.Header
        },
        new List<string>()
    }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularClient", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Angular origin
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // هذه مطلوبة عند استخدام credentials أو SignalR
    });
});

builder.Services.AddControllers();
builder.Services.AddSignalR();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();
app.UseRouting();
app.UseStaticFiles();
app.MapHub<ChatHub>("hubs/chat");
app.MapAccountEndpoint();
app.MapControllers();
app.UseCors("AllowAngularClient");


app.Run();
