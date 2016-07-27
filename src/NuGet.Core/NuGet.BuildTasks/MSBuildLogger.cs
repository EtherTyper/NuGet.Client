using Microsoft.Build.Utilities;
using NuGet.Common;

namespace NuGet.BuildTasks
{
    /// <summary>
    /// TaskLoggingHelper -> ILogger
    /// </summary>
    internal class MSBuildLogger : ILogger
    {
        private readonly TaskLoggingHelper _taskLogging;

        public MSBuildLogger(TaskLoggingHelper taskLogging)
        {
            _taskLogging = taskLogging;
        }

        public void LogDebug(string data)
        {
            _taskLogging.LogMessage(data);
        }

        public void LogError(string data)
        {
            _taskLogging.LogError(data);
        }

        public void LogErrorSummary(string data)
        {
            _taskLogging.LogMessage(data);
        }

        public void LogInformation(string data)
        {
            _taskLogging.LogMessage(data);
        }

        public void LogInformationSummary(string data)
        {
            _taskLogging.LogMessage(data);
        }

        public void LogMinimal(string data)
        {
            _taskLogging.LogMessage(data);
        }

        public void LogVerbose(string data)
        {
            _taskLogging.LogMessage(data);
        }

        public void LogWarning(string data)
        {
            _taskLogging.LogWarning(data);
        }
    }
}
