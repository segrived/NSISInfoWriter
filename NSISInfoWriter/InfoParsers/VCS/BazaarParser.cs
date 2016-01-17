using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace NSISInfoWriter.InfoParsers.VCS
{
    public struct BazaarVersionInformation
    {
        public string Date { get; }
        public string RevisionNumber { get; }
        public string RevisionID { get; }

        public BazaarVersionInformation(string input) {
            var revIdRegex = new Regex("revision-id: (.*)");
            var revNoRegex = new Regex("revno: (.*)");
            var dateRegex = new Regex("date: (.*)");

            this.RevisionID = revIdRegex.Match(input).Groups[1].Value;
            this.RevisionNumber = revNoRegex.Match(input).Groups[1].Value;
            this.Date = dateRegex.Match(input).Groups[1].Value;
        }
    }

    public class BazaarParser : IParser
    {
        public const string Prefix = "BAZAAR";

        private string repoDirectory;
        private readonly CommandProcessor cmdProcessor;
        private string timeFormat;

        public BazaarParser(string repoDirectory, string timeFormat) {
            this.cmdProcessor = new CommandProcessor("bzr.exe", repoDirectory);
            this.timeFormat = timeFormat;
            this.repoDirectory = repoDirectory;
        }

        private bool IsAvailableVCSExecutable() =>
            this.cmdProcessor.IsZeroExitCode("help");


        private bool IsUnderControl() => IsUnderControl(this.repoDirectory);
        // can't get exit code with simple `status` command
        private bool IsUnderControl(string directory) {
            if (Directory.Exists(Path.Combine(directory, ".bzr"))) {
                return true;
            } else {
                var directoryInfo = new DirectoryInfo(directory);
                if (directoryInfo.Parent == null) {
                    return false;
                }
                return IsUnderControl(Directory.GetParent(directory).FullName);
            }
        }

        public BazaarVersionInformation GetVersionInfo() {
            var output = cmdProcessor.GetOut("version-info");
            return new BazaarVersionInformation(output);
        }

        public string GetEmail() =>
            this.cmdProcessor.GetOut("config email");

        public bool IsParseble() {
            return this.IsAvailableVCSExecutable() && this.IsUnderControl(this.repoDirectory);
        }

        public Dictionary<string, string> Generate() {
            var dict = new Dictionary<string, string>();
            var versionInfo = GetVersionInfo();
            dict.Add($"{Prefix}_LAST_COMMIT_DATE", versionInfo.Date);
            dict.Add($"{Prefix}_LAST_REVISION_ID", versionInfo.RevisionID);
            dict.Add($"{Prefix}_LAST_REVISION_NUMBER", versionInfo.RevisionNumber);
            dict.Add($"{Prefix}_USEREMAIL", this.GetEmail());
            return dict;
        }
    }
}
