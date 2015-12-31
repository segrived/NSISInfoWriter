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

        private Dictionary<string, string> _definesDict;

        public NsisScriptGenerator(string outputFileName, string prefix, bool ignoreEmpty) {
            this.OutputFileName = outputFileName;
            this.Prefix = prefix;
            this.IgnoreEmpty = ignoreEmpty;

            this._definesDict = new Dictionary<string, string>();
        }

        public void Add(string key, string value) {
            if (this.IgnoreEmpty && String.IsNullOrWhiteSpace(value)) {
                return;
            }
            if (!String.IsNullOrWhiteSpace(this.Prefix)) {
                key = String.Format("{0}_{1}", Prefix, key);
            }
            key = key.ToUpperInvariant();
            this._definesDict.Add(key, value);
        }

        public void Save() {
            using (var writer = File.CreateText(this.OutputFileName)) {
                foreach (var kvp in this._definesDict) {
                    string line = String.Format("!define {0} {1}", kvp.Key, kvp.Value);
                    writer.WriteLine(line);
                }
            }
        }
    }
}
