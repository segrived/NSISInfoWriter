namespace NSISInfoWriter.Generators
{
    public class InnoScriptGenerator : ScriptGenerator
    {
        public InnoScriptGenerator(ScriptGeneratorOptions options) : base(options) { }

        protected override string ProcessItem(string key, string value) {
            value = value.Replace("\"", "\"\"");
            return $"#define {this.GetKeyName(key)} \"{value}\"";
        }
    }
}
