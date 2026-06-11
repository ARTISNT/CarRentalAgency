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
            ("phoneNumber", Prop("string"))
        ),
        ["PassportRequest"] = Schema(
            ("name", Prop("string")),
            ("surname", Prop("string")),
            ("patronymic", Prop("string")),
            ("passportNumber", Prop("string")),
            ("identityNumber", Prop("string")),
            ("passportIssueDate", Prop("string", "date")),
            ("birthDate", Prop("string", "date"))
        ),
        ["CreateCarRequest"] = SchemaOptional(
            new (string name, object prop)[] {
                ("releaseDate", Prop("string", "date-time")),
                ("licensePlate", Prop("string")),
                ("vinCode", Prop("string")),
                ("color", Prop("string")),
                ("model", Prop("string")),
                ("brand", Prop("string")),
                ("isFacelift", Prop("boolean")),
                ("mileage", Prop("number", "double")),
                ("bodyStyle", Prop("string")),
                ("transmissionType", Prop("string")),
                ("driveType", Prop("string")),
                ("engineType", Prop("string")),
                ("horsePower", Prop("integer")),
                ("pricePerHour", Prop("number", "double")),
                ("carClass", Prop("string")),
                ("photoUrl", Prop("string"))
            },
            new (string name, object prop)[] {
                ("generation", Prop("string", nullable: true)),
                ("variant", Prop("string", nullable: true)),
                ("fuelCurrentLiters", Prop("number", "double", nullable: true)),
                ("fuelCapacityLiters", Prop("number", "double", nullable: true)),
                ("batteryCurrentKWh", Prop("number", "double", nullable: true)),
                ("batteryCapacityKWh", Prop("number", "double", nullable: true)),
                ("engineVolume", Prop("number", "double", nullable: true)),
                ("powerReverse", Prop("number", "double", nullable: true))
            }
        ),
        ["UpdateCarRequest"] = SchemaOptional(
            new (string name, object prop)[] {
                ("releaseDate", Prop("string", "date-time")),
                ("licensePlate", Prop("string")),
                ("vinCode", Prop("string")),
                ("color", Prop("string")),
                ("model", Prop("string")),
                ("brand", Prop("string")),
                ("isFacelift", Prop("boolean")),
                ("mileage", Prop("number", "double")),
                ("bodyStyle", Prop("string")),
                ("transmissionType", Prop("string")),
                ("driveType", Prop("string")),
                ("engineType", Prop("string")),
                ("engineVolume", Prop("number", "double")),
                ("horsePower", Prop("integer")),
                ("powerReverse", Prop("number", "double")),
                ("pricePerHour", Prop("number", "double")),
                ("carClass", Prop("string")),
                ("photoUrl", Prop("string"))
            },
            new (string name, object prop)[] {
                ("generation", Prop("string", nullable: true)),
                ("variant", Prop("string", nullable: true)),
                ("fuelCurrentLiters", Prop("number", "double", nullable: true)),
                ("fuelCapacityLiters", Prop("number", "double", nullable: true)),
                ("batteryCurrentKWh", Prop("number", "double", nullable: true)),
                ("batteryCapacityKWh", Prop("number", "double", nullable: true))
            }
        ),
        ["CreateRentalRequest"] = SchemaOptional(
            new (string name, object prop)[] {
                ("userId", Prop("string", "uuid")),
                ("carId", Prop("string", "uuid")),
                ("startDate", Prop("string", "date-time")),
                ("endDate", Prop("string", "date-time"))
            },
            new (string name, object prop)[] {
                ("promoCode", Prop("string", nullable: true))
            }
        ),
        ["EndRentalRequest"] = SchemaOptional(
            new (string name, object prop)[] {
                ("returnDate", Prop("string", "date-time")),
                ("mileage", Prop("integer")),
                ("fuelLevel", Prop("number", "decimal")),
                ("penaltyAmount", Prop("number", "decimal"))
            },
            new (string name, object prop)[] {
                ("damageDescription", Prop("string", nullable: true))
            }
        ),
        ["CreateContractRequest"] = SchemaOptional(
            new (string name, object prop)[] {
                ("rentalId", Prop("string", "uuid")),
                ("carId", Prop("string", "uuid")),
                ("contractTemplateId", Prop("string", "uuid"))
            },
            new (string name, object prop)[] {
                ("clientId", Prop("string", "uuid", nullable: true))
            }
        ),
        ["RenewRentalRequest"] = Schema(
            ("newDate", Prop("string", "date-time"))
        ),
        ["CancelRentalRequest"] = Schema(
            ("reason", Prop("string", nullable: true))
        ),
        ["GetEstimatedRentalPriceRequest"] = Schema(
            ("promoCode", Prop("string", nullable: true))
        ),
        ["SignContractRequest"] = Schema(
            ("id", Prop("string", "uuid")),
            ("signatureBase64", Prop("string", null, "Base64 signature image"))
        ),
        ["CancelContractRequest"] = Schema(
            ("contractId", Prop("string", "uuid")),
            ("reason", Prop("string"))
        ),
        ["ChangeContractStatusRequest"] = Schema(
            ("contractId", Prop("string", "uuid")),
            ("newStatus", Prop("string", null, null, new[] { "Active", "Cancelled", "Completed" }))
        ),
        ["CreateTemplateRequest"] = Schema(
            ("name", Prop("string")),
            ("content", Prop("string", null, "HTML template content")),
            ("version", Prop("integer")),
            ("documentType", Prop("string")),
            ("validFrom", Prop("string", "date-time"))
        ),
        ["UpdateTemplateContentRequest"] = Schema(
            ("id", Prop("string", "uuid")),
            ("content", Prop("string", null, "HTML template content"))
        ),
        ["RenameTemplateRequest"] = Schema(
            ("id", Prop("string", "uuid")),
            ("name", Prop("string"))
        ),
        ["ProcessReturnRequest"] = Schema(
            ("targetStatus", Prop("string"))
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
    Get("/api/User", "Get all users");
    Add("/api/User/{id}", ("get", "Get user by ID", null), ("delete", "Remove user", null));
    Post("/api/User/login-user", "Login (returns JWT token)", "#/components/schemas/LoginRequest");
    Post("/api/User/register", "Register", "#/components/schemas/RegisterRequest");
    Get("/api/User/user-personal-info/{id}", "Get user personal info");
    Post("/api/User/add-passport/{userId}", "Add passport data", "#/components/schemas/PassportRequest");
    Put("/api/User/deactivate-user/{userId}", "Deactivate user");
    Put("/api/User/activate-user/{userId}", "Activate user");
    Delete("/api/User/remove-user/{userId}", "Remove user");

    // CarService
    Get("/api/Car", "Get all cars");
    Get("/api/Car/available", "Get available cars (public)");
    Get("/api/Car/public-car/{carId}", "Get public detailed car info (no auth)");
    Get("/api/Car/my-rented", "Get my rented cars");
    Get("/api/Car/detailed-car/{carId}", "Get detailed car info");
    Post("/api/Car/add-car", "Add a new car", "#/components/schemas/CreateCarRequest");
    Put("/api/Car/update-car/{id}", "Update car info", "#/components/schemas/UpdateCarRequest");
    Delete("/api/Car/delete-car/{id}", "Delete car");
    Put("/api/Car/rent/{carId}", "Rent a car");
    Put("/api/Car/return/{carId}", "Return a car");
    Put("/api/Car/break/{carId}", "Report car as broken");
    Put("/api/Car/send-to-maintenance/{carId}", "Send car to maintenance");
    Put("/api/Car/send-to-repair/{carId}", "Send car to repair");
    Put("/api/Car/complete-maintenance/{carId}", "Complete maintenance");
    Put("/api/Car/process-return/{carId}", "Process car return", "#/components/schemas/ProcessReturnRequest");
    Put("/api/Car/mark-returned/{carId}", "Mark car as returned");

    // RentalService
    Get("/api/Rental/GetRentals", "Get all rentals");
    Get("/api/Rental/GetRental/{id}", "Get rental by ID");
    Post("/api/Rental/CalculateEstimatedCost/{id}", "Calculate estimated cost", "#/components/schemas/GetEstimatedRentalPriceRequest");
    Post("/api/Rental/CreateRental", "Create a new rental", "#/components/schemas/CreateRentalRequest");
    Put("/api/Rental/RenewRental/{id}", "Renew rental", "#/components/schemas/RenewRentalRequest");
    Put("/api/Rental/EndRental/{id}", "End rental", "#/components/schemas/EndRentalRequest");
    Put("/api/Rental/CancelRental/{id}", "Cancel rental", "#/components/schemas/CancelRentalRequest");

    // ContractService
    Get("/api/Contract/get-contracts", "Get all contracts");
    Get("/api/Contract/get-contract-{id}", "Get contract by ID");
    Get("/api/Contract/get-contract-{id}/pdf", "Download contract PDF");
    Post("/api/Contract/create-contract", "Create a new contract", "#/components/schemas/CreateContractRequest");
    Put("/api/Contract/sign-contract", "Sign a contract", "#/components/schemas/SignContractRequest");
    Put("/api/Contract/cancel-contract", "Cancel a contract", "#/components/schemas/CancelContractRequest");
    Put("/api/Contract/change-status", "Change contract status", "#/components/schemas/ChangeContractStatusRequest");

    // ContractTemplate
    Get("/api/ContractTemplate/get-templates", "Get contract templates");
    Get("/api/ContractTemplate/get-template-{id}", "Get template by ID");
    Post("/api/ContractTemplate/create-template", "Create a new template", "#/components/schemas/CreateTemplateRequest");
    Put("/api/ContractTemplate/update-content", "Update template content", "#/components/schemas/UpdateTemplateContentRequest");
    Put("/api/ContractTemplate/rename", "Rename template", "#/components/schemas/RenameTemplateRequest");

    // PaymentService
    Get("/api/Payments/methods", "Get payment methods");
    Post("/api/Payments/pay/{rentalId}", "Process payment");
    Post("/api/Payments/refund/{rentalId}", "Process refund");
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

static Dictionary<string, object> SchemaOptional(
    (string name, object prop)[] required,
    (string name, object prop)[] optional)
{
    var props = new Dictionary<string, object>();
    var reqList = new List<string>();
    foreach (var (name, prop) in required)
    {
        props[name] = prop;
        reqList.Add(name);
    }
    foreach (var (name, prop) in optional)
    {
        props[name] = prop;
    }
    return new()
    {
        ["type"] = "object",
        ["properties"] = props,
        ["required"] = reqList.ToArray()
    };
}

static Dictionary<string, object> Prop(string type, string? format = null, string? description = null, string[]? enumValues = null, bool nullable = false)
{
    var p = new Dictionary<string, object> { ["type"] = type };
    if (format != null) p["format"] = format;
    if (description != null) p["description"] = description;
    if (enumValues != null) p["enum"] = enumValues;
    if (nullable) p["nullable"] = true;
    return p;
}

static Dictionary<string, object> Dict(params (string key, object val)[] items)
{
    var d = new Dictionary<string, object>();
    foreach (var (key, val) in items) d[key] = val;
    return d;
}

static List<object> List(params object[] items) => new(items);
