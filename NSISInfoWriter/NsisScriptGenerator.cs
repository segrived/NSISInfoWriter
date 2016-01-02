using System;
using System.Collections.Generic;
using System.IO;

namespace NSISInfoWriter
{
    public class NsisScriptGenerator
    {
        public string OutputFileName { get; private set; }
        public string Prefix { get; private set; }
        public bool IgnoreEmpty { get; private set; }

        private readonly Dictionary<string, string> definesDict;

        public NsisScriptGenerator(string outputFileName, string prefix, bool ignoreEmpty) {
            this.OutputFileName = outputFileName;
            this.Prefix = prefix;
            this.IgnoreEmpty = ignoreEmpty;
            this.definesDict = new Dictionary<string, string>();
        }

        public void Add(string key, string value) {
            if (value == null) {
                value = String.Empty;
            }
            if (this.IgnoreEmpty && String.IsNullOrWhiteSpace(value)) {
                return;
            }
            if (!String.IsNullOrWhiteSpace(this.Prefix)) {
                key = String.Format("{0}_{1}", Prefix, key);
            }
            key = key.ToUpperInvariant();
            this.definesDict.Add(key, value);
        }

        public void AddRange(Dictionary<string, string> dict) {
            foreach (var kvp in dict) {
                this.definesDict.Add(kvp.Key, kvp.Value);
            }
        }

        public void Save() {
            using (var writer = File.CreateText(this.OutputFileName)) {
                foreach (var kvp in this.definesDict) {
                    // escaping quotes (based on http://nsis.sourceforge.net/Docs/Chapter4.html)
                    // looks ugly, but anyway
                    string value = kvp.Value.Replace(@"""", @"$\""");
                    string line = String.Format("!define {0} \"{1}\"", kvp.Key, value);
                    writer.WriteLine(line);
                }
            }
        }
    }
}
