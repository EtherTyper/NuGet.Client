using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NuGet.Commands
{
    /// <summary>
    /// In Memory dg file provider.
    /// </summary>
    public class MemoryMSBuildP2PRestoreRequestProvider : MSBuildP2PRestoreRequestProvider
    {
        private readonly RestoreCommandProvidersCache _providerCache;
        private readonly string[] _graphLines;
        private readonly string _graphId;

        public MemoryMSBuildP2PRestoreRequestProvider(
            RestoreCommandProvidersCache providerCache,
            string graphId,
            string[] graphLines)
            : base(providerCache)
        {
            _providerCache = providerCache;
            _graphLines = graphLines;
            _graphId = graphId;
        }

        public override Task<IReadOnlyList<RestoreSummaryRequest>> CreateRequests(
            string inputPath,
            RestoreArgs restoreContext)
        {
            if (!_graphId.Equals(inputPath, StringComparison.Ordinal))
            {
                throw new KeyNotFoundException(nameof(inputPath));
            }

            var requests = GetRequestsFromGraph(restoreContext, _graphLines);

            return Task.FromResult<IReadOnlyList<RestoreSummaryRequest>>(requests);
        }

        public override Task<bool> Supports(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return Task.FromResult(_graphId.Equals(path, StringComparison.Ordinal));
        }
    }
}
