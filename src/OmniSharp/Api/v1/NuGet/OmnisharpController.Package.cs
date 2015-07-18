using System;
﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Recommendations;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Framework.Logging;
#if DNX451
using NuGet.Logging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
#endif
using OmniSharp.Dnx;
using OmniSharp.Documentation;
using OmniSharp.Extensions;
using OmniSharp.Intellisense;
using OmniSharp.Models;
using OmniSharp.NuGet;

namespace OmniSharp
{
#if DNX451
    public class PackageController
    {
        private Microsoft.Framework.Logging.ILogger _logger;
        private static CancellationTokenSource _tokenSource;

        public PackageController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<PackageController>();
        }


        [HttpPost("packagesearch")]
        public async Task<PackageSearchResponse> PackageSearch(PackageSearchRequest request)
        {
            var projectPath = request.ProjectPath;
            if (request.ProjectPath.EndsWith(".json"))
            {
                projectPath = Path.GetDirectoryName(projectPath);
            }

            if (!string.IsNullOrWhiteSpace(projectPath))
            {
                if (request.SupportedFrameworks == null)
                    request.SupportedFrameworks = Enumerable.Empty<string>();

                if (request.PackageTypes == null)
                    request.PackageTypes = Enumerable.Empty<string>();

                if (_tokenSource != null)
                {
                    _logger.LogInformation("cancelling");
                    _tokenSource.Cancel();
                }
                _tokenSource = new CancellationTokenSource();

                var filter = new SearchFilter()
                {
                    SupportedFrameworks = request.SupportedFrameworks,
                    IncludePrerelease = request.IncludePrerelease,
                    PackageTypes = request.PackageTypes
                };
                var tasks = new List<Task<IEnumerable<SimpleSearchMetadata>>>();
                var repositoryProvider = new OmniSharpSourceRepositoryProvider(projectPath);
                var repos = repositoryProvider.GetRepositories().ToArray();

                var allTasks = new List<Task>();
                var concurrency = PlatformHelper.IsMono ? 10 : 10;
                var throttler = new SemaphoreSlim(initialCount: concurrency);
                var allResults = new List<SimpleSearchMetadata>();

                foreach (var repo in repos)
                {
                    _logger.LogInformation($"Searching {repo} for {request.Search}");
                    // do an async wait until we can schedule again
                    await throttler.WaitAsync();

                    // using Task.Run(...) to run the lambda in its own parallel
                    // flow on the threadpool
                    allTasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            var resource = await repo.GetResourceAsync<SimpleSearchResource>();
                            if (resource != null)
                            {
                                var results = await resource.Search(request.Search, filter, 0, 50, _tokenSource.Token);
                                _logger.LogInformation($"Found {results.Count()} results for {request.Search} from {repo}");
                                allResults.AddRange(results);
                            }
                        }
                        catch(OperationCanceledException)
                        {
                            _logger.LogInformation("search cancelled");
                        }
                        finally
                        {
                            throttler.Release();
                        }
                    }));
                }

                await Task.WhenAll(allTasks);
                return MergeResults(allResults, repos);
            }

            return new PackageSearchResponse();
        }

        private PackageSearchResponse MergeResults(IEnumerable<SimpleSearchMetadata> results, IEnumerable<SourceRepository> repos)
        {
            var comparer = new PackageIdentityComparer();
            return new PackageSearchResponse()
            {
                Sources = repos.Select(repo => repo.PackageSource.Source),
                Packages = results
                    .GroupBy(metadata => metadata.Identity.Id)
                    .Select(metadataGroup => metadataGroup.OrderByDescending(metadata => metadata.Identity, comparer).First())
                    .OrderBy(metadata => metadata.Identity.Id)
                    .Select(metadata => new PackageSearchItem()
                    {
                        Id = metadata.Identity.Id,
                        Version = metadata.Identity.Version.ToNormalizedString(),
                        HasVersion = metadata.Identity.HasVersion,
                        Description = metadata.Description
                    })
            };
        }

        [HttpPost("packageversion")]
        public async Task<PackageVersionResponse> PackageVersion(PackageVersionRequest request)
        {
            var projectPath = request.ProjectPath;
            if (request.ProjectPath.EndsWith(".json"))
            {
                projectPath = Path.GetDirectoryName(projectPath);
            }

            if (!string.IsNullOrWhiteSpace(projectPath))
            {
                var token = CancellationToken.None;

                var filter = new SearchFilter
                {
                    IncludePrerelease = request.IncludePrerelease
                };
                var foundVersions = new List<NuGetVersion>();
                var repositoryProvider = new OmniSharpSourceRepositoryProvider(projectPath);
                var repos = repositoryProvider.GetRepositories().ToArray();
                foreach (var repo in repos)
                {
                    // TODO: Swap when bug is fixed
                    // https://github.com/NuGet/NuGet3/pull/90
                    /*
                    var resource = await repo.GetResourceAsync<FindPackageByIdResource>();
                    if (resource != null)
                    {
                        resource.Logger = NullLogger.Instance;
                        resource.NoCache = true;
                        foundVersions.AddRange(await resource.GetAllVersionsAsync(request.Id, token));
                    }*/
                    var resource = await repo.GetResourceAsync<SimpleSearchResource>();
                    if (resource != null)
                    {
                        var result = await resource.Search(request.Id, filter, 0, 50, token);
                        var package = result.FirstOrDefault(metadata => metadata.Identity.Id == request.Id);
                        if (package != null)
                            foundVersions.AddRange(package.AllVersions);
                    }
                }

                var comparer = new VersionComparer();
                var versions = Enumerable.Distinct<NuGetVersion>(foundVersions, comparer)
                    .OrderByDescending(version => version, comparer)
                    .Select(version => version.ToNormalizedString());

                return new PackageVersionResponse()
                {
                    Versions = versions
                };
            }

            return new PackageVersionResponse();
        }
    }
#endif
}
