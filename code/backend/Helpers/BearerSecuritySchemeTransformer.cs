using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace backend.Helpers
{
    internal sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
    {
        public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
            if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
            {
                // Define the security scheme
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer", // Scheme name should be "Bearer" (often lowercase "bearer" in spec, but "Bearer" matches your example)
                    In = ParameterLocation.Header,
                    BearerFormat = "JWT", // Aligning with your example's "JWT"
                    Description = "Please enter a valid token" // Corrected missing closing quote
                };

                document.Components ??= new OpenApiComponents();
                // Use a dictionary for security schemes if adding multiple, or directly assign if only one.
                // For consistency with AddSecurityDefinition which takes an ID and the scheme:
                if (document.Components.SecuritySchemes == null)
                {
                    document.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>();
                }
                document.Components.SecuritySchemes["Bearer"] = securityScheme; // "Bearer" is the ID

                // Add the security requirement to make the scheme apply globally
                var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                        // The OpenApiSecurityScheme object acts as a key and must reference the actual scheme definition
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer" // This ID must match the key used in Components.SecuritySchemes
                            }
                        },
                        Array.Empty<string>() // No specific scopes required for Bearer token
                    }
                };

                document.SecurityRequirements ??= new List<OpenApiSecurityRequirement>();
                document.SecurityRequirements.Add(securityRequirement);
            }
        }
    }
}
