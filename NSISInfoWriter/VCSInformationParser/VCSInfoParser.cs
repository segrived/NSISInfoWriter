using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace NSISInfoWriter.VCSInformationParser
{
    public abstract class VCSInfoParser
    {
        protected string workingDirectory;
        protected string timeFormat;
        private string vcsExec;

        protected Dictionary<string, string> generated = new Dictionary<string, string>();

        public VCSInfoParser(string vcsExec, string wd, string timeFormat) {
            this.vcsExec = vcsExec;
            this.workingDirectory = wd;
            this.timeFormat = timeFormat;
        }

        protected string CmdOutput(string args) {
            return Helpers.GetCommandOutput(this.vcsExec, args, this.workingDirectory);
        }

        abstract public string Prefix {get; }

        virtual public bool IsAvailableVCSExecutable()
        {
            int code = Helpers.GetExitCode(this.vcsExec, "--version");
            return code == 0;
        }

        /// <summary>
        /// Indicate working directory under VCS control or not
        /// </summary>
        /// <returns></returns>
        abstract public bool IsUnderControl();

        abstract protected void ParseInformation();

        // TODO: replace with proper implementation
        public Dictionary<string, string> GetInformation() {
            this.generated.Clear();
            this.ParseInformation();
            return this.generated;
        }

        protected void AddInformation(string key, string value) {
            key = string.Format("{0}_{1}", this.Prefix, key);
            this.generated.Add(key, value);
        }
    }
}
