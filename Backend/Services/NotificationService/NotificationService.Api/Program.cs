using MassTransit;
using NotificationService.Application.Abstractions;
using NotificationService.Infrastructure.Messaging.Consumers;
using NotificationService.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.SetKebabCaseEndpointNameFormatter();
    busConfigurator.AddConsumer<RentalCreatedConsumer>();
    busConfigurator.AddConsumer<RentalEndedConsumer>();
    busConfigurator.AddConsumer<RentalRenewedConsumer>();
    busConfigurator.AddConsumer<RentalReturnRequestedConsumer>();
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

        configurator.ReceiveEndpoint("notification-service-contract-signed", e =>
        {
            e.ConfigureConsumer<ContractSignedConsumer>(context);
        });
        configurator.ReceiveEndpoint("notification-service-contract-created", e =>
        {
            e.ConfigureConsumer<ContractCreatedConsumer>(context);
        });
        configurator.ReceiveEndpoint("notification-service-contract-ended", e =>
        {
            e.ConfigureConsumer<ContractEndedConsumer>(context);
        });
        configurator.ReceiveEndpoint("notification-service-rental-created", e =>
        {
            e.ConfigureConsumer<RentalCreatedConsumer>(context);
        });
        configurator.ReceiveEndpoint("notification-service-rental-ended", e =>
        {
            e.ConfigureConsumer<RentalEndedConsumer>(context);
        });
        configurator.ReceiveEndpoint("notification-service-rental-renewed", e =>
        {
            e.ConfigureConsumer<RentalRenewedConsumer>(context);
        });
        configurator.ReceiveEndpoint("notification-service-rental-return-requested", e =>
        {
            e.ConfigureConsumer<RentalReturnRequestedConsumer>(context);
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

app.MapControllers();

app.Run();
