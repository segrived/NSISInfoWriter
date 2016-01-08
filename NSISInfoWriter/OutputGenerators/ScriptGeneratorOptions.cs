namespace NSISInfoWriter.OutputGenerators
{
    public class ScriptGeneratorOptions
    {
        /// <summary>
        /// If true, all items with empty keys will be ignored
        /// </summary>
        public bool IgnoreEmpty { get; set; }

        public string Prefix { get; set; }
    }
}
