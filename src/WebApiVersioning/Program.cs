// --------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// --------------------------------------------------------------

using Asp.Versioning;
using WebApiVersioning;
using WebApiVersioning.Helpers.Versioning;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// *** Versioning
builder.Services.AddApiVersioningCustomized(options =>
{
    // options.ApiVersionSelector = new ConstantApiVersionSelector(new ApiVersion(Convert.ToDouble(ApiVersions.V2_0)));
    options.AssemblyDocumentationFile = Path.Combine(AppContext.BaseDirectory, typeof(Program).Assembly.GetName().Name + ".xml");
    options.Title = "WorkOrder APIs";
    options.Description = "List of WorkOrder APIs";
});
// *** Versioning

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // *** Versioning
        options.AddEndpointVersioning(typeof(ApiVersions));
        // *** Versioning
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
