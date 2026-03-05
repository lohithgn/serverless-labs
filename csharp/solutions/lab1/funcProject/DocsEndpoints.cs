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
                    <title>Serverless HTTP API - Swagger UI</title>
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
                    Title = "Inventory API",
                    Version = "1.0.0",
                    Description = "Azure Functions HTTP API with ASP.NET Core"
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
                    ["/api/products"] = new OpenApiPathItem
                    {
                        Operations =
                        {
                            [OperationType.Get] = new OpenApiOperation
                            {
                                Summary = "List all products in inventory",
                                OperationId = "ListProducts",
                                Responses = new OpenApiResponses
                                {
                                    ["200"] = new OpenApiResponse
                                    {
                                        Description = "List of products",
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
                                                            Items = ProductSchema()
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
                                Summary = "Add a new product to the inventory",
                                OperationId = "AddProduct",
                                RequestBody = new OpenApiRequestBody
                                {
                                    Required = true,
                                    Content =
                                    {
                                        ["application/json"] = new OpenApiMediaType
                                        {
                                            Schema = ProductCreateSchema()
                                        }
                                    }
                                },
                                Responses = new OpenApiResponses
                                {
                                    ["201"] = new OpenApiResponse
                                    {
                                        Description = "Product created",
                                        Content =
                                        {
                                            ["application/json"] = new OpenApiMediaType
                                            {
                                                Schema = ProductSchema()
                                            }
                                        }
                                    },
                                    ["409"] = new OpenApiResponse
                                    {
                                        Description = "Product with this SKU already exists"
                                    }
                                }
                            }
                        }
                    },
                    ["/api/products/{sku}"] = new OpenApiPathItem
                    {
                        Operations =
                        {
                            [OperationType.Get] = new OpenApiOperation
                            {
                                Summary = "Get a specific product by SKU",
                                OperationId = "GetProduct",
                                Parameters = new List<OpenApiParameter>
                                {
                                    new()
                                    {
                                        Name = "sku",
                                        In = ParameterLocation.Path,
                                        Required = true,
                                        Schema = new OpenApiSchema { Type = "string" }
                                    }
                                },
                                Responses = new OpenApiResponses
                                {
                                    ["200"] = new OpenApiResponse
                                    {
                                        Description = "Product found",
                                        Content =
                                        {
                                            ["application/json"] = new OpenApiMediaType
                                            {
                                                Schema = ProductSchema()
                                            }
                                        }
                                    },
                                    ["404"] = new OpenApiResponse
                                    {
                                        Description = "Product not found"
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        private static OpenApiSchema ProductCreateSchema() => new()
        {
            Type = "object",
            Properties =
            {
                ["name"] = new OpenApiSchema { Type = "string", MaxLength = 255 },
                ["description"] = new OpenApiSchema { Type = "string", Nullable = true, MaxLength = 1000 },
                ["category"] = new OpenApiSchema { Type = "string", MaxLength = 100 },
                ["price"] = new OpenApiSchema { Type = "number", Format = "decimal" },
                ["sku"] = new OpenApiSchema { Type = "string", Pattern = @"^[A-Z0-9-]+$", MaxLength = 50 },
                ["quantity"] = new OpenApiSchema { Type = "integer", Default = new OpenApiInteger(0) },
                ["status"] = new OpenApiSchema
                {
                    Type = "string",
                    Enum = { new OpenApiString("Active"), new OpenApiString("Inactive") },
                    Default = new OpenApiString("Active")
                }
            },
            Required = new HashSet<string> { "name", "category", "price", "sku" }
        };

        private static OpenApiSchema ProductSchema()
        {
            var schema = ProductCreateSchema();
            schema.Properties["id"] = new OpenApiSchema { Type = "string" };
            return schema;
        }
    }
}
