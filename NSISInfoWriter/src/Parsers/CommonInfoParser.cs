using System;
using System.Collections.Generic;
using System.IO;

namespace NSISInfoWriter.Parsers
{
    public enum FileSizeInformationUnit : long //-V3059
    {
        B = 1L,
        Kb = 1024L,
        Mb = 1048576L,
        Gb = 1073741824L
    }

    public class CommonInfoParser : IParser
    {
        private readonly string _fileName;
        private readonly FileInfo _fileInfo;
        private readonly string _dateFormat;

        public CommonInfoParser(string fileName, string dateFormat) {
            this._fileName = fileName;
            this._fileInfo = new FileInfo(fileName);
            this._dateFormat = dateFormat;
        }

        private string GetFileName() => this._fileInfo.Name;

        private string GetFileLength(FileSizeInformationUnit unit) {
            var size = this._fileInfo.Length / (long)unit;
            return size.ToString();
        }

        private string GetFileCreationTime() =>
            this._fileInfo.CreationTime.ToString(this._dateFormat);

        private string GetFileLastWriteTime() =>
            this._fileInfo.LastWriteTime.ToString(this._dateFormat);

        // Returns empty string, if invalid PE image
        private string GetImageArchitecture() {
            try {
                var arch = Helpers.GetImageArchitecture(this._fileName);
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
                { "FILE_LENGTH_KB"      , this.GetFileLength(FileSizeInformationUnit.Kb) },
                { "FILE_LENGTH_MB"      , this.GetFileLength(FileSizeInformationUnit.Mb) },
                { "FILE_CREATION_DATE"  , this.GetFileCreationTime() },
                { "FILE_LAST_WRITE_TIME", this.GetFileLastWriteTime() },
                { "FILE_ARCHITECTURE"   , this.GetImageArchitecture() }
            };
        }
    }
}
