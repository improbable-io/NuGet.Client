﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Configuration;
using NuGet.Packaging;

namespace NuGet.CommandLine
{
    [Command(typeof(NuGetCommand), "add", "AddCommandDescription;DefaultConfigDescription",
        MinArgs = 1, MaxArgs = 2, UsageDescriptionResourceName = "AddCommandUsageDescription",
        UsageSummaryResourceName = "AddCommandUsageSummary", UsageExampleResourceName = "AddCommandUsageExamples")]
    public class AddCommand : Command
    {
        [Option(typeof(NuGetCommand), "AddCommandSourceDescription", AltName = "src")]
        public string Source { get; set; }

        public override async Task ExecuteCommandAsync()
        {
            // Arguments[0] will not be null at this point.
            // Because, this command has MinArgs set to 1.
            var packagePath = Arguments[0];
            OfflineFeedUtility.ValidatePath(packagePath);

            if (!File.Exists(packagePath))
            {
                throw new CommandLineException(
                    LocalizedResourceManager.GetString(nameof(NuGetResources.NupkgPath_NotFound)),
                    packagePath);
            }

            Source = OfflineFeedUtility.GetEffectiveSourceFeedFolder(Source, Settings);
            OfflineFeedUtility.ValidatePath(Source);

            // If the Source Feed Folder does not exist, it will be created.

            var offlineFeedAddContext = new OfflineFeedAddContext(
                packagePath,
                Source,
                Console, // IConsole is an ILogger
                throwIfSourcePackageIsInvalid: true,
                throwIfPackageExistsAndInvalid: true,
                throwIfPackageExists: false);

            await OfflineFeedUtility.AddPackageToSource(offlineFeedAddContext, CancellationToken.None);
        }
    }
}