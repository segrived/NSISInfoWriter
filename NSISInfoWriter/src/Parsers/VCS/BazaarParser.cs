using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace NSISInfoWriter.Parsers.VCS
{
    public struct BazaarVersionInformation
    {
        public string Date { get; }
        public string RevisionNumber { get; }
        public string RevisionId { get; }

        public BazaarVersionInformation(string input) {
            var revIdRegex = new Regex("revision-id: (.*)");
            var revNoRegex = new Regex("revno: (.*)");
            var dateRegex = new Regex("date: (.*)");

            this.RevisionId = revIdRegex.Match(input).Groups[1].Value;
            this.RevisionNumber = revNoRegex.Match(input).Groups[1].Value;
            this.Date = dateRegex.Match(input).Groups[1].Value;
        }
    }

    public class BazaarParser : IParser
    {
        public const string Prefix = "BAZAAR";

        private readonly string _repoDirectory;
        private readonly CommandProcessor _cmdProcessor;
        private readonly string _timeFormat;

        public BazaarParser(string repoDirectory, string timeFormat) {
            this._cmdProcessor = new CommandProcessor("bzr.exe", repoDirectory);
            this._timeFormat = timeFormat;
            this._repoDirectory = repoDirectory;
        }

        private bool IsAvailableVcsExecutable() =>
            this._cmdProcessor.IsZeroExitCode("help");


        private bool IsUnderControl() => this.IsUnderControl(this._repoDirectory);
        // can't get exit code with simple `status` command
        private bool IsUnderControl(string directory) {
            if (Directory.Exists(Path.Combine(directory, ".bzr"))) {
                return true;
            } else {
                var directoryInfo = new DirectoryInfo(directory);
                if (directoryInfo.Parent == null) {
                    return false;
                }
                return this.IsUnderControl(Directory.GetParent(directory).FullName);
            }
        }

        public BazaarVersionInformation GetVersionInfo() {
            var output = this._cmdProcessor.GetOut("version-info");
            return new BazaarVersionInformation(output);
        }

        public string GetEmail() =>
            this._cmdProcessor.GetOut("config email");

        public bool IsParseble() {
            return this.IsAvailableVcsExecutable() && this.IsUnderControl(this._repoDirectory);
        }

        public Dictionary<string, string> Generate() {
            var dict = new Dictionary<string, string>();
            var versionInfo = this.GetVersionInfo();
            var dateTime = DateTime.Parse(versionInfo.Date).ToString(this._timeFormat);
            dict.Add($"{Prefix}_LAST_COMMIT_DATE", dateTime);
            dict.Add($"{Prefix}_LAST_REVISION_ID", versionInfo.RevisionId);
            dict.Add($"{Prefix}_LAST_REVISION_NUMBER", versionInfo.RevisionNumber);
            dict.Add($"{Prefix}_USEREMAIL", this.GetEmail());
            return dict;
        }
    }
}
