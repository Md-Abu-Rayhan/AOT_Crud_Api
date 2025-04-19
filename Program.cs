using AOTCrudApi.Models;
using AOTCrudApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

// Set the feature flag BEFORE creating the builder
AppContext.SetSwitch("Microsoft.AspNetCore.Mvc.ApiExplorer.IsEnhancedModelMetadataSupported", true);

var builder = WebApplication.CreateSlimBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Products API",
        Version = "v1"
    });
});

// Enable enhanced model metadata support for AOT
builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.TypeInfoResolver = AppJsonSerializationContext.Default;
});

// Configure runtime to enable enhanced model metadata
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.PropertyNamingPolicy = null;
    options.SerializerOptions.TypeInfoResolver = AppJsonSerializationContext.Default;
});

// Add full MVC with JSON support instead of just MvcCore
builder.Services.AddControllers()
    .AddJsonOptions(options => 
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.TypeInfoResolver = AppJsonSerializationContext.Default;
    });

// Register services
builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddScoped<ProductService>();

// Enable routing configuration with Regex constraint for parameter names
builder.Services.Configure<RouteOptions>(options =>
    options.SetParameterPolicy<RegexInlineRouteConstraint>("regex"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers(); // Map the controllers to endpoints

app.Run();
