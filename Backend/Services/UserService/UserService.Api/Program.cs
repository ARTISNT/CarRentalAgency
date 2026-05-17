using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using UserService.Api.OpenApiConfiguration;
using UserService.Application.Common;
using UserService.Application.Features.Users.GetUsers;
using UserService.Application.Features.Users.RegisterUser;
using UserService.Domain.Common;
using UserService.Domain.DomainEvents;
using UserService.Domain.Permissions;
using UserService.Domain.Users;
using UserService.Infrastructure;
using UserService.Infrastructure.DomainEvents;
using UserService.Infrastructure.Repositories;
using UserService.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
    {
        options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    });

builder.Services.AddControllers();

builder.Services.AddDbContext<UserServiceContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(RegisterUserCommandHandler).Assembly,
    typeof(Program).Assembly
));

builder.Services.AddAutoMapper(cfg => { }, typeof(UserResponse).Assembly);
/*
builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.SetKebabCaseEndpointNameFormatter();
    
    busConfigurator.UsingRabbitMq((context, configurator)=>
    {
        configurator.Host(new Uri(builder.Configuration["MessageBroker:Host"]), h =>
        {
            h.Username(builder.Configuration["MessageBroker:Username"]);
            h.Password(builder.Configuration["MessageBroker:Password"]);
        });
    });
});
*/
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IPasswordProcessor, PasswordProcessor>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddTransient<IDomainEventDispatcher,  DomainEventDispatcher>();

builder.Services.AddAuthorization(options =>
{ 
    options.AddPolicy(Permissions.ViewUsers.Name,
        p => p.RequireClaim("permissions", Permissions.ViewUsers.Name));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])),
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ClockSkew = TimeSpan.Zero,
        };
    });

builder.Services.AddScoped<IDomainEventHandler<UserRegisteredDomainEvent>, RegisterUserDomainEventHandler>();

builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

    
app.MapControllers();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.Run();