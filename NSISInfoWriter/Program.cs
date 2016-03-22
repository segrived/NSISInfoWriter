using System;
using System.IO;
using CommandLine;
using NSISInfoWriter.InfoParsers;
using NSISInfoWriter.InfoParsers.VCS;
using NSISInfoWriter.OutputWriters;
using NSISInfoWriter.OutputGenerators;
using System.Collections.Generic;
using NSISInfoWriter.InfoParsers.Metainfo;

namespace NSISInfoWriter
{
    static class Program
    {
        public const string StdoutFileName = "stdout";

        static void WriteResults(CliOptions o) {
            ConsoleLogger.IsEnabled = o.DebugMode;

            if (o.ExcludeCommon && o.ExcludeVcs && o.ExcludeVersion) {
                Helpers.ShowError("Error: Everything was excluded, nothing to do");
                Environment.Exit(1);
            }

            var fullFileName = Helpers.GetFullFileName(o.InputFile);

            var writer = (o.OutputFile.Equals(StdoutFileName, StringComparison.OrdinalIgnoreCase))
                ? (IWriter)new ConsoleWriter()
                : (IWriter)new FileWriter(o.OutputFile, o.PrependToFile);

            try {
                var generatorOptions = new ScriptGeneratorOptions {
                    Prefix = o.Prefix,
                    IgnoreEmpty = o.IncludeEmpty
                };

                // TODO: refactor it
                var installSystems = new Dictionary<string, Lazy<ScriptGenerator>> {
                    ["nsis"] = new Lazy<ScriptGenerator>(() => new NsisScriptGenerator(generatorOptions)),
                    ["inno"] = new Lazy<ScriptGenerator>(() => new InnoScriptGenerator(generatorOptions))
                };

                if (!installSystems.ContainsKey(o.InstallSystem)) {
                    Helpers.ShowError("Invalid installation system, exiting");
                    Environment.Exit(1);
                }

                var generator = installSystems[o.InstallSystem].Value;

                // common file information
                if (!o.ExcludeCommon) {
                    generator.AddParser(new CommonInfoParser(fullFileName, o.DateFormat));
                }

                // version file information
                if (!o.ExcludeVersion) {
                    generator.AddParser(new PeParser(fullFileName, o.VersionFormat));
                    generator.AddParser(new JarParser(fullFileName));
                }

                var repoPath = String.IsNullOrEmpty(o.RepoPath)
                    ? Path.GetDirectoryName(fullFileName)
                    : o.RepoPath;

                ConsoleLogger.LogInfo($"Repository path: {repoPath}");

                if ((!o.ExcludeVcs) && Directory.Exists(repoPath)) {
                    generator.AddParser(new GitParser(repoPath, o.DateFormat));
                    generator.AddParser(new MercurialParser(repoPath, o.DateFormat));
                    generator.AddParser(new SubversionParser(repoPath, o.DateFormat));
                    generator.AddParser(new BazaarParser(repoPath, o.DateFormat));
                }
                writer.Write(generator);

            } catch (FileNotFoundException) {
                Helpers.ShowError($"File '{o.InputFile}' not found, exiting");
            } catch (IOException) {
                Helpers.ShowError($"IOExeption: '{o.InputFile}', exiting");
            } catch (Exception ex) {
                Helpers.ShowError($"Error while generating process: {ex.Message}, exiting");
            }
        }

        static void Main(string[] args) {
            Parser.Default.ParseArguments<CliOptions>(args).WithParsed(WriteResults);
        }
    }
}
