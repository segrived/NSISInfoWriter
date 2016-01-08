using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace NSISInfoWriter.InfoParsers
{
    public class JarParser : IParser
    {
        private string FileName { get; set; }

        // minimum size of zip file, based on wiki information
        private const int MinimumZipFileSize = 22;
        // Jar (zip) signature = 50 4B 03 04
        private const int JarSignature = 0x04034B50;

        private readonly Regex lineRegex =
            new Regex("^(?<key>.*):\\s*\"(?<value>.*)\"$", RegexOptions.Compiled);

        public JarParser(string fileName) {
            this.FileName = fileName;
        }

        private string ReadManifestFile() {
            using (var jarStream = new FileStream(this.FileName, FileMode.Open, FileAccess.Read)) {
                var e = new ZipArchive(jarStream).GetEntry("META-INF/MANIFEST.MF");
                if (e == null) {
                    return String.Empty;
                }
                using (var entryStream = e.Open())
                using (var reader = new StreamReader(entryStream)) {
                    return reader.ReadToEnd();
                }
            }
        }

        private string ProcessKey(string key) {
            var result = key.ToUpper().Replace("-", "_");
            return $"VI_{result}";
        }

        public bool IsParseble() {
            if (new FileInfo(this.FileName).Length < MinimumZipFileSize) {
                return false;
            }
            var sigReader = new SignatureReader(this.FileName);
            return sigReader.Read32() == JarSignature;
        }

        public Dictionary<string, string> Generate() {
            List<string> manifestContent;
            var dict = new Dictionary<string, string>();

            // TODO: Refactor this code
            try {
                manifestContent = this.ReadManifestFile().SplitByLines();
            } catch (Exception) {
                return dict;
            }
            foreach (var line in manifestContent) {
                var match = lineRegex.Match(line);
                if (!match.Success) {
                    continue;
                }
                var key = match.Groups["key"].Value;
                if (string.IsNullOrWhiteSpace(key)) {
                    continue;
                }
                var value = match.Groups["value"].Value;
                dict.Add(this.ProcessKey(key), value);
            }
            return dict;
        }
    }
}
