using System.Text.Json;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;
var ocelotConfig = environment == "Development" ? "ocelot.Development.json" : "ocelot.json";
builder.Configuration.AddJsonFile(ocelotConfig, optional: false, reloadOnChange: true);

builder.Services.AddOcelot();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    RegisterOpenApiEndpoint(app);
    app.MapScalarApiReference(options =>
    {
        options.WithOpenApiRoutePattern("/openapi/gateway.json");
        options.WithTitle("Car Rental Agency API");
    });
}

app.UseRouting();
app.UseWhen(ctx => ctx.GetEndpoint() == null, b => b.UseOcelot().GetAwaiter().GetResult());
app.Run();

static void RegisterOpenApiEndpoint(WebApplication app)
{
    app.MapGet("/openapi/gateway.json", () =>
    {
        var doc = JsonSerializer.SerializeToDocument(Doc(), new JsonSerializerOptions { WriteIndented = true });
        return Results.Content(doc.RootElement.GetRawText(), "application/json");
    }).ExcludeFromDescription();
}

static Dictionary<string, object> Doc()
{
    return new()
    {
        ["openapi"] = "3.0.0",
        ["info"] = Dict(
            ("title", "Car Rental Agency — API Gateway"),
            ("version", "1.0.0"),
            ("description", "API Gateway for Car Rental Agency microservices. Authenticate via UserService to get a JWT token, then pass it as `Authorization: Bearer {token}` header.")
        ),
        ["servers"] = List(Dict(("url", "/"), ("description", "API Gateway"))),
        ["components"] = Dict(
            ("securitySchemes", Dict(
                ("bearerAuth", Dict(("type", "http"), ("scheme", "bearer"), ("bearerFormat", "JWT"))
            ))),
            ("schemas", Schemas())
        ),
        ["security"] = List(Dict(("bearerAuth", Array.Empty<string>()))),
        ["paths"] = Paths()
    };
}

static Dictionary<string, object> Schemas()
{
    return new()
    {
        ["LoginRequest"] = Schema(
            ("email", Prop("string", "email", "User email")),
            ("password", Prop("string", "password", "User password"))
        ),
        ["RegisterRequest"] = Schema(
            ("email", Prop("string", "email")),
            ("password", Prop("string", "password")),
            ("firstName", Prop("string")),
            ("lastName", Prop("string")),
            ("phone", Prop("string"))
        ),
        ["PassportData"] = Schema(
            ("passportNumber", Prop("string", null, "Passport number")),
            ("issuedBy", Prop("string", null, "Issuing authority")),
            ("issuedDate", Prop("string", "date")),
            ("birthDate", Prop("string", "date"))
        ),
        ["CarCreate"] = Schema(
            ("brand", Prop("string")),
            ("model", Prop("string")),
            ("year", Prop("integer")),
            ("licensePlate", Prop("string")),
            ("dailyRate", Prop("number", "decimal")),
            ("seats", Prop("integer")),
            ("transmission", Prop("string", null, null, new[] { "Automatic", "Manual" }))
        ),
        ["CarUpdate"] = Schema(
            ("brand", Prop("string")),
            ("model", Prop("string")),
            ("year", Prop("integer")),
            ("licensePlate", Prop("string")),
            ("dailyRate", Prop("number", "decimal")),
            ("seats", Prop("integer")),
            ("transmission", Prop("string", null, null, new[] { "Automatic", "Manual" })),
            ("status", Prop("string", null, null, new[] { "Available", "Rented", "Maintenance", "Repair" }))
        ),
        ["CreateRentalRequest"] = Schema(
            ("carId", Prop("string", "uuid")),
            ("userId", Prop("string", "uuid")),
            ("startDate", Prop("string", "date-time")),
            ("endDate", Prop("string", "date-time"))
        ),
        ["ExtendRentalRequest"] = Schema(
            ("newEndDate", Prop("string", "date-time"))
        ),
        ["CreateContractRequest"] = Schema(
            ("rentalId", Prop("string", "uuid")),
            ("templateId", Prop("string", "uuid"))
        ),
        ["SignContractRequest"] = Schema(
            ("contractId", Prop("string", "uuid")),
            ("signature", Prop("string", null, "Base64 signature image"))
        ),
        ["ChangeContractStatusRequest"] = Schema(
            ("contractId", Prop("string", "uuid")),
            ("status", Prop("string", null, null, new[] { "Active", "Cancelled", "Completed" }))
        ),
        ["CreateTemplateRequest"] = Schema(
            ("name", Prop("string")),
            ("content", Prop("string", null, "HTML template content"))
        ),
        ["UpdateTemplateContentRequest"] = Schema(
            ("templateId", Prop("string", "uuid")),
            ("content", Prop("string", null, "HTML template content"))
        ),
        ["RenameTemplateRequest"] = Schema(
            ("templateId", Prop("string", "uuid")),
            ("name", Prop("string"))
        ),
        ["PaymentRequest"] = Schema(
            ("amount", Prop("number", "decimal")),
            ("method", Prop("string", null, null, new[] { "CreditCard", "DebitCard", "PayPal" })),
            ("cardNumber", Prop("string", null, "Last 4 digits or full card number"))
        ),
        ["RefundRequest"] = Schema(
            ("amount", Prop("number", "decimal")),
            ("reason", Prop("string"))
        ),
        ["WebhookPayload"] = new Dictionary<string, object>
        {
            ["type"] = "object",
            ["properties"] = new Dictionary<string, object>
            {
                ["event"] = Prop("string", null, "Webhook event type"),
                ["data"] = Dict(("type", "object"), ("description", "Event payload data"))
            },
            ["required"] = new[] { "event", "data" }
        }
    };
}

