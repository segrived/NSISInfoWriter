using System;
using System.Collections.Generic;

namespace NSISInfoWriter.Parsers.VCS
{
    public class GitParser : IParser
    {
        public const string Prefix = "GIT";

        private readonly CommandProcessor _cmdProcessor;
        private readonly string _timeFormat;
        
        public GitParser(string repoDirectory, string timeFormat) {
            this._cmdProcessor = new CommandProcessor("git.exe", repoDirectory);
            this._timeFormat = timeFormat;
        }

        private bool IsAvailableVcsExecutable() =>
            this._cmdProcessor.IsZeroExitCode("--version");

        private bool IsUnderControl() => this._cmdProcessor.GetOut("rev-parse --is-inside-work-tree") == "true";
        
        private string GetLastCommitHash(bool isShort = true) {
            var fmt = isShort ? "%h" : "%H";
            return this._cmdProcessor.GetOut($"log --pretty=format:{fmt} -n 1");
        }

        private string GetLastCommitDate() {
            var unformatted = this._cmdProcessor.GetOut("log --pretty=format:%ai -n 1");
            return DateTime.Parse(unformatted).ToString(this._timeFormat);
        }


        public bool IsParseble() {
            return this.IsAvailableVcsExecutable() && this.IsUnderControl();
        }

        public Dictionary<string, string> Generate() {
            var dict = new Dictionary<string, string>();
            dict.Add($"{Prefix}_LAST_COMMIT_HASH_LONG", this.GetLastCommitHash(false));
            dict.Add($"{Prefix}_LAST_COMMIT_HASH_SHORT", this.GetLastCommitHash(true));
            dict.Add($"{Prefix}_LAST_COMMIT_DATE", this.GetLastCommitDate());
            dict.Add($"{Prefix}_USERNAME", this._cmdProcessor.GetOut("config user.name"));
            dict.Add($"{Prefix}_USEREMAIL", this._cmdProcessor.GetOut("config user.email"));
            return dict;
        }
    }
}
