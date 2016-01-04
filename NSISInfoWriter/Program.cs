using System;
using System.IO;
using CommandLine;
using NSISInfoWriter.OutputGenerator;

namespace NSISInfoWriter
{
    class Program
    {
        public const string DefaultOutputFileExt = "nsh";

        static void WriteResults(CLIOptions o) {
            if (o.ExcludeCommon && o.ExcludeVCS && o.ExcludeVersion) {
                Console.WriteLine("ERROR: Everything was excluded, nothing to do");
                Environment.Exit(1);
            }

            if (!File.Exists(o.InputFile)) {
                Console.WriteLine("ERROR: Input file does not exist");
            }

            var writer = o.OutputFile.Equals("stdout", StringComparison.OrdinalIgnoreCase)
                ? (IScriptWriter)new ConsoleWriter()
                : (IScriptWriter)new FileWriter(o.OutputFile);

            if (!Path.HasExtension(o.OutputFile)) {
                o.OutputFile = String.Format("{0}.{1}", o.OutputFile, DefaultOutputFileExt);
            }

            var generator = new NsisScriptWriter(writer, o.Prefix, o.IgnoreEmpty);

            try {
                generator.Add("SCRIPT_GENERATE_TIME", DateTime.Now.ToString(o.DateFormat));

                // common file information
                if (! o.ExcludeCommon) {
                    var commonInfo = new CommonInfoGenerator(o.InputFile, o.DateFormat);
                    generator.AddRange(commonInfo.Generate());
                }

                // version file information
                if (!o.ExcludeVersion) {
                    var versionInfo = new VersionInfoGenerator(o.InputFile, o.VersionFormat);
                    generator.AddRange(versionInfo.Generate());
                }

                var repoPath = String.IsNullOrEmpty(o.RepoPath)
                    ? Path.GetDirectoryName(o.InputFile)
                    : o.RepoPath;

                if ((!o.ExcludeVCS) && Directory.Exists(repoPath)) {
                    
                    // git related information
                    var git = new VCSInformationParser.GitParser(repoPath, o.DateFormat);
                    if (git.IsAvailableVCSExecutable() && git.IsUnderControl()) {
                        generator.AddRange(git.GetInformation());
                    }

                    // mercurial related information
                    var hg = new VCSInformationParser.MercurialParser(repoPath, o.DateFormat);
                    if (hg.IsAvailableVCSExecutable() && hg.IsUnderControl()) {
                        generator.AddRange(hg.GetInformation());
                    }

                    // subversion related information
                    var svn = new VCSInformationParser.SubversionParser(repoPath, o.DateFormat);
                    if (svn.IsAvailableVCSExecutable() && svn.IsUnderControl()) {
                        generator.AddRange(svn.GetInformation());
                    }
                }

                generator.Save();

            } catch (FileNotFoundException) {
                Console.WriteLine($"File not found: {o.InputFile}, exiting");
            } catch (IOException ex) {
                Console.WriteLine($"IOExeption: {ex.ToString()}, exiting");
            }
        }

        static void Main(string[] args) {
            Parser.Default.ParseArguments<CLIOptions>(args).WithParsed(WriteResults);
        }
    }
}
