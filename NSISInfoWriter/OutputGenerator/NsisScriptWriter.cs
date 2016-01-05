using System;
using System.Collections.Generic;

namespace NSISInfoWriter.OutputGenerator
{
    public class NsisScriptWriter
    {
        private readonly IScriptWriter writer;
        private readonly List<string> outputContent;

        public string Prefix { get; private set; }
        public bool IncludeEmpty { get; private set; }

        public NsisScriptWriter(IScriptWriter writer, string prefix, bool includeEmpty) {
            this.outputContent = new List<string>();
            this.writer = writer;

            this.Prefix = prefix;
            this.IncludeEmpty = includeEmpty;
        }

        public void Add(string key, string value) {
            if (value == null) {
                value = String.Empty;
            }

            if (!this.IncludeEmpty && (String.IsNullOrWhiteSpace(value) || value == "0")) {
                return;
            }

            // prefixes
            if (!String.IsNullOrWhiteSpace(this.Prefix)) {
                key = String.Format("{0}_{1}", Prefix, key);
            }
            // escaping quotes (based on http://nsis.sourceforge.net/Docs/Chapter4.html)
            // looks ugly, but anyway
            value = value.Replace(@"""", @"$\""");
            key = key.ToUpperInvariant();
            string outLine = String.Format("!define {0} \"{1}\"", key, value);
            this.outputContent.Add(outLine);
        }

        public void AddRange(Dictionary<string, string> dict) {
            foreach (var kvp in dict) {
                this.Add(kvp.Key, kvp.Value);
            }
        }

        public void Save() {
            var output = String.Join(Environment.NewLine, this.outputContent);
            this.writer.Write(output);
        }
    }
}
