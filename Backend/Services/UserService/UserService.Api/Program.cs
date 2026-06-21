using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using UserService.Api.Middleware;
using UserService.Api.OpenApiConfiguration;
using UserService.Application.Abstractions;
using UserService.Application.Authorization;
using UserService.Application.Common;
using UserService.Application.Features.Users.GetUsers;
using UserService.Application.Features.Users.RegisterUser;
using UserService.Application.Features.Users.RequestEmailVerification;
using UserService.Domain.Common;
using UserService.Domain.DomainEvents;
using UserService.Domain.Permissions;
using UserService.Domain.Users;
using UserService.Infrastructure;
using UserService.Infrastructure.DomainEvents;
using UserService.Infrastructure.EmailOutbox;
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

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        typeof(RegisterUserCommandHandler).Assembly,
        typeof(Program).Assembly);

    cfg.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
});

builder.Services.AddAutoMapper(cfg => { }, typeof(UserResponse).Assembly);

builder.Services.AddHttpClient("NotificationService", client =>
{
    var baseUrl = builder.Configuration["NotificationService:BaseUrl"]
        ?? "http://notification-service:8080";
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.Configure<OutboxOptions>(builder.Configuration.GetSection(OutboxOptions.SectionName));
builder.Services.AddSingleton<RequestEmailVerificationLinkBuilder>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailOutboxRepository, EmailOutboxRepository>();
builder.Services.AddScoped<IOutboxReader>(sp => (EmailOutboxRepository)sp.GetRequiredService<IEmailOutboxRepository>());
builder.Services.AddHostedService<OutboxDispatcherHostedService>();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IPasswordProcessor, PasswordProcessor>();
builder.Services.AddScoped<IJwtProvider, UserJwtProvider>();
builder.Services.AddScoped<IEmailVerificationTokenHasher, EmailVerificationTokenHasher>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddScoped<IUserAuthorizationService,  UserAuthorizationService>();
builder.Services.AddTransient<IDomainEventDispatcher,  DomainEventDispatcher>();

builder.Services.AddAuthorization(options =>
{
    foreach (var permission in Permission.All)
    {
        options.AddPolicy(permission.Name, 
            p => p.RequireClaim("permissions", permission.Name));
    }
    
    options.AddPolicy("RentalServiceOnly", policy =>
    {
        policy.RequireClaim("service", "RentalService");
    });
    
    options.AddPolicy("ContractServiceOnly", policy =>
    {
        policy.RequireClaim("service", "ContractService");
    });
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "UserAuth";
        options.DefaultChallengeScheme = "UserAuth";
    })
    .AddJwtBearer("UserAuth", options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["UserJwt:Issuer"],
            ValidAudience = builder.Configuration["UserJwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["UserJwt:SecretKey"]))
        };
    })
    .AddJwtBearer("InternalAuth", options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["InternalJwt:Issuer"],
            ValidAudience = builder.Configuration["InternalJwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["InternalJwt:SecretKey"]))
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

app.UseUserServiceExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.Run();