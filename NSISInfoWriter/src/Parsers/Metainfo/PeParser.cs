using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NSISInfoWriter.Parsers.Metainfo
{
    public class PeParser : IParser
    {
        private const int MinimumPeImageSize = 97;

        private string FileName { get; }
        private FileVersionInfo VInfo { get; }
        private string VersionFormat { get; }

        // PE image signature = 4D 5A
        private const short PeSignature = 0x5A4D;

        public PeParser(string fileName, string versionFormat) {
            this.VInfo = FileVersionInfo.GetVersionInfo(fileName);
            this.VersionFormat = versionFormat;
            this.FileName = fileName;
        }

        private bool IsFileVersionPresent() {
            return this.VInfo.FileMajorPart != 0 || this.VInfo.FileMinorPart != 0
                || this.VInfo.FileBuildPart != 0 || this.VInfo.FilePrivatePart != 0;
        }

        private bool IsProductVersionPresent() {
            return this.VInfo.ProductMajorPart != 0 || this.VInfo.ProductMinorPart != 0
                || this.VInfo.ProductBuildPart != 0 || this.VInfo.ProductPrivatePart != 0;
        }


        /// <summary>
        /// Returns formatted product version information
        /// </summary>
        /// <returns></returns>
        private string GetFormattedProductVersion() {
            if (!this.IsProductVersionPresent()) {
                return String.Empty;
            }
            var fmt = this.FormatVersionParts(this.VInfo.ProductMajorPart,
                                         this.VInfo.ProductMinorPart,
                                         this.VInfo.ProductBuildPart,
                                         this.VInfo.ProductPrivatePart);
            return fmt;
        }

        /// <summary>
        /// Returns formatted file version information
        /// </summary>
        /// <returns></returns>
        private string GetFormattedFileVersion() {
            if (!this.IsFileVersionPresent()) {
                return String.Empty;
            }
            var fmt = this.FormatVersionParts(this.VInfo.FileMajorPart,
                                         this.VInfo.FileMinorPart,
                                         this.VInfo.FileBuildPart,
                                         this.VInfo.FilePrivatePart);
            return fmt;
        }

        private string FormatVersionParts(int major, int minor, int build, int priv) {
            var formattedString = this.VersionFormat
                .Replace("%mj%", major.ToString())
                .Replace("%mi%", minor.ToString())
                .Replace("%b%", build.ToString())
                .Replace("%p%", priv.ToString());
            return formattedString;
        }

        public bool IsParseble() {
            if (new FileInfo(this.FileName).Length < MinimumPeImageSize) {
                return false;
            }
            var sigReader = new SignatureReader(this.FileName);
            return sigReader.Read16() == PeSignature;
        }

        public Dictionary<string, string> Generate() {
            return new Dictionary<string, string> {
                { "VI_FILEVERSION"          , this.VInfo.FileVersion},
                { "VI_PRODUCTIONVERSION"    , this.VInfo.ProductVersion },
                { "VI_COPYRIGHTS"           , this.VInfo.LegalCopyright },
                { "VI_DESCRIPTION"          , this.VInfo.FileDescription },
                { "VI_COMPANY"              , this.VInfo.CompanyName },
                { "VI_PRODUCT_NAME"         , this.VInfo.ProductName },
                // formatted versions
                { "VI_FMT_FILEVERSION"      , this.GetFormattedFileVersion() },
                { "VI_FMT_PRODUCTIONVERSION", this.GetFormattedProductVersion() }
            };
        }
    }
}