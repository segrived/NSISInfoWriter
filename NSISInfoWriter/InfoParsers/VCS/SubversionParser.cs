using System;

namespace NSISInfoWriter.InfoParsers.VCS
{
    class SubversionParser : VCSInfoParser
    {
        public SubversionParser(string wd, string timeFormat) : base("svn", wd, timeFormat) { }

        public override string Prefix { get; } = "SVN";

        public override bool IsUnderControl() {
            var res = this.cmdProcessor.GetExitCode("info");
            return res == 0;
        }

        private string GetLastRevisionDate() {
            var unformatted = this.cmdProcessor.GetCommandOutput("info --show-item last-changed-date");
            return DateTime.Parse(unformatted).ToString(this.timeFormat);
        }

        private string GetURL() {
            return this.cmdProcessor.GetCommandOutput("info --show-item url");
        }

        private string GetLastRevisionNumber() {
            return this.cmdProcessor.GetCommandOutput("info --show-item revision");
        }

        protected override void ParseInformation() {
            this.AddInformation("LAST_REVISION_DATE", this.GetLastRevisionDate());
            this.AddInformation("LAST_REVISION_NUMBER", this.GetLastRevisionNumber());
            this.AddInformation("REPO_URL", this.GetURL());
        }
    }
}
