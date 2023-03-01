// --------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// --------------------------------------------------------------

using System.Text;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApiVersioning.Helpers.Versioning;

/// <summary>
/// Configures the Swagger generation options.
/// </summary>
/// <remarks>This allows API versioning to define a Swagger document per API version after the
/// <see cref="IApiVersionDescriptionProvider"/> service has been resolved from the service container.</remarks>
internal class SwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;
    private readonly ApiVersioningOptions _versioningOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="SwaggerOptions"/> class.
    /// </summary>
    /// <param name="provider">The <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger documents.</param>
    public SwaggerOptions(
        [FromServices] IApiVersionDescriptionProvider provider,
        [FromServices] ApiVersioningOptions versioningOptions)
    {
        _provider = provider;
        _versioningOptions = versioningOptions;
    }

    /// <inheritdoc />
    public void Configure(SwaggerGenOptions options)
    {
        // add a swagger document for each discovered API version
        // note: you might choose to skip or document deprecated API versions differently
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }
    }

    private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var text = new StringBuilder(_versioningOptions.Description);
        var info = new OpenApiInfo()
        {
            Title = _versioningOptions.Title,
            Version = description.ApiVersion.ToString(),
        };

        if (description.IsDeprecated)
        {
            text.Append($" {_versioningOptions.Deprecated}");
        }

        info.Description = text.ToString();

        return info;
    }
}