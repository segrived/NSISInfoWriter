using System;
using System.ComponentModel;
using System.Diagnostics;

namespace NSISInfoWriter
{
    class Helpers
    {
        public string FileName { get; private set; }

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

    }
}
