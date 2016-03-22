using System;

namespace NSISInfoWriter.OutputGenerators
{
    public class NsisScriptGenerator : ScriptGenerator
    {
        public NsisScriptGenerator(ScriptGeneratorOptions options) : base(options) { }

        protected override string ProcessItem(string key, string value) {
            value = value.Replace(@"""", @"$\""");
            return $"!define {this.GetKeyName(key)} \"{value}\"";
        }
    }
}
