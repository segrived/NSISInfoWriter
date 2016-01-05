using System.Collections.Generic;
using System.Diagnostics;

namespace NSISInfoWriter.InfoParsers
{
    public enum VersionType
    {
        Product, File
    }

    public class VersionInfoParser
    {
        public FileVersionInfo VersionInformation { get; private set; }
        public string VersionFormat { get; private set; }

        public VersionInfoParser(string fileName, string versionFormat) {
            this.VersionInformation = FileVersionInfo.GetVersionInfo(fileName);
            this.VersionFormat = versionFormat;
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

        private string GetFormattedVersion(VersionType versionType) {

            int minorPart, majorPart, buildPart, privatePart;
            if (versionType == VersionType.File) {
                minorPart = this.VersionInformation.FileMinorPart;
                majorPart = this.VersionInformation.FileMajorPart;
                buildPart = this.VersionInformation.FileBuildPart;
                privatePart = this.VersionInformation.FilePrivatePart;
            } else {
                minorPart = this.VersionInformation.ProductMinorPart;
                majorPart = this.VersionInformation.ProductMajorPart;
                buildPart = this.VersionInformation.ProductBuildPart;
                privatePart = this.VersionInformation.ProductPrivatePart;
            }
            var formattedString = this.VersionFormat
                .Replace("%mi%", minorPart.ToString())
                .Replace("%mj%", majorPart.ToString())
                .Replace("%b%", buildPart.ToString())
                .Replace("%p%", privatePart.ToString());
            return formattedString;
        }

        public Dictionary<string, string> Generate() {
            return new Dictionary<string, string> {
                { "VI_FILEVERSION"          , this.GetFileVersion()},
                { "VI_FMT_FILEVERSION"      , this.GetFormattedVersion(VersionType.File) },
                { "VI_PRODUCTIONVERSION"    , this.GetProductVersion() },
                { "VI_FMT_PRODUCTIONVERSION", this.GetFormattedVersion(VersionType.Product) },
                { "VI_COPYRIGHTS"           , this.GetCopyrights() },
                { "VI_DESCRIPTION"          , this.GetDescription() },
                { "VI_COMPANY"              , this.GetCompany() },
                { "VI_PRODUCT_NAME"         , this.GetProductName() }
            };
        }
    }
}