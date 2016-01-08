using System;
using System.Collections.Generic;

namespace NSISInfoWriter.InfoParsers.VCS
{
    public class GitParser : IParser
    {
        public const string Prefix = "GIT";

        private readonly CommandProcessor cmdProcessor;
        private string timeFormat;

        public GitParser(string repoDirectory, string timeFormat) {
            this.cmdProcessor = new CommandProcessor("git.exe", repoDirectory);
            this.timeFormat = timeFormat;
        }

        private bool IsAvailableVCSExecutable() =>
            this.cmdProcessor.IsZeroExitCode("--version");

        private bool IsUnderControl() =>
            cmdProcessor.GetOut("rev-parse --is-inside-work-tree") == "true";

        private string GetLastCommitHash(bool isShort = true) {
            var fmt = isShort ? "%h" : "%H";
            return cmdProcessor.GetOut($"log --pretty=format:{fmt} -n 1");
        }

        private string GetLastCommitDate() {
            var unformatted = cmdProcessor.GetOut("log --pretty=format:%ai -n 1");
            return DateTime.Parse(unformatted).ToString(this.timeFormat);
        }


        public bool IsParseble() {
            return this.IsAvailableVCSExecutable() && this.IsUnderControl();
        }

        public Dictionary<string, string> Generate() {
            var dict = new Dictionary<string, string>();
            dict.Add($"{Prefix}_LAST_COMMIT_HASH_LONG", GetLastCommitHash(false));
            dict.Add($"{Prefix}_LAST_COMMIT_HASH_SHORT", GetLastCommitHash(true));
            dict.Add($"{Prefix}_LAST_COMMIT_DATE", GetLastCommitDate());
            dict.Add($"{Prefix}_USERNAME", cmdProcessor.GetOut("config user.name"));
            dict.Add($"{Prefix}_USEREMAIL", cmdProcessor.GetOut("config user.email"));
            return dict;
        }
    }
}
