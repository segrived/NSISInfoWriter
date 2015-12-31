using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSISInfoWriter
{
    class Helpers
    {
        public string FileName { get; private set; }

        public static bool IsGitAvialable() {
            var psi = new ProcessStartInfo {
                RedirectStandardOutput = true,
                FileName = "git",
                Arguments = "--version",
                CreateNoWindow = true,
                UseShellExecute = false
            };
            try {
                var p = Process.Start(psi);
                p.WaitForExit();
                return p.ExitCode == 0;
            } catch (Win32Exception) {
                return false;
            }
        }

        public static string GetCommandOutput(string fileName, string arguments, string wd) {
            var psi = new ProcessStartInfo {
                FileName = fileName,
                Arguments = arguments,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                WorkingDirectory = wd
            };
            var p = Process.Start(psi);
            var output = p.StandardOutput.ReadToEnd();
            return output.Trim();
        }

        public static bool IsUnderGit(string wd) {
            // if inside .git directory - false, if directory isn't under git control -
            // GetCommandOutput will return nothing, because stderr is ignored
            var res = Helpers.GetCommandOutput("git", "rev-parse --is-inside-work-tree", wd);
            return res == "true";
        }

    }
}
