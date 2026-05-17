using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace UserService.Api.OpenApiConfiguration
{
    internal sealed class BearerSecuritySchemeTransformer(
        IAuthenticationSchemeProvider authenticationSchemeProvider
    ) : IOpenApiDocumentTransformer
    {
        public async Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken
        )
        {
            var authenticationSchemes =
                await authenticationSchemeProvider.GetAllSchemesAsync();

            if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
            {
                // Add the security scheme at the document level
                // var requirements = new Dictionary<string, OpenApiSecurityScheme>
                // {
                //     ["Bearer"] = new()
                //     {
                //         Type = SecuritySchemeType.Http,
                //         Scheme = "bearer",
                //         In = ParameterLocation.Header,
                //         BearerFormat = "Json Web Token"
                //     }
                // };

                var bearerScheme = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                };

                document.Components ??= new OpenApiComponents();
                document.AddComponent("Bearer", bearerScheme);

                // document.Components.SecuritySchemes = requirements;

                var securityRequirements = new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                };

                // Apply it as a requirement for all operations
                foreach (var operation in document.Paths.Values
                             .SelectMany(path => path.Operations))
                {
                    operation.Value.Security ??=
                        new List<OpenApiSecurityRequirement>();

                    operation.Value.Security.Add(securityRequirements);
                }
            }
        }
    }
}
