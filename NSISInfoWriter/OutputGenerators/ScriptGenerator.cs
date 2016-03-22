using NSISInfoWriter.InfoParsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSISInfoWriter.OutputGenerators
{
    public abstract class ScriptGenerator
    {
        private ScriptGeneratorOptions Options { get; }
        private List<IParser> Parsers { get; }

        virtual public string CommentChar { get; } = ";";

        protected ScriptGenerator(ScriptGeneratorOptions options) {
            this.Parsers = new List<IParser>();
            this.Options = options;
        }

        public void AddParser(IParser parser) {
            ConsoleLogger.LogInfo($"Parser {parser.GetType().Name} was added");
            this.Parsers.Add(parser);
        }

        /// <summary>
        /// Prepend prefix and upcase source key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string GetKeyName(string key) {
            bool isEmptyPrefix = String.IsNullOrEmpty(this.Options.Prefix);
            var prefixed = isEmptyPrefix ? key : $"{this.Options.Prefix}_{key}";
            return prefixed.ToUpperInvariant();
        }

        protected virtual bool IsAllowedItem(string key, string value) {
            bool isEmpty = String.IsNullOrWhiteSpace(value) || value == "0";
            return this.Options.IgnoreEmpty || !isEmpty;
        }

        protected abstract string ProcessItem(string key, string value);

        public string GetOutput() {
            var sb = new StringBuilder();
            foreach (var parser in this.Parsers.Where(p => p.IsParseble())) {
                ConsoleLogger.LogInfo($"Processing '{parser.GetType().Name}'...");
                foreach (var item in parser.Generate()) {
                    if (!this.IsAllowedItem(item.Key, item.Value)) {
                        continue;
                    }
                    sb.AppendLine(this.ProcessItem(item.Key, item.Value));
                }
            }
            return String.Join(Environment.NewLine, sb.ToString());
        }
    }
}
