// --------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// --------------------------------------------------------------

using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace WebApiVersioning.Helpers.Versioning;

/// <summary>
/// See https://github.com/dotnet/aspnet-api-versioning/wiki/
/// </summary>
public static class ServicesExtensions
{
    /// <summary>
    /// Adds service API versioning and Swagger to the specified services collection.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddApiVersioningCustomized(this IServiceCollection services, Action<ApiVersioningOptions>? options = null)
    {
        var config = new ApiVersioningOptions();
        options?.Invoke(config);

        services.AddApiVersioning(
            options =>
            {
                options.ReportApiVersions = true;

                // Last available version is assumed when a client does does not provide an API version.
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);

                // Report the API version compatibility information in responses.
                // HTTP headers "api-supported-versions" and "api-deprecated-versions" will be added to all valid service routes
                options.ReportApiVersions = true;

                options.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("api-version"),
                    new HeaderApiVersionReader("x-ms-api-version"),
                    new MediaTypeApiVersionReader("ver"));
            })
        .AddMvc()
        .AddApiExplorer(
            options =>
            {
                // Add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // Note: the specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "F";

                // Note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                options.SubstituteApiVersionInUrl = true;
            });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddTransient<ApiVersioningOptions>(factory => config);
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerOptions>();
        services.AddSwaggerGen(
            options =>
            {
                // add a custom operation filter which sets default values
                options.OperationFilter<SwaggerDefaultValues>();

                // Integrate xml comments
                if (!string.IsNullOrEmpty(config.AssemblyDocumentationFile))
                {
                    if (File.Exists(config.AssemblyDocumentationFile))
                    {
                        options.IncludeXmlComments(config.AssemblyDocumentationFile);
                    }
                }
            });

        return services;
    }

    /// <summary>
    /// Add versioning Endpoints to the SwaggerUI middleware,
    /// to display versions in dropdown list.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="app"></param>
    /// <returns></returns>
    public static SwaggerUIOptions AddEndpointVersioning(this SwaggerUIOptions options, WebApplication app)
    {
        var versions = app.DescribeApiVersions().Select(i => i.GroupName);
        return AddEndpointVersioning(options, versions);
    }

    /// <summary>
    /// Add versioning Endpoints to the SwaggerUI middleware,
    /// to display versions in dropdown list.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="apiVersions"></param>
    /// <returns></returns>
    public static SwaggerUIOptions AddEndpointVersioning(this SwaggerUIOptions options, Type apiVersions)
    {
        var versions = apiVersions.GetFields().Select(i => Convert.ToString(i.GetValue(null)) ?? string.Empty);
        return AddEndpointVersioning(options, versions);
    }

    /// <summary>
    /// Add versioning Endpoints to the SwaggerUI middleware,
    /// to display versions in dropdown list.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="versions"></param>
    /// <returns></returns>
    public static SwaggerUIOptions AddEndpointVersioning(this SwaggerUIOptions options, IEnumerable<string> versions)
    {
        // build a swagger endpoint for each discovered API version
        foreach (var description in versions.OrderByDescending(i => i))
        {
            var url = $"/swagger/{description}/swagger.json";
            var name = description;
            options.SwaggerEndpoint(url, name);
        }

        return options;
    }
}