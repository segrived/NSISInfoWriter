using System;
using System.Collections.Generic;

namespace NSISInfoWriter.InfoParsers.VCS
{
    public class MercurialParser : IParser
    {
        public const string Prefix = "HG";

        private CommandProcessor cmdProcessor;
        private string timeFormat;

        public MercurialParser(string repoDirectory, string timeFormat) {
            this.cmdProcessor = new CommandProcessor("hg.exe", repoDirectory);
            this.timeFormat = timeFormat;
        }

        private bool IsAvailableVCSExecutable() =>
            cmdProcessor.IsZeroExitCode("--version");

        private bool IsUnderControl() =>
            cmdProcessor.IsZeroExitCode("--cwd . root");

        private string GetLastCommitHash(bool isShort = true) {
            var command = isShort ? "parent --template {node|short}" : "parent --template {node}";
            return cmdProcessor.GetOut(command);
        }

        private string GetLastCommitDate() {
            var unformatted = cmdProcessor.GetOut("log --template {date|isodatesec} -l 1");
            return DateTime.Parse(unformatted).ToString(this.timeFormat);
        }

        private string GetUserName() =>
            cmdProcessor.GetOut("showconfig ui.username");

        public bool IsParseble() {
            return this.IsAvailableVCSExecutable() && this.IsUnderControl();
        }

        public Dictionary<string, string> Generate() {
            var dict = new Dictionary<string, string>();
            dict.Add($"{Prefix}_LAST_COMMIT_HASH_LONG", GetLastCommitHash(false));
            dict.Add($"{Prefix}_LAST_COMMIT_HASH_SHORT", GetLastCommitHash(true));
            dict.Add($"{Prefix}_LAST_COMMIT_DATE", GetLastCommitDate());
            dict.Add($"{Prefix}_USERNAME", GetUserName());
            return dict;
        }
    }
}
