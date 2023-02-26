using Microsoft.EntityFrameworkCore;
using todo_universe.Data;
using todo_universe.Models;
using todo_universe;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NuGet.Common;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.OpenApi.Models;
using todo_universe.Manager;
using todo_universe.Helpers;
using todo_universe.Services;
using todo_universe.Repository;
using todo_universe.ActionFilters;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<ITodo, Todo>();
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<TodoService>();
builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.AddScoped<UserIdActionFilter>();
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Token"])),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddSingleton<JwtAuthenticationManager>();
//register helper services
builder.Services.AddScoped<AccountService>();
//builder.Services.AddSingleton<UserHelpers>();
builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "ClientApp/build";
});



builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:AppDbContextConnection"]);
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "APIApplication", Version = "v1" });
});

//_jwtAuthManager.GetPrincipal(token);
//authanticate user with token from request headers 
//builder.Services.AddAuthorization(options =>
//{
//   options.AddPolicy("RequireLoggedIn", policy => policy.RequireClaim(ClaimTypes.Name));
//});


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

app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
//app.UseCors(builder => {
//    builder.WithOrigins("http://127.0.0.1:5173")
//           .AllowAnyMethod()
//           .AllowAnyHeader()
//           .AllowCredentials();
//});

//app.Use(async (context, next) => {
//    context.Response.Headers.Add("Access-Control-Allow-Origin", "http://127.0.0.1:5173");
//    context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
//    context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
//    context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");

//    if (context.Request.Method == "OPTIONS")
//    {
//        context.Response.StatusCode = 200;
//    }
//    else
//    {
//        await next();
//    }
//});


//app.UseSpa(spa =>
//{
//    spa.Options.SourcePath = "ClientApp";

//    if (app.Environment.IsDevelopment())
//    {
//        spa.UseReactDevelopmentServer(npmScript: "dev");
//    }
//});


app.Run();
