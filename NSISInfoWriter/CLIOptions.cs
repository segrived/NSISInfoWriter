using CommandLine;

namespace NSISInfoWriter
{
    public class CLIOptions
    {
        [Option('i', "input", Required = true, HelpText = "Input executable file")]
        public string InputFile { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output file")]
        public string OutputFile { get; set; }

        [Option('s', "ex-vcs", Default = false, HelpText = "Exclude version control system(s) related information from output")]
        public bool ExcludeVCS { get; set; }

        [Option('c', "ex-common", Default = false, HelpText = "Exclude common file information from output (size, name, etc.)")]
        public bool ExcludeCommon { get; set; }

        [Option('v', "ex-version", Default = false, HelpText = "Exclude version information from output")]
        public bool ExcludeVersion { get; set; }

        [Option('p', "prefix", Default = "", HelpText = "Constants prefix in output script")]
        public string Prefix { get; set; } 

        [Option('r', "repo-path", Default = "", HelpText = "Path to VCS repository")]
        public string RepoPath { get; set; }

        [Option('e', "ignore-empty", Default = false, HelpText = "Empty values will be rejected from output")]
        public bool IgnoreEmpty { get; set; }

        [Option('f', "version-format", Default = "%mj%.%mi%.%b%.%p%", HelpText = "Version information format")]
        public string VersionFormat {get; set; }

        [Option('d', "date-format", Default = "yyyyMMddHHmmss", HelpText = "Date/time format")]
        public string DateFormat { get; set; }
    }
}
