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



        public FileInfoParser(string fileName) {
            this.FileName = fileName;
            this.FileInfo = new FileInfo(this.FileName);
            this.VersionInfo = FileVersionInfo.GetVersionInfo(this.FileName);
            this.WorkingDirectory = Path.GetDirectoryName(this.FileName);
        }
    }
}
