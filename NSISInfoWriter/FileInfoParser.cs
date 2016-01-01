using System.Diagnostics;
using System.IO;

namespace NSISInfoWriter
{
    public class FileInfoParser
    {
        public string FileName { get; private set; }

        public FileInfo FileInfo { get; private set; }
        public FileVersionInfo VersionInfo { get; private set; }
        public string WorkingDirectory { get; private set; }

        public string GetLastCommitHash(bool isShort = true) {
            var fmt = isShort ? "%h" : "%H";
            return Helpers.GetCommandOutput("git", "log --pretty=format:" + fmt + " -n 1", this.WorkingDirectory);
        }

        public string GetLastCommitDate() {
            return Helpers.GetCommandOutput("git", "log --pretty=format:%ai -n 1", this.WorkingDirectory);
        }

        public string GetGitUserName() {
            return Helpers.GetCommandOutput("git", "config user.name", this.WorkingDirectory);
        }

        public string GetGitUserEmail() {
            return Helpers.GetCommandOutput("git", "config user.email", this.WorkingDirectory);
        }

        public FileInfoParser(string fileName) {
            this.FileName = fileName;
            this.FileInfo = new FileInfo(this.FileName);
            this.VersionInfo = FileVersionInfo.GetVersionInfo(this.FileName);
            this.WorkingDirectory = Path.GetDirectoryName(this.FileName);
        }
    }
}
