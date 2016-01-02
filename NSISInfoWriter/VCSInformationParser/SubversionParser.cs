namespace NSISInfoWriter.VCSInformationParser
{
    class SubversionParser : VCSInfoParser
    {
        public SubversionParser(string wd) : base("svn", wd) { }

        public override string Prefix { get; } = "SVN";

        public override bool IsUnderControl() {
            var res = Helpers.GetExitCode("svn", "info", this.workingDirectory);
            return res == 0;
        }

        private string GetLastRevisionDate() {
            return this.CmdOutput("info --show-item last-changed-date");
        }

        private string GetURL() {
            return this.CmdOutput("info --show-item url");
        }

        private string GetLastRevisionNumber() {
            return this.CmdOutput("info --show-item revision");
        }

        protected override void ParseInformation() {
            this.AddInformation("LAST_REVISION_DATE", this.GetLastRevisionDate());
            this.AddInformation("LAST_REVISION_NUMBER", this.GetLastRevisionNumber());
            this.AddInformation("REPO_URL", this.GetURL());
        }
    }
}
