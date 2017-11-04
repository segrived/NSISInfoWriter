using System;
using System.Collections.Generic;

namespace NSISInfoWriter.Parsers.VCS
{
    public class MercurialParser : IParser
    {
        public const string Prefix = "HG";

        private readonly CommandProcessor _cmdProcessor;
        private readonly string _timeFormat;

        public MercurialParser(string repoDirectory, string timeFormat) {
            this._cmdProcessor = new CommandProcessor("hg.exe", repoDirectory);
            this._timeFormat = timeFormat;
        }

        private bool IsAvailableVcsExecutable() =>
            this._cmdProcessor.IsZeroExitCode("--version");

        private bool IsUnderControl() =>
            this._cmdProcessor.IsZeroExitCode("--cwd . root");

        private string GetLastCommitHash(bool isShort = true) {
            var command = isShort ? "parent --template {node|short}" : "parent --template {node}";
            return this._cmdProcessor.GetOut(command);
        }

        private string GetLastCommitDate() {
            var unformatted = this._cmdProcessor.GetOut("log --template {date|isodatesec} -l 1");
            return DateTime.Parse(unformatted).ToString(this._timeFormat);
        }

        private string GetUserName() =>
            this._cmdProcessor.GetOut("showconfig ui.username");

        public bool IsParseble() {
            return this.IsAvailableVcsExecutable() && this.IsUnderControl();
        }

        public Dictionary<string, string> Generate() {
            var dict = new Dictionary<string, string>();
            dict.Add($"{Prefix}_LAST_COMMIT_HASH_LONG", this.GetLastCommitHash(false));
            dict.Add($"{Prefix}_LAST_COMMIT_HASH_SHORT", this.GetLastCommitHash(true));
            dict.Add($"{Prefix}_LAST_COMMIT_DATE", this.GetLastCommitDate());
            dict.Add($"{Prefix}_USERNAME", this.GetUserName());
            return dict;
        }
    }
}
