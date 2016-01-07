using System;
using System.IO;
using CommandLine;
using NSISInfoWriter.OutputGenerator;
using NSISInfoWriter.InfoParsers;
using NSISInfoWriter.InfoParsers.VCS;

namespace NSISInfoWriter
{
    class Program
    {
        public const string StdoutFileName = "stdout";

        static void WriteResults(CLIOptions o) {
            if (o.ExcludeCommon && o.ExcludeVCS && o.ExcludeVersion) {
                Helpers.ShowError("Error: Everything was excluded, nothing to do");
                Environment.Exit(1);
            }

            var fullFileName = Helpers.GetFullFileName(o.InputFile);

            var writer = (o.OutputFile.Equals(StdoutFileName, StringComparison.OrdinalIgnoreCase))
                ? (IScriptWriter)new ConsoleWriter()
                : (IScriptWriter)new FileWriter(o.OutputFile, o.PrependToFile);

            try {
                var generator = new NsisScriptWriter(writer, o.Prefix, o.IncludeEmpty);
                generator.Add("SCRIPT_GENERATE_TIME", DateTime.Now.ToString(o.DateFormat));

                // common file information
                if (!o.ExcludeCommon) {
                    var commonInfo = new CommonInfoParser(fullFileName, o.DateFormat);
                    generator.AddRange(commonInfo.Generate());
                }

                // version file information
                if (!o.ExcludeVersion) {
                    var versionInfo = new PEMetainfoParser(fullFileName, o.VersionFormat);
                    if (versionInfo.IsValid()) {
                        generator.AddRange(versionInfo.Generate());
                    }
                    var jarInfoParser = new JarMetainfoParser(fullFileName);
                    if (jarInfoParser.IsValid()) {
                        generator.AddRange(jarInfoParser.Generate());
                    }
                }

                var repoPath = String.IsNullOrEmpty(o.RepoPath)
                    ? Path.GetDirectoryName(fullFileName)
                    : o.RepoPath;

                if ((!o.ExcludeVCS) && Directory.Exists(repoPath)) {

                    // git related information
                    var git = new GitParser(repoPath, o.DateFormat);
                    if (git.IsAvailableVCSExecutable() && git.IsUnderControl()) {
                        generator.AddRange(git.GetInformation());
                    }

                    // mercurial related information
                    var hg = new MercurialParser(repoPath, o.DateFormat);
                    if (hg.IsAvailableVCSExecutable() && hg.IsUnderControl()) {
                        generator.AddRange(hg.GetInformation());
                    }

                    // subversion related information
                    var svn = new SubversionParser(repoPath, o.DateFormat);
                    if (svn.IsAvailableVCSExecutable() && svn.IsUnderControl()) {
                        generator.AddRange(svn.GetInformation());
                    }
                }

                generator.Save();
            } catch (FileNotFoundException) {
                Helpers.ShowError($"File '{o.InputFile}' not found, exiting");
            } catch (IOException) {
                Helpers.ShowError($"IOExeption: '{o.InputFile}', exiting");
            } catch (Exception ex) {
                Helpers.ShowError($"Error while generating process: {ex.Message}, exiting");
            }
        }

        static void Main(string[] args) {
            Parser.Default.ParseArguments<CLIOptions>(args).WithParsed(WriteResults);
        }
    }
}
