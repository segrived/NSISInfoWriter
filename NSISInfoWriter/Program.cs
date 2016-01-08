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
            ConsoleLogger.IsEnabled = o.DebugMode;
            ConsoleLogger.LogInfo("Initialised, start processing");
            if (o.ExcludeCommon && o.ExcludeVCS && o.ExcludeVersion) {
                Helpers.ShowError("Error: Everything was excluded, nothing to do");
                Environment.Exit(1);
            }

            var fullFileName = Helpers.GetFullFileName(o.InputFile);
            ConsoleLogger.LogInfo($"Input file: {fullFileName}");
            ConsoleLogger.LogInfo($"Output file: {o.OutputFile}");

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
                } else {
                    ConsoleLogger.LogWarn("Common information was excluded from output");
                }

                // version file information
                if (!o.ExcludeVersion) {
                    var versionInfo = new PEMetainfoParser(fullFileName, o.VersionFormat);
                    if (versionInfo.IsValid()) {
                        ConsoleLogger.LogInfo("Input file is valid PE image");
                        generator.AddRange(versionInfo.Generate());
                    } else {
                        ConsoleLogger.LogWarn("Input file isn't PE image");
                    }
                    var jarInfoParser = new JarMetainfoParser(fullFileName);
                    if (jarInfoParser.IsValid()) {
                        ConsoleLogger.LogInfo("Input file is valid JAR archive");
                        generator.AddRange(jarInfoParser.Generate());
                    } else {
                        ConsoleLogger.LogWarn("Input file isn't JAR archive");
                    }
                } else {
                    ConsoleLogger.LogWarn("Meta-information was excluded from output");
                }

                var repoPath = String.IsNullOrEmpty(o.RepoPath)
                    ? Path.GetDirectoryName(fullFileName)
                    : o.RepoPath;

                ConsoleLogger.LogInfo($"Repository path: {repoPath}");

                if ((!o.ExcludeVCS) && Directory.Exists(repoPath)) {

                    // git related information
                    var git = new GitParser(repoPath, o.DateFormat);
                    if (git.IsAvailableVCSExecutable() && git.IsUnderControl()) {
                        ConsoleLogger.LogInfo("GIT repository detected");
                        generator.AddRange(git.GetInformation());
                    } else {
                        ConsoleLogger.LogWarn("Git executable not found ot not a git repository");
                    }

                    // mercurial related information
                    var hg = new MercurialParser(repoPath, o.DateFormat);
                    if (hg.IsAvailableVCSExecutable() && hg.IsUnderControl()) {
                        ConsoleLogger.LogInfo("Mercurial repository detected");
                        generator.AddRange(hg.GetInformation());
                    } else {
                        ConsoleLogger.LogWarn("Mercurial executable not found ot not a mercurial repository");
                    }

                    // subversion related information
                    var svn = new SubversionParser(repoPath, o.DateFormat);
                    if (svn.IsAvailableVCSExecutable() && svn.IsUnderControl()) {
                        ConsoleLogger.LogInfo("SVN repository detected");
                        generator.AddRange(svn.GetInformation());
                    } else {
                        ConsoleLogger.LogWarn("SVN executable not found or not a svn repository");
                    }
                } else {
                    ConsoleLogger.LogWarn("VCS information was excluded from output");
                }
                ConsoleLogger.LogInfo("Done! Now generated content will be written to output");
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
