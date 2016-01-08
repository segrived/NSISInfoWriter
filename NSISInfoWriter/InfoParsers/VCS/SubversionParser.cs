using System;
using System.Collections.Generic;

namespace NSISInfoWriter.InfoParsers.VCS
{
    class SubversionParser : IParser
    {
        public const string Prefix = "SVN";

        private CommandProcessor cmd;
        private string timeFormat;

        public SubversionParser(string repoDirectory, string timeFormat) {
            this.cmd = new CommandProcessor("svn.exe", repoDirectory);
            this.timeFormat = timeFormat;
        }

        private bool IsAvailableVCSExecutable() =>
            cmd.IsZeroExitCode("--version");

        private bool IsUnderControl() =>
            this.cmd.IsZeroExitCode("info");

        private string GetURL() =>
            this.cmd.GetOut("info --show-item url");

        private string GetLastRevNumber() =>
            this.cmd.GetOut("info --show-item revision");

        private string GetLastRevisionDate() {
            var unformatted = this.cmd.GetOut("info --show-item last-changed-date");
            return DateTime.Parse(unformatted).ToString(this.timeFormat);
        }

        public bool IsParseble() {
            return this.IsAvailableVCSExecutable() && this.IsUnderControl();
        }

        public Dictionary<string, string> Generate() {
            var dict = new Dictionary<string, string>();
            dict.Add($"{Prefix}_LAST_REVISION_DATE", GetLastRevisionDate());
            dict.Add($"{Prefix}_LAST_REVISION_NUMBER", GetLastRevNumber());
            dict.Add($"{Prefix}_REPO_URL", GetURL());
            return dict;
        }
    }
}
