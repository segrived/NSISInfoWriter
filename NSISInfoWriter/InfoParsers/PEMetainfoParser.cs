using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NSISInfoWriter.InfoParsers
{
    public enum VersionType
    {
        Product, File
    }

    public class PEMetainfoParser
    {
        private string FileName { get; set; }
        private FileVersionInfo VersionInformation { get; set; }
        private string VersionFormat { get; set; }

        // PE image signature = 4D 5A
        private const short PESignature = 0x5A4D;

        public PEMetainfoParser(string fileName, string versionFormat) {
            this.VersionInformation = FileVersionInfo.GetVersionInfo(fileName);
            this.VersionFormat = versionFormat;
            this.FileName = fileName;
        }

        public bool IsValid() {
            var sigReader = new SignatureReader(this.FileName);
            return sigReader.ReadSignature16() == PESignature;
        }

        private string GetFileVersion() {
            return this.VersionInformation.FileVersion;
        }

        private string GetProductVersion() {
            return this.VersionInformation.ProductVersion;
        }

        private string GetCopyrights() {
            return this.VersionInformation.LegalCopyright;
        }

        private string GetDescription() {
            return this.VersionInformation.FileDescription;
        }

        private string GetCompany() {
            return this.VersionInformation.CompanyName;
        }

        private string GetProductName() {
            return this.VersionInformation.ProductName;
        }

        private bool IsFileVersionPresent() {
            return this.VersionInformation.FileMajorPart != 0
                || this.VersionInformation.FileMinorPart != 0
                || this.VersionInformation.FileBuildPart != 0
                || this.VersionInformation.FilePrivatePart != 0;
        }

        private bool IsProductVersionPresent() {
            return this.VersionInformation.ProductMajorPart != 0
                || this.VersionInformation.ProductMinorPart != 0
                || this.VersionInformation.ProductBuildPart != 0
                || this.VersionInformation.ProductPrivatePart != 0;
        }


        private string GetFormattedProductVersion() {
            if (!this.IsProductVersionPresent()) {
                return String.Empty;
            }
            int major = this.VersionInformation.ProductMajorPart;
            int minor = this.VersionInformation.ProductMinorPart;
            int build = this.VersionInformation.ProductBuildPart;
            int priv = this.VersionInformation.ProductPrivatePart;
            return FormatVersionParts(major, minor, build, priv);
        }

        private string GetFormattedFileVersion() {
            if (!this.IsFileVersionPresent()) {
                return String.Empty;
            }
            int major = this.VersionInformation.FileMajorPart;
            int minor = this.VersionInformation.FileMinorPart;
            int build = this.VersionInformation.FileBuildPart;
            int priv = this.VersionInformation.FilePrivatePart;
            return FormatVersionParts(major, minor, build, priv);
        }


        private string FormatVersionParts(int major, int minor, int build, int priv) {
            var formattedString = this.VersionFormat
                .Replace("%mj%", major.ToString())
                .Replace("%mi%", minor.ToString())
                .Replace("%b%", build.ToString())
                .Replace("%p%", priv.ToString());
            return formattedString;
        }

        public Dictionary<string, string> Generate() {
            return new Dictionary<string, string> {
                { "VI_FILEVERSION"          , this.GetFileVersion()},
                { "VI_FMT_FILEVERSION"      , this.GetFormattedFileVersion() },
                { "VI_PRODUCTIONVERSION"    , this.GetProductVersion() },
                { "VI_FMT_PRODUCTIONVERSION", this.GetFormattedProductVersion() },
                { "VI_COPYRIGHTS"           , this.GetCopyrights() },
                { "VI_DESCRIPTION"          , this.GetDescription() },
                { "VI_COMPANY"              , this.GetCompany() },
                { "VI_PRODUCT_NAME"         , this.GetProductName() }
            };
        }
    }
}