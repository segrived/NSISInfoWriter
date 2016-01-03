using System;
using System.IO;
using CommandLine;

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

            if(! Path.HasExtension(o.OutputFile)) {
                o.OutputFile = String.Format("{0}.{1}", o.OutputFile, DefaultOutputFileExt);
            }

            var generator = new NsisScriptGenerator(o.OutputFile, o.Prefix, o.IgnoreEmpty);

            try {
                var infoParser = new FileInfoParser(o.InputFile);
                var versionFormatter = new VersionFormatGenerator(infoParser.VersionInfo);

                generator.Add("SCRIPT_GENERATE_TIME", DateTime.Now.ToString(o.DateFormat));

                // common file information
                if (! o.ExcludeCommon) {
                    generator.Add("FILE_NAME"           , infoParser.FileName);
                    generator.Add("FILE_SIZE"           , infoParser.FileInfo.Length.ToString());
                    generator.Add("FILE_SIZE_KB"        , (infoParser.FileInfo.Length / 1024).ToString());
                    generator.Add("FILE_SIZE_MB"        , (infoParser.FileInfo.Length / 1048576).ToString());
                    generator.Add("FILE_CREATION_DATE"  , infoParser.FileInfo.CreationTime.ToString(o.DateFormat));
                    generator.Add("FILE_LAST_WRITE_TIME", infoParser.FileInfo.LastWriteTime.ToString(o.DateFormat));
                    try {
                        var arch = Helpers.GetImageArchitecture(infoParser.FileName);
                        generator.Add("FILE_ARCH", Helpers.ImageArchitectureToString(arch));
                    } catch (BadImageFormatException) { }
                }

                // version file information
                if (!o.ExcludeVersion) {
                    generator.Add("VI_PRODUCTIONVERSION"      , infoParser.VersionInfo.ProductVersion);
                    generator.Add("VI_FILEVERSION"            , infoParser.VersionInfo.FileVersion);
                    generator.Add("VI_FMT_PRODUCTIONVERSION"  , versionFormatter.FormatVersion(VersionType.PRODUCT, o.VersionFormat));
                    generator.Add("VI_FMT_FILEVERSION"        , versionFormatter.FormatVersion(VersionType.FILE, o.VersionFormat));
                    generator.Add("VI_COPYRIGHTS"             , infoParser.VersionInfo.LegalCopyright);
                    generator.Add("VI_DESCRIPTION"            , infoParser.VersionInfo.FileDescription);
                    generator.Add("VI_COMPANY"                , infoParser.VersionInfo.CompanyName);
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
