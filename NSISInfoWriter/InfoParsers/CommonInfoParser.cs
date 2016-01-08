using System;
using System.Collections.Generic;
using System.IO;

namespace NSISInfoWriter.InfoParsers
{
    public enum FileSizeInformationUnit : long
    {
        B = 1L,
        KB = 1024L,
        MB = 1048576L,
        GB = 1073741824L
    }

    public class CommonInfoParser : IParser
    {
        private string fileName;
        private FileInfo fileInfo;
        private string dateFormat;

        public CommonInfoParser(string fileName, string dateFormat) {
            this.fileName = fileName;
            this.fileInfo = new FileInfo(fileName);
            this.dateFormat = dateFormat;
        }

        private string GetFileName() => fileInfo.Name;

        private string GetFileLength(FileSizeInformationUnit unit) {
            var size = fileInfo.Length / (long)unit;
            return size.ToString();
        }

        private string GetFileCreationTime() =>
            fileInfo.CreationTime.ToString(this.dateFormat);

        private string GetFileLastWriteTime() =>
            fileInfo.LastWriteTime.ToString(this.dateFormat);

        // Returns empty string, if invalid PE image
        private string GetImageArchitecture() {
            try {
                var arch = Helpers.GetImageArchitecture(this.fileName);
                return Helpers.ImageArchitectureToString(arch);
            } catch (BadImageFormatException) {
                return String.Empty;
            }
        }

        // IParser Implementation

        public bool IsParseble() => true;

        public Dictionary<string, string> Generate() {
            return new Dictionary<string, string> {
                { "FILE_NAME"           , this.GetFileName() },
                { "FILE_LENGTH"         , this.GetFileLength(FileSizeInformationUnit.B) },
                { "FILE_LENGTH_KB"      , this.GetFileLength(FileSizeInformationUnit.KB) },
                { "FILE_LENGTH_MB"      , this.GetFileLength(FileSizeInformationUnit.MB) },
                { "FILE_CREATION_DATE"  , this.GetFileCreationTime() },
                { "FILE_LAST_WRITE_TIME", this.GetFileLastWriteTime() },
                { "FILE_ARCHITECTURE"   , this.GetImageArchitecture() }
            };
        }
    }
}
