using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSISInfoWriter.VCSInformationParser
{
    public class MercurialParser : VCSInfoParser
    {
        public MercurialParser(string workingDirectory) : base("hg", workingDirectory) { }

        public override string Prefix { get; } = "HG";

        protected override void ParseInformation() {
            this.AddInformation("LAST_COMMIT_HASH_LONG" , this.GetLastCommitHash(false));
            this.AddInformation("LAST_COMMIT_HASH_SHORT", this.GetLastCommitHash(true));
            this.AddInformation("LAST_COMMIT_DATE"      , this.GetLastCommitDate());
            this.AddInformation("USERNAME"              , this.GetUserName());
        }

        private string GetLastCommitHash(bool isShort = true) {
            var command = isShort ? "parent --template {node|short}" : "parent --template {node}";
            return this.CmdOutput(command);
        }

        private string GetLastCommitDate() {
            return this.CmdOutput("log --template {date|isodate} -l 1");
        }

        private string GetUserName() {
            return this.CmdOutput("showconfig ui.username");
        }


        public override bool IsUnderControl() {
            var res = Helpers.GetExitCode("hg", "--cwd . root", this.workingDirectory);
            return res == 0;
        }
    }
}