static Dictionary<string, object> Paths()
{
    var p = new Dictionary<string, object>();

    void Add(string path, params (string method, string summary, string? schemaRef)[] ops)
    {
        var item = new Dictionary<string, object>();
        foreach (var (method, summary, schemaRef) in ops)
        {
            var tag = summary switch
            {
                _ when summary.Contains("user", StringComparison.OrdinalIgnoreCase) => "UserService",
                _ when summary.Contains("car", StringComparison.OrdinalIgnoreCase) => "CarService",
                _ when summary.Contains("rental", StringComparison.OrdinalIgnoreCase) => "RentalService",
                _ when summary.Contains("contract") || summary.Contains("template") => "ContractService",
                _ when summary.Contains("payment") || summary.Contains("refund") || summary.Contains("webhook") => "PaymentService",
                _ => "Other"
            };

            var op = new Dictionary<string, object>
            {
                ["summary"] = summary,
                ["tags"] = new[] { tag },
                ["responses"] = new Dictionary<string, object>
                {
                    ["200"] = Dict(("description", "Success")),
                    ["400"] = Dict(("description", "Bad Request")),
                    ["401"] = Dict(("description", "Unauthorized")),
                    ["403"] = Dict(("description", "Forbidden")),
                    ["404"] = Dict(("description", "Not Found"))
                },
                ["security"] = new[] { new Dictionary<string, object> { ["bearerAuth"] = Array.Empty<string>() } }
            };
            if (schemaRef != null)
            {
                op["requestBody"] = new Dictionary<string, object>
                {
                    ["required"] = true,
                    ["content"] = new Dictionary<string, object>
                    {
                        ["application/json"] = new Dictionary<string, object>
                        {
                            ["schema"] = new Dictionary<string, object> { ["$ref"] = schemaRef }
                        }
                    }
                };
            }
            item[method] = op;
        }
        p[path] = item;
    }

    void Get(string path, string summary) => Add(path, ("get", summary, null));
    void Post(string path, string summary, string? schemaRef = null) => Add(path, ("post", summary, schemaRef));
    void Put(string path, string summary, string? schemaRef = null) => Add(path, ("put", summary, schemaRef));
    void Delete(string path, string summary) => Add(path, ("delete", summary, null));

    // UserService
    Add("/api/User", ("get", "Get all users", null), ("post", "Register a new user", "#/components/schemas/RegisterRequest"));
    Add("/api/User/{id}", ("get", "Get user by ID", null), ("delete", "Remove user", null));
    Post("/api/User/login-user", "Login (returns JWT token)", "#/components/schemas/LoginRequest");
    Post("/api/User/register", "Register", "#/components/schemas/RegisterRequest");
    Get("/api/User/user-personal-info/{id}", "Get user personal info");
    Post("/api/User/add-passport/{userId}", "Add passport data", "#/components/schemas/PassportData");
    Put("/api/User/deactivate-user/{userId}", "Deactivate user");
    Put("/api/User/activate-user/{userId}", "Activate user");
    Delete("/api/User/remove-user/{userId}", "Remove user");

    // CarService
    Add("/api/Car", ("get", "Get all cars", null), ("post", "Add a new car", "#/components/schemas/CarCreate"));
    Get("/api/Car/available", "Get available cars (public)");
    Get("/api/Car/public-car/{carId}", "Get public detailed car info (no auth)");
    Get("/api/Car/my-rented", "Get my rented cars");
    Get("/api/Car/detailed-car/{carId}", "Get detailed car info");
    Post("/api/Car/add-car", "Add a new car", "#/components/schemas/CarCreate");
    Put("/api/Car/update-car/{id}", "Update car info", "#/components/schemas/CarUpdate");
    Delete("/api/Car/delete-car/{id}", "Delete car");
    Put("/api/Car/rent/{carId}", "Rent a car");
    Put("/api/Car/return/{carId}", "Return a car");
    Put("/api/Car/break/{carId}", "Report car as broken");
    Put("/api/Car/send-to-maintenance/{carId}", "Send car to maintenance");
    Put("/api/Car/send-to-repair/{carId}", "Send car to repair");
    Put("/api/Car/complete-maintenance/{carId}", "Complete maintenance");
    Put("/api/Car/process-return/{carId}", "Process car return");
    Put("/api/Car/mark-returned/{carId}", "Mark car as returned");

    // RentalService
    Get("/api/Rental/GetRentals", "Get all rentals");
    Get("/api/Rental/GetRental/{id}", "Get rental by ID");
    Post("/api/Rental/CalculateEstimatedCost/{id}", "Calculate estimated cost");
    Post("/api/Rental/CreateRental", "Create a new rental", "#/components/schemas/CreateRentalRequest");
    Put("/api/Rental/RenewRental/{id}", "Renew rental", "#/components/schemas/ExtendRentalRequest");
    Put("/api/Rental/EndRental/{id}", "End rental");
    Put("/api/Rental/CancelRental/{id}", "Cancel rental");

    // ContractService
    Get("/api/Contract/get-contracts", "Get all contracts");
    Get("/api/Contract/get-contract-{id}", "Get contract by ID");
    Get("/api/Contract/get-contract-{id}/pdf", "Download contract PDF");
    Post("/api/Contract/create-contract", "Create a new contract", "#/components/schemas/CreateContractRequest");
    Put("/api/Contract/sign-contract", "Sign a contract", "#/components/schemas/SignContractRequest");
    Put("/api/Contract/cancel-contract", "Cancel a contract");
    Put("/api/Contract/change-status", "Change contract status", "#/components/schemas/ChangeContractStatusRequest");

    // ContractTemplate
    Get("/api/ContractTemplate/get-templates", "Get contract templates");
    Get("/api/ContractTemplate/get-template-{id}", "Get template by ID");
    Post("/api/ContractTemplate/create-template", "Create a new template", "#/components/schemas/CreateTemplateRequest");
    Put("/api/ContractTemplate/update-content", "Update template content", "#/components/schemas/UpdateTemplateContentRequest");
    Put("/api/ContractTemplate/rename", "Rename template", "#/components/schemas/RenameTemplateRequest");

    // PaymentService
    Get("/api/Payments/methods", "Get payment methods");
    Post("/api/Payments/pay/{rentalId}", "Process payment", "#/components/schemas/PaymentRequest");
    Post("/api/Payments/refund/{rentalId}", "Process refund", "#/components/schemas/RefundRequest");
    Post("/api/Payments/webhook", "Payment webhook", "#/components/schemas/WebhookPayload");

    return p;
}

static Dictionary<string, object> Schema(params (string name, object prop)[] properties)
{
    var props = new Dictionary<string, object>();
    var required = new List<string>();
    foreach (var (name, prop) in properties)
    {
        props[name] = prop;
        required.Add(name);
    }
    return new()
    {
        ["type"] = "object",
        ["properties"] = props,
        ["required"] = required.ToArray()
    };
}

static Dictionary<string, object> Prop(string type, string? format = null, string? description = null, string[]? enumValues = null)
{
    var p = new Dictionary<string, object> { ["type"] = type };
    if (format != null) p["format"] = format;
    if (description != null) p["description"] = description;
    if (enumValues != null) p["enum"] = enumValues;
    return p;
}

static Dictionary<string, object> Dict(params (string key, object val)[] items)
{
    var d = new Dictionary<string, object>();
    foreach (var (key, val) in items) d[key] = val;
    return d;
}

static List<object> List(params object[] items) => new(items);
