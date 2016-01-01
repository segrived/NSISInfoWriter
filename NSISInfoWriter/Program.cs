using System;
using System.IO;
using CommandLine;

namespace NSISInfoWriter
{
    class Program
    {
        static void WriteResults(CLIOptions o) {
            var generator = new NsisScriptGenerator(o.OutputFile, o.Prefix, o.IgnoreEmpty);

            if (o.ExcludeCommon && o.ExcludeVCS && o.ExcludeVersion) {
                Console.WriteLine("ERROR: Everything was excluded, nothing to do");
                Environment.Exit(1);
            }


            try {
                var infoParser = new FileInfoParser(o.InputFile);
                var versionFormatter = new VersionFormatGenerator(infoParser.VersionInfo);
                // common file information
                if (! o.ExcludeCommon) {
                    generator.Add("FILE_NAME"     , infoParser.FileName);
                    generator.Add("FILE_SIZE"     , infoParser.FileInfo.Length.ToString());
                    generator.Add("FILE_SIZE_KB"  , (infoParser.FileInfo.Length / 1024).ToString());
                    generator.Add("FILE_SIZE_MB"  , (infoParser.FileInfo.Length / 1048576).ToString());
                }

                // version file information
                if (!o.ExcludeVersion) {
                    generator.Add("VI_PRODUCTIONVERSION"      , infoParser.VersionInfo.ProductVersion);
                    generator.Add("VI_FILEVERSION"            , infoParser.VersionInfo.FileVersion);
                    generator.Add("VI_FMT_PRODUCTIONVERSION"  , versionFormatter.FormatVersion(VersionType.PRODUCT, o.Format));
                    generator.Add("VI_FMT_FILEVERSION"        , versionFormatter.FormatVersion(VersionType.FILE, o.Format));
                    generator.Add("VI_COPYRIGHTS"             , infoParser.VersionInfo.LegalCopyright);
                    generator.Add("VI_DESCRIPTION"            , infoParser.VersionInfo.FileDescription);
                    generator.Add("VI_COMPANY"                , infoParser.VersionInfo.CompanyName);
                }

                if(! o.ExcludeVCS) {
                    var inputDir = Path.GetDirectoryName(o.InputFile);
                    
                    // git related information
                    var git = new VCSInformationParser.GitParser(inputDir);
                    if (git.IsAvailableVCSExecutable() && git.IsUnderControl()) {
                        generator.AddRange(git.GetInformation());
                    }

                    // mercurial related information
                    var hg = new VCSInformationParser.MercurialParser(inputDir);
                    if (hg.IsAvailableVCSExecutable() && hg.IsUnderControl()) {
                        generator.AddRange(hg.GetInformation());
                    }

                    // mercurial related information
                    var svn = new VCSInformationParser.SubversionParser(inputDir);
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
