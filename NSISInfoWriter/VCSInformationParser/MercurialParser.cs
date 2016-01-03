using System;

namespace NSISInfoWriter.VCSInformationParser
{
    public class MercurialParser : VCSInfoParser
    {
        public MercurialParser(string wd, string timeFormat) : base("hg", wd, timeFormat) { }

        public override string Prefix { get; } = "HG";

        private string GetLastCommitHash(bool isShort = true) {
            var command = isShort ? "parent --template {node|short}" : "parent --template {node}";
            return this.cmdProcessor.GetCommandOutput(command);
        }

        private string GetLastCommitDate() {
            var unformatted = this.cmdProcessor.GetCommandOutput("log --template {date|isodatesec} -l 1");
            return DateTime.Parse(unformatted).ToString(this.timeFormat);
        }

        private string GetUserName() {
            return this.cmdProcessor.GetCommandOutput("showconfig ui.username");
        }


        public override bool IsUnderControl() {
            var res = this.cmdProcessor.GetExitCode("--cwd . root");
            return res == 0;
        }

        protected override void ParseInformation() {
            this.AddInformation("LAST_COMMIT_HASH_LONG", this.GetLastCommitHash(false));
            this.AddInformation("LAST_COMMIT_HASH_SHORT", this.GetLastCommitHash(true));
            this.AddInformation("LAST_COMMIT_DATE", this.GetLastCommitDate());
            this.AddInformation("USERNAME", this.GetUserName());
        }
    }
}
