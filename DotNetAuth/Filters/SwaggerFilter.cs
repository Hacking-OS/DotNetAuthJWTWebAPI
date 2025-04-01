using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using DotNetAuth.Domain.Contracts;

public class ErrorResponseOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var responseAttributes = context.MethodInfo
            .GetCustomAttributes(typeof(ProducesResponseTypeAttribute), false)
            .Cast<ProducesResponseTypeAttribute>()
            .Where(attr => attr.Type == typeof(ErrorResponse))
            .ToList();

        foreach (var attr in responseAttributes)
        {
            int statusCode = attr.StatusCode == 0 ? 400 : attr.StatusCode;

            // 🚀 Ensure the status code exists in Responses
            if (!operation.Responses.ContainsKey(statusCode.ToString()))
            {
                operation.Responses[statusCode.ToString()] = new OpenApiResponse
                {
                    Description = $"Error Response {statusCode}"
                };
            }

            if (operation.Responses.TryGetValue(statusCode.ToString(), out OpenApiResponse response))
            {
                // 🌟 Add Example Data to Swagger Response
                response.Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["title"] = new OpenApiSchema { Type = "string" },
                                ["statusCode"] = new OpenApiSchema { Type = "integer" },
                                ["message"] = new OpenApiSchema { Type = "string" }
                            }
                        },
                        Example = new Microsoft.OpenApi.Any.OpenApiObject
                        {
                            ["title"] = new Microsoft.OpenApi.Any.OpenApiString(GetErrorTitle(statusCode)),
                            ["statusCode"] = new Microsoft.OpenApi.Any.OpenApiInteger(statusCode),
                            ["message"] = new Microsoft.OpenApi.Any.OpenApiString("An error occurred.")
                        }
                    }
                };
            }
        }
    }


    private string GetErrorTitle(int statusCode)
    {
        return statusCode switch
        {
            400 => "Bad Request",
            401 => "Unauthorized",
            403 => "Forbidden",
            404 => "Not Found",
            500 => "Internal Server Error",
            _ => "Error"
        };
    }
}
