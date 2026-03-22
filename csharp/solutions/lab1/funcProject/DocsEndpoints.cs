using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace FuncProject
{
    /// <summary>
    /// Custom Swagger UI endpoint for Azure Functions.
    /// This is the .NET equivalent of the Python blog's custom /api/docs endpoint
    /// that uses FastAPI's get_swagger_ui_html() with the spec embedded inline.
    /// </summary>
    public class DocsEndpoints
    {
        [Function("ApiDocs")]
        public ContentResult Docs(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "docs")]
            HttpRequest req)
        {
            var specJson = GenerateOpenApiSpec().SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            var html = $$"""
                <!DOCTYPE html>
                <html>
                <head>
                    <title>IT Asset Tracker API - Swagger UI</title>
                    <link rel="stylesheet" type="text/css" href="https://unpkg.com/swagger-ui-dist@5/swagger-ui.css" />
                </head>
                <body>
                    <div id="swagger-ui"></div>
                    <script src="https://unpkg.com/swagger-ui-dist@5/swagger-ui-bundle.js"></script>
                    <script>
                        SwaggerUIBundle({
                            spec: {{specJson}},
                            dom_id: '#swagger-ui'
                        });
                    </script>
                </body>
                </html>
                """;

            return new ContentResult
            {
                Content = html,
                ContentType = "text/html",
                StatusCode = 200
            };
        }

        private static OpenApiDocument GenerateOpenApiSpec()
        {
            return new OpenApiDocument
            {
                Info = new OpenApiInfo
                {
                    Title = "IT Asset Tracker API",
                    Version = "1.0.0",
                    Description = "Azure Functions HTTP API for tracking IT assets"
                },
                Paths = new OpenApiPaths
                {
                    ["/api/hello"] = new OpenApiPathItem
                    {
                        Operations =
                        {
                            [OperationType.Get] = new OpenApiOperation
                            {
                                Summary = "Say hello",
                                OperationId = "Hello",
                                Parameters = new List<OpenApiParameter>
                                {
                                    new()
                                    {
                                        Name = "name",
                                        In = ParameterLocation.Query,
                                        Required = false,
                                        Schema = new OpenApiSchema
                                        {
                                            Type = "string",
                                            Default = new OpenApiString("World")
                                        }
                                    }
                                },
                                Responses = new OpenApiResponses
                                {
                                    ["200"] = new OpenApiResponse
                                    {
                                        Description = "Successful response",
                                        Content =
                                        {
                                            ["application/json"] = new OpenApiMediaType
                                            {
                                                Schema = new OpenApiSchema { Type = "string" }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    ["/api/helloUser"] = new OpenApiPathItem
                    {
                        Operations =
                        {
                            [OperationType.Post] = new OpenApiOperation
                            {
                                Summary = "Say hello to a user",
                                OperationId = "HelloUser",
                                RequestBody = new OpenApiRequestBody
                                {
                                    Required = true,
                                    Content =
                                    {
                                        ["application/json"] = new OpenApiMediaType
                                        {
                                            Schema = new OpenApiSchema
                                            {
                                                Type = "object",
                                                Properties =
                                                {
                                                    ["name"] = new OpenApiSchema { Type = "string" }
                                                },
                                                Required = new HashSet<string> { "name" }
                                            }
                                        }
                                    }
                                },
                                Responses = new OpenApiResponses
                                {
                                    ["200"] = new OpenApiResponse
                                    {
                                        Description = "Successful response",
                                        Content =
                                        {
                                            ["application/json"] = new OpenApiMediaType
                                            {
                                                Schema = new OpenApiSchema { Type = "string" }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    ["/api/assets"] = new OpenApiPathItem
                    {
                        Operations =
                        {
                            [OperationType.Get] = new OpenApiOperation
                            {
                                Summary = "List all tracked IT assets",
                                OperationId = "ListAssets",
                                Responses = new OpenApiResponses
                                {
                                    ["200"] = new OpenApiResponse
                                    {
                                        Description = "List of assets",
                                        Content =
                                        {
                                            ["application/json"] = new OpenApiMediaType
                                            {
                                                Schema = new OpenApiSchema
                                                {
                                                    Type = "object",
                                                    Properties =
                                                    {
                                                        ["items"] = new OpenApiSchema
                                                        {
                                                            Type = "array",
                                                            Items = AssetSchema()
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            [OperationType.Post] = new OpenApiOperation
                            {
                                Summary = "Register a new IT asset",
                                OperationId = "AddAsset",
                                RequestBody = new OpenApiRequestBody
                                {
                                    Required = true,
                                    Content =
                                    {
                                        ["application/json"] = new OpenApiMediaType
                                        {
                                            Schema = AssetCreateSchema()
                                        }
                                    }
                                },
                                Responses = new OpenApiResponses
                                {
                                    ["201"] = new OpenApiResponse
                                    {
                                        Description = "Asset registered",
                                        Content =
                                        {
                                            ["application/json"] = new OpenApiMediaType
                                            {
                                                Schema = AssetSchema()
                                            }
                                        }
                                    },
                                    ["409"] = new OpenApiResponse
                                    {
                                        Description = "Asset with this tag already exists"
                                    }
                                }
                            }
                        }
                    },
                    ["/api/assets/{assetTag}"] = new OpenApiPathItem
                    {
                        Operations =
                        {
                            [OperationType.Get] = new OpenApiOperation
                            {
                                Summary = "Get a specific asset by asset tag",
                                OperationId = "GetAsset",
                                Parameters = new List<OpenApiParameter>
                                {
                                    new()
                                    {
                                        Name = "assetTag",
                                        In = ParameterLocation.Path,
                                        Required = true,
                                        Schema = new OpenApiSchema { Type = "string" }
                                    }
                                },
                                Responses = new OpenApiResponses
                                {
                                    ["200"] = new OpenApiResponse
                                    {
                                        Description = "Asset found",
                                        Content =
                                        {
                                            ["application/json"] = new OpenApiMediaType
                                            {
                                                Schema = AssetSchema()
                                            }
                                        }
                                    },
                                    ["404"] = new OpenApiResponse
                                    {
                                        Description = "Asset not found"
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        private static OpenApiSchema AssetCreateSchema() => new()
        {
            Type = "object",
            Properties =
            {
                ["name"] = new OpenApiSchema { Type = "string", MaxLength = 255 },
                ["description"] = new OpenApiSchema { Type = "string", Nullable = true, MaxLength = 1000 },
                ["department"] = new OpenApiSchema { Type = "string", MaxLength = 100 },
                ["purchasePrice"] = new OpenApiSchema { Type = "number", Format = "decimal" },
                ["assetTag"] = new OpenApiSchema { Type = "string", Pattern = @"^[A-Z0-9-]+$", MaxLength = 50 },
                ["type"] = new OpenApiSchema
                {
                    Type = "string",
                    Enum = { new OpenApiString("Laptop"), new OpenApiString("Monitor"), new OpenApiString("Phone"), new OpenApiString("Printer"), new OpenApiString("Software"), new OpenApiString("Other") },
                    Default = new OpenApiString("Other")
                },
                ["assignedTo"] = new OpenApiSchema { Type = "string", Nullable = true, MaxLength = 255 },
                ["purchaseDate"] = new OpenApiSchema { Type = "string", Format = "date", Nullable = true },
                ["status"] = new OpenApiSchema
                {
                    Type = "string",
                    Enum = { new OpenApiString("Available"), new OpenApiString("Assigned"), new OpenApiString("InRepair"), new OpenApiString("Retired") },
                    Default = new OpenApiString("Available")
                }
            },
            Required = new HashSet<string> { "name", "department", "purchasePrice", "assetTag", "type" }
        };

        private static OpenApiSchema AssetSchema()
        {
            var schema = AssetCreateSchema();
            schema.Properties["id"] = new OpenApiSchema { Type = "string" };
            return schema;
        }
    }
}
