using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentService.Application.Abstractions.Auth;
using PaymentService.Application.Abstractions.Clients;
using PaymentService.Application.Abstractions.PaymentGateway;
using PaymentService.Application.Abstractions.Repositories;
using PaymentService.Application.Abstractions.UnitOfWork;
using PaymentService.Infrastructure.Implementations.Auth;
using PaymentService.Infrastructure.Implementations.Clients;
using PaymentService.Infrastructure.Implementations.ExternalServices.BePaid;
using PaymentService.Infrastructure.Implementations.Repositories;
using PaymentService.Infrastructure.Implementations.UnitOfWork;
using PaymentService.Infrastructure.Persistence.DB;

namespace PaymentService.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var paymentSection = configuration.GetSection("ConnectionStrings");
            var connectionString = paymentSection["DefaultConnection"];

            services.AddDbContext<PaymentContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddHttpClient<IRentalServiceClient, RentalServiceClient>(client =>
            {
                var rentalSection = configuration.GetSection("RentalService");
                client.BaseAddress = new Uri(rentalSection["BaseUrl"]!);
            });
            services.AddHttpClient<IPaymentGateway, BePaidClient>();

            services.AddMassTransit(x =>
            {
                x.AddEntityFrameworkOutbox<PaymentContext>(o =>
                {
                    o.UseSqlServer();

                    o.UseBusOutbox();

                    o.QueryDelay = TimeSpan.FromSeconds(5);
                    o.DuplicateDetectionWindow = TimeSpan.FromMinutes(30);
                });

                x.UsingRabbitMq((context, cfg) =>
                {
                    var host = configuration["MessageBroker:Host"] ?? "localhost";
                    var port = configuration.GetValue<ushort>("MessageBroker:Port", 5672);

                    cfg.Host(host, port, "/", h =>
                    {
                        h.Username(configuration["MessageBroker:User"]);
                        h.Password(configuration["MessageBroker:Password"]);
                    });
                    cfg.ConfigureEndpoints(context);
                });
            });
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
            services.AddScoped<IJwtProvider, InternalJwtProvider>();
            return services;
        }
    }
}
