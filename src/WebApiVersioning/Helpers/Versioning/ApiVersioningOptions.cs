// --------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// --------------------------------------------------------------

namespace WebApiVersioning.Helpers.Versioning;

public class ApiVersioningOptions
{
    /// <summary>
    /// The XML Documentation File to generate documentation for WebAPI.
    /// Ex. `Path.Combine(AppContext.BaseDirectory, typeof(Program).Assembly.GetName().Name + ".xml")`
    /// </summary>
    public string AssemblyDocumentationFile { get; set; } = string.Empty;

    /// <summary>
    /// The title of the application.
    /// </summary>
    public string Title { get; set; } = "My Project APIs";

    /// <summary>
    /// A short description of the application.
    /// </summary>
    public string Description { get; set; } = "An example application with OpenAPI, Swashbuckle, and API versioning.";

    /// <summary>
    /// Description complement when this version is deprecated.
    /// </summary>
    public string Deprecated { get; set; } = "This API version has been deprecated.";
}
