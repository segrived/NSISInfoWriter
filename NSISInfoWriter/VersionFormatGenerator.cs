using System.Collections.Generic;
using System.Diagnostics;

namespace NSISInfoWriter
{
    public enum VersionType {
        PRODUCT, FILE
    }

    public class VersionFormatGenerator
    {
        private FileVersionInfo versionInfo;

        public VersionFormatGenerator(FileVersionInfo info) {
            this.versionInfo = info;
        }

        public string FormatVersion(VersionType t, string format) {
            
            int minorPart, majorPart, buildPart, privatePart;
            if(t == VersionType.FILE) {
                minorPart   = this.versionInfo.FileMinorPart;
                majorPart   = this.versionInfo.FileMajorPart;
                buildPart   = this.versionInfo.FileBuildPart;
                privatePart = this.versionInfo.FilePrivatePart;
            } else {
                minorPart   = this.versionInfo.ProductMinorPart;
                majorPart   = this.versionInfo.ProductMajorPart;
                buildPart   = this.versionInfo.ProductBuildPart;
                privatePart = this.versionInfo.ProductPrivatePart;
            }
            var formattedString = format
                .Replace("minor",   minorPart.ToString())
                .Replace("major",   majorPart.ToString())
                .Replace("build",   buildPart.ToString())
                .Replace("private", privatePart.ToString());
            return formattedString;
        }
    }
}
