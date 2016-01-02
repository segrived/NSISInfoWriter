using System;
using System.ComponentModel;
using System.Diagnostics;

namespace NSISInfoWriter
{
    class Helpers
    {
        public enum FileArchitecture : ushort
        {
            Pe32 = 0x10b,
            Pe32P = 0x20b
        }

        public static ProcessStartInfo GetPSI(string command, string args, string wd = null) {
            if(wd == null) {
                wd = Environment.CurrentDirectory;
            }
            return new ProcessStartInfo {
                FileName               = command,
                Arguments              = args,
                CreateNoWindow         = true,
                RedirectStandardOutput = true,
                RedirectStandardError  = true,
                UseShellExecute        = false,
                WorkingDirectory       = wd
            };
        }


        public static int GetExitCode(string command, string args, string wd = null) {
            try {
                var p = Process.Start(GetPSI(command, args, wd));
                p.WaitForExit();
                return p.ExitCode;
            } catch (Win32Exception) {
                return -1; // error code
            }
        }

        public static string GetCommandOutput(string command, string args, string wd = null) {
            var p = Process.Start(GetPSI(command, args, wd));
            var output = p.StandardOutput.ReadToEnd();
            return output.Trim();
        }

        public static FileArchitecture GetImageArchitecture(string filePath) {
            using (var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            using (var reader = new System.IO.BinaryReader(stream)) {
                if (reader.ReadUInt16() != 23117)
                    throw new BadImageFormatException("Not a valid Portable Executable image", filePath);
                stream.Seek(0x3A, System.IO.SeekOrigin.Current);
                stream.Seek(reader.ReadUInt32(), System.IO.SeekOrigin.Begin);
                if (reader.ReadUInt32() != 17744)
                    throw new BadImageFormatException("Not a valid Portable Executable image", filePath);
                stream.Seek(20, System.IO.SeekOrigin.Current);
                var archShort = reader.ReadUInt16();
                if(! Enum.IsDefined(typeof(FileArchitecture), archShort)) {
                    throw new BadImageFormatException("Unknown image architecture", filePath);
                }
                return (FileArchitecture)archShort;
            }
        }

        public static string ImageArchitectureToString(FileArchitecture arch) {
            return arch == FileArchitecture.Pe32 ? "x86" : "x64";
        }

    }
}
