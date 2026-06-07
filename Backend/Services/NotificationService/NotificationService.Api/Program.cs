using MassTransit;
using NotificationService.Application.Abstractions;
using NotificationService.Infrastructure.Messaging.Consumers;
using NotificationService.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.SetKebabCaseEndpointNameFormatter();
    busConfigurator.AddConsumer<RentalCreatedConsumer>();
    busConfigurator.AddConsumer<RentalEndedConsumer>();
    busConfigurator.AddConsumer<RentalRenewedConsumer>();
    busConfigurator.AddConsumer<ContractCreatedConsumer>();
    busConfigurator.AddConsumer<ContractSignedConsumer>();
    busConfigurator.AddConsumer<ContractEndedConsumer>();

    busConfigurator.UsingRabbitMq((context, configurator) =>
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

builder.Services.AddScoped<INotificationSender, EmailNotificationSender>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();
