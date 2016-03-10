using System;
using System.Collections.Generic;

namespace NSISInfoWriter.InfoParsers.VCS
{
    public class GitParser : IParser
    {
        public const string Prefix = "GIT";

        private readonly CommandProcessor cmdProcessor;
        private readonly string timeFormat;
        
        public GitParser(string repoDirectory, string timeFormat) {
            this.cmdProcessor = new CommandProcessor("git.exe", repoDirectory);
            this.timeFormat = timeFormat;
        }

        private bool IsAvailableVCSExecutable() =>
            this.cmdProcessor.IsZeroExitCode("--version");

        private bool IsUnderControl() => this.cmdProcessor.GetOut("rev-parse --is-inside-work-tree") == "true";
        
        private string GetLastCommitHash(bool isShort = true) {
            var fmt = isShort ? "%h" : "%H";
            return this.cmdProcessor.GetOut($"log --pretty=format:{fmt} -n 1");
        }

        private string GetLastCommitDate() {
            var unformatted = this.cmdProcessor.GetOut("log --pretty=format:%ai -n 1");
            return DateTime.Parse(unformatted).ToString(this.timeFormat);
        }


        public bool IsParseble() {
            return this.IsAvailableVCSExecutable() && this.IsUnderControl();
        }

        public Dictionary<string, string> Generate() {
            var dict = new Dictionary<string, string>();
            dict.Add($"{Prefix}_LAST_COMMIT_HASH_LONG", this.GetLastCommitHash(false));
            dict.Add($"{Prefix}_LAST_COMMIT_HASH_SHORT", this.GetLastCommitHash(true));
            dict.Add($"{Prefix}_LAST_COMMIT_DATE", this.GetLastCommitDate());
            dict.Add($"{Prefix}_USERNAME", this.cmdProcessor.GetOut("config user.name"));
            dict.Add($"{Prefix}_USEREMAIL", this.cmdProcessor.GetOut("config user.email"));
            return dict;
        }
    }
}
