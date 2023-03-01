// --------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// --------------------------------------------------------------

using System.Reflection;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace WebApiVersioning.Helpers.Versioning;

public class ApiVersioningTools
{
    /// <summary>
    /// Returns all defined Api Versions for the specified class or method.
    /// </summary>
    /// <typeparam name="TController"></typeparam>
    /// <param name="method">Name of the method</param>
    /// <returns></returns>
    /// <example>
    /// <code>
    /// ApiVersioningTools.GetAvailableVersions&lt;EchoController&gt;(nameof(EchoController.ExecuteAsync))
    /// </code>
    /// </example>
    public static IEnumerable<(string Version, bool IsDeprecated)> GetAvailableVersions<TController>(string? method = null)
        where TController : ControllerBase
    {
        // Class with ApiVersion attribute
        var classVersions = GetVersions(typeof(TController));

        // Method with ApiVersion attribute
        var methodVersions = string.IsNullOrEmpty(method)
                           ? Array.Empty<VersionAndStatus>()
                           : GetVersions(typeof(TController).GetMethod(method));

        // Method with MapToApiVersion attribute
        var onlyVersions = string.IsNullOrEmpty(method)
                           ? Array.Empty<VersionAndStatus>()
                           : GetMappedVersions(typeof(TController).GetMethod(method));

        // All available versions
        var allVersions = classVersions.Union(methodVersions)
                                       .Distinct();

        // Filter the result if MapToApiVersion exists
        if (onlyVersions.Any())
        {
            foreach (var item in onlyVersions)
            {
                var version = allVersions.FirstOrDefault(i => i.Version == item.Version);
                if (version != null)
                {
                    yield return (version.Version, version.IsDeprecated);
                }
            }
        }

        // All available versions
        else
        {
            foreach (var item in allVersions)
            {
                yield return (item.Version, item.IsDeprecated);
            }
        }

        // List of ApiVersion attributes
        IEnumerable<VersionAndStatus> GetVersions(MemberInfo? member)
        {
            if (member is null)
            {
                return Array.Empty<VersionAndStatus>();
            }

            return member.GetCustomAttributes<ApiVersionAttribute>()
                         .Select(i => i.Versions.Select(v => new VersionAndStatus(v.ToString(), i.Deprecated)))
                         .SelectMany(i => i)
                         .Distinct();
        }

        // List of MapToApiVersion attributes
        IEnumerable<VersionAndStatus> GetMappedVersions(MemberInfo? member)
        {
            if (member is null)
            {
                return Array.Empty<VersionAndStatus>();
            }

            return member.GetCustomAttributes<MapToApiVersionAttribute>()
                         .Select(i => i.Versions.Select(v => new VersionAndStatus(v.ToString(), false)))
                         .SelectMany(i => i)
                         .Distinct();
        }
    }

    private record VersionAndStatus(string Version, bool IsDeprecated);
}