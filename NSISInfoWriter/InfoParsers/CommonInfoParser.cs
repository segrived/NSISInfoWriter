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

    public class CommonInfoParser
    {
        private string FileName { get; }
        private FileInfo FileInformation { get; }
        private string DateFormat { get; }

        public CommonInfoParser(string fileName, string dateFormat) {
            this.FileName = fileName;
            this.FileInformation = new FileInfo(fileName);
            this.DateFormat = dateFormat;
        }

        private string GetFileName() {
            return this.FileInformation.Name;
        }

        private string GetFileLength(FileSizeInformationUnit unit) {
            var size = this.FileInformation.Length / (long)unit;
            return size.ToString();
        }

        private string GetFileCreationTime() {
            return this.FileInformation.CreationTime.ToString(this.DateFormat);
        }

        private string GetFileLastWriteTime() {
            return this.FileInformation.LastWriteTime.ToString(this.DateFormat);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns empty string, if invalid PE image</returns>
        private string GetImageArchitecture() {
            try {
                var arch = Helpers.GetImageArchitecture(this.FileName);
                return Helpers.ImageArchitectureToString(arch);
            } catch (BadImageFormatException) {
                return String.Empty;
            }
        }

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
