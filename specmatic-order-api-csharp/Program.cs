using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using specmatic_order_api_csharp.services;
using Microsoft.OpenApi.Models;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using specmatic_uuid_api.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddEndpointsApiExplorer(); // Add for OpenAPI/Swagger generation
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Order API", Version = "v1" });
            c.MapType<IFormFile>(() => new OpenApiSchema
            {
                Type = "string",
                Format = "binary"
            });
        });

        // Register your custom services
        builder.Services.AddScoped<OrderService>();
        builder.Services.AddScoped<ProductService>();

        // Register controllers
        builder.Services.AddControllers();

        // Customize the response for invalid model state
        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                var response = new ErrorResponse
                {
                    TimeStamp = DateTime.UtcNow.ToString("o"),
                    Error = "Bad Request",
                    Message = string.Join(", ", errors)
                };
                return new BadRequestObjectResult(response);
            };
        });

        var app = builder.Build();
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseSwagger();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order API V1");
            c.RoutePrefix = string.Empty; // Optional: Set Swagger UI to the root
        });
        app.Run();
    }
}