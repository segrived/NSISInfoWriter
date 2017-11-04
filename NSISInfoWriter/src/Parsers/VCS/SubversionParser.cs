using System;
using System.Collections.Generic;

namespace NSISInfoWriter.Parsers.VCS
{
    class SubversionParser : IParser
    {
        public const string Prefix = "SVN";

        private readonly CommandProcessor _cmd;
        private readonly string _timeFormat;

        public SubversionParser(string repoDirectory, string timeFormat) {
            this._cmd = new CommandProcessor("svn.exe", repoDirectory);
            this._timeFormat = timeFormat;
        }

        private bool IsAvailableVcsExecutable() =>
            this._cmd.IsZeroExitCode("--version");

        private bool IsUnderControl() =>
            this._cmd.IsZeroExitCode("info");

        private string GetUrl() =>
            this._cmd.GetOut("info --show-item url");

        private string GetLastRevNumber() =>
            this._cmd.GetOut("info --show-item revision");

        private string GetLastRevisionDate() {
            var unformatted = this._cmd.GetOut("info --show-item last-changed-date");
            return DateTime.Parse(unformatted).ToString(this._timeFormat);
        }

        public bool IsParseble() {
            return this.IsAvailableVcsExecutable() && this.IsUnderControl();
        }

        public Dictionary<string, string> Generate() {
            var dict = new Dictionary<string, string>();
            dict.Add($"{Prefix}_LAST_REVISION_DATE", this.GetLastRevisionDate());
            dict.Add($"{Prefix}_LAST_REVISION_NUMBER", this.GetLastRevNumber());
            dict.Add($"{Prefix}_REPO_URL", this.GetUrl());
            return dict;
        }
    }
}
