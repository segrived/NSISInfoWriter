using System;

namespace NSISInfoWriter.InfoParsers.VCS
{
    public class GitParser : VCSInfoParser
    {
        public override string Prefix {get; } = "GIT";

        public GitParser(string wd, string timeFormat) : base("git", wd, timeFormat) { }

        public override bool IsUnderControl() {
            // if inside .git directory - false, if directory isn't under git control -
            // GetCommandOutput will return nothing, because stderr is ignored
            var res = this.cmdProcessor.GetCommandOutput("rev-parse --is-inside-work-tree");
            return res == "true";
        }

        private string GetLastCommitHash(bool isShort = true) {
            var fmt = isShort ? "%h" : "%H";
            return this.cmdProcessor.GetCommandOutput("log --pretty=format:" + fmt + " -n 1");
        }

        private string GetLastCommitDate() {
            var unformatted = this.cmdProcessor.GetCommandOutput("log --pretty=format:%ai -n 1");
            return DateTime.Parse(unformatted).ToString(this.timeFormat);
        }

        private string GetUserName() {
            return this.cmdProcessor.GetCommandOutput("config user.name");
        }

        private string GetUserEmail() {
            return this.cmdProcessor.GetCommandOutput("config user.email");
        }

        protected override void ParseInformation() {
            this.AddInformation("LAST_COMMIT_HASH_LONG" , this.GetLastCommitHash(false));
            this.AddInformation("LAST_COMMIT_HASH_SHORT", this.GetLastCommitHash(true));
            this.AddInformation("LAST_COMMIT_DATE"      , this.GetLastCommitDate());
            this.AddInformation("USERNAME"              , this.GetUserName());
            this.AddInformation("USEREMAIL"             , this.GetUserEmail());
        }
    }
}
