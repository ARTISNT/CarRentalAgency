using System.Text;
using ContractService.Application.Abstractions.External;
using ContractService.Application.Abstractions.Security;
using ContractService.Application.Abstractions.Services;
using ContractService.Application.Authorization;
using ContractService.Application.Common;
using ContractService.Application.Features.Contracts.CreateContract;
using ContractService.Application.Features.Contracts.GetContract;
using ContractService.Application.Options;
using ContractService.Application.Services;
using ContractService.Domain.Contracts;
using ContractService.Infrastructure.ExternalServices;
using ContractService.Infrastructure.Messaging.Consumers;
using ContractService.Infrastructure.Persistence;
using ContractService.Infrastructure.Persistence.Repositories;
using ContractService.Infrastructure.Security;
using ContractService.Infrastructure.Services.ContractsGeneration;
using ContractService.Infrastructure.Services.ContractsSigning;
using ContractService.OpenApiConfiguration;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
    {
        options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    });

builder.Services.AddControllers();

builder.Services.AddDbContext<ContractServiceContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(cfg => {}, typeof(ContractResponse).Assembly);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(Program).Assembly,
    typeof(CreateContractCommand).Assembly,
    typeof(Program).Assembly));

builder.Services.Configure<DocumentTemplateOptions>(
    builder.Configuration.GetSection("DocumentBasicIdTemplates"));

builder.Services.AddSingleton<IInternalJwtProvider, InternalJwtProvider>();
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IContractTemplateRepository, ContractTemplateTemplateRepository>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<ICarExternalService, CarExternalService>();
builder.Services.AddScoped<IClientExternalService, ClientExternalService>();
builder.Services.AddScoped<IRentalExternalService, RentalExternalService>();
builder.Services.AddScoped<IPdfContractGenerator, PdfContractGenerator>();
builder.Services.AddScoped<IContractStorage, ClientContractStorageManager>();
builder.Services.AddScoped<IContractAuthorizationPolicy, ContractAuthorizationPolicy>();
builder.Services.AddScoped<IContractAuthorizationService, ContractAuthorizationService>();
builder.Services.AddScoped<ContractDocumentService>();
builder.Services.AddScoped<IClientContext, ClientContext>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IContractSigningService, ContractSigningService>();
builder.Services.AddScoped<IContractCertificateProvider, ContractCertificateProvider>();

builder.Services.AddHttpClient("UserApi", client =>
{
    client.BaseAddress = new Uri("http://user-service:8080");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddHttpClient("CarApi", client =>
{
    client.BaseAddress = new Uri("http://car-service:8080");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddHttpClient("RentalApi", client =>
{
    client.BaseAddress = new Uri("http://rental-service:8080");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddAuthorization(cfg =>
{
    foreach (var permission in Permissions.AllPermissions)
    {
        cfg.AddPolicy(permission, policy => policy.RequireClaim("permissions", permission));
    }
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
builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.SetKebabCaseEndpointNameFormatter();
    busConfigurator.AddConsumer<RentalRenewedConsumer>();
    
    busConfigurator.UsingRabbitMq((context, configurator)=>
    {
        string host = builder.Configuration["MessageBroker:Host"] ?? "localhost";
        ushort port = builder.Configuration.GetValue<ushort>("MessageBroker:Port", 5672); 
        
        
        configurator.Host(host, port, "/", h =>
        {
            h.Username(builder.Configuration["MessageBroker:User"]);
            h.Password(builder.Configuration["MessageBroker:Password"]);
        });

        configurator.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); 
}

app.MapControllers();
app.UseHttpsRedirection();
app.Run();