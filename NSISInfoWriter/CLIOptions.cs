using CommandLine;

namespace NSISInfoWriter
{
    public class CLIOptions
    {
        [Option('i', "input", Required = true, HelpText = "Input executable file")]
        public string InputFile { get; set; }
        [Option('o', "output", Required = true, HelpText = "Output file")]
        public string OutputFile { get; set; }
        [Option('g', "ex-git", Default = false, HelpText = "Exclude git related information from output")]
        public bool ExcludeGit { get; set; }
        [Option('c', "ex-common", Default = false, HelpText = "Exclude common file information from output (size, name, etc.)")]
        public bool ExcludeCommon { get; set; }
        [Option('v', "ex-version", Default = false, HelpText = "Exclude version information from output")]
        public bool ExcludeVersion { get; set; }
        [Option('p', "prefix", Default = "", HelpText = "Constants prefix in output script")]
        public string Prefix { get; set; }
        [Option('e', "ignore-empty", Default = false, HelpText = "Empty values will be rejected from output")]
        public bool IgnoreEmpty { get; set; }
    }
}
