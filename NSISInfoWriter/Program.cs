using System;
using System.IO;
using CommandLine;

namespace NSISInfoWriter
{
    class Program
    {
        static void WriteResults(CLIOptions o) {
            var generator = new NsisScriptGenerator(o.OutputFile, o.Prefix, o.IgnoreEmpty);

            if (o.ExcludeCommon && o.ExcludeGit && o.ExcludeVersion) {
                Console.WriteLine("ERROR: Everything was excluded, nothing to do");
                Environment.Exit(1);
            }

            try {
                var infoParser = new FileInfoParser(o.InputFile);

                // common file information
                if (! o.ExcludeCommon) {
                    generator.Add("FILE_NAME"                 , infoParser.FileName);
                    generator.Add("FILE_SIZE"                 , infoParser.FileInfo.Length.ToString());
                    generator.Add("FILE_SIZE_KB"              , (infoParser.FileInfo.Length / 1024).ToString());
                    generator.Add("FILE_SIZE_MB",               (infoParser.FileInfo.Length / 1048576).ToString());
                }

                // version file information
                if (!o.ExcludeVersion) {
                    generator.Add("VI_PRODUCTIONVERSION"      , infoParser.VersionInfo.ProductVersion);
                    generator.Add("VI_FILEVERSION"            , infoParser.VersionInfo.FileVersion);
                    generator.Add("VI_COPYRIGHTS"             , infoParser.VersionInfo.LegalCopyright);
                    generator.Add("VI_DESCRIPTION"            , infoParser.VersionInfo.FileDescription);
                    generator.Add("VI_COMPANY"                , infoParser.VersionInfo.CompanyName);
                }

                // git related information
                if (!o.ExcludeGit && Helpers.IsGitAvialable() && Helpers.IsUnderGit(infoParser.WorkingDirectory)) {
                    generator.Add("GIT_LAST_COMMIT_HASH_LONG" , infoParser.GetLastCommitHash(false));
                    generator.Add("GIT_LAST_COMMIT_HASH_SHORT", infoParser.GetLastCommitHash(true));
                    generator.Add("GIT_USERNAME"              , infoParser.GetGitUserName());
                    generator.Add("GIT_USEREMAIL"             , infoParser.GetGitUserEmail());
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
