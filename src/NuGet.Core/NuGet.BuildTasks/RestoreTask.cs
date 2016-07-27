using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NuGet.Commands;
using NuGet.Configuration;
using NuGet.ProjectModel;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace NuGet.BuildTasks
{
    /// <summary>
    /// .NET Core compatible restore task for csproj + project.json.
    /// </summary>
    public class RestoreTask : Task
    {
        /// <summary>
        /// DG file entries
        /// </summary>
        [Required]
        public ITaskItem[] RestoreGraphItems { get; set; }

        public override bool Execute()
        {
            Debugger.Launch();

            var log = new MSBuildLogger(Log);
            var graphId = Guid.NewGuid().ToString();
            var graphLines = RestoreGraphItems.Select(item => item.ToString()).ToArray();

            using (var cacheContext = new SourceCacheContext())
            {
                cacheContext.NoCache = false;
                cacheContext.IgnoreFailedSources = false;
                var providerCache = new RestoreCommandProvidersCache();

                // Ordered request providers
                var providers = new List<IRestoreRequestProvider>();
                providers.Add(new MemoryMSBuildP2PRestoreRequestProvider(providerCache, graphId, graphLines));

                var defaultSettings = Settings.LoadDefaultSettings(root: null, configFileName: null, machineWideSettings: null);
                var sourceProvider = new CachingSourceProvider(new PackageSourceProvider(defaultSettings));

                var restoreContext = new RestoreArgs()
                {
                    CacheContext = cacheContext,
                    LockFileVersion = LockFileFormat.Version,
                    ConfigFile = null,
                    DisableParallel = false,
                    GlobalPackagesFolder = null,
                    Inputs = new List<string>() { graphId },
                    Log = log,
                    MachineWideSettings = new XPlatMachineWideSetting(),
                    RequestProviders = providers,
                    Sources = new List<string>(),
                    FallbackSources = new List<string>(),
                    CachingSourceProvider = sourceProvider
                };

                if (restoreContext.DisableParallel)
                {
                    HttpSourceResourceProvider.Throttle = SemaphoreSlimThrottle.CreateBinarySemaphore();
                }

                var restoreSummaries = RestoreRunner.Run(restoreContext).Result;

                // Summary
                RestoreSummary.Log(log, restoreSummaries);

                return restoreSummaries.All(x => x.Success);
            }
        }
    }
}
