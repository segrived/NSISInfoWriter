using System;
using System.Collections.Generic;
using System.IO;

namespace NSISInfoWriter
{
    public static class Helpers
    {
        public enum FileArchitecture : ushort
        {
            Pe32 = 0x10b,
            Pe32P = 0x20b
        }

        public static FileArchitecture GetImageArchitecture(string filePath) {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(stream)) {
                if (reader.ReadUInt16() != 23117)
                    throw new BadImageFormatException("Not a valid Portable Executable image", filePath);
                stream.Seek(0x3A, SeekOrigin.Current);
                stream.Seek(reader.ReadUInt32(), SeekOrigin.Begin);
                if (reader.ReadUInt32() != 17744)
                    throw new BadImageFormatException("Not a valid Portable Executable image", filePath);
                stream.Seek(20, SeekOrigin.Current);
                var archShort = reader.ReadUInt16();
                if (!Enum.IsDefined(typeof(FileArchitecture), archShort)) {
                    throw new BadImageFormatException("Unknown image architecture", filePath);
                }
                return (FileArchitecture)archShort;
            }
        }

        public static string ImageArchitectureToString(FileArchitecture arch) {
            return arch == FileArchitecture.Pe32 ? "x86" : "x64";
        }

        public static string GetFullFileName(string fileName) {
            return new FileInfo(fileName).FullName;
        }

        public static void ShowColor(string message, ConsoleColor c) {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = c;
            Console.WriteLine(message);
            Console.ForegroundColor = oldColor;
        }

        public static void ShowError(string message) {
            Helpers.ShowColor(message, ConsoleColor.Red);
        }


        public static List<string> SplitByLines(this string input) {
            var lines = new List<string>();
            string line;
            using (var reader = new StringReader(input)) {
                while ((line = reader.ReadLine()) != null) {
                    lines.Add(line);
                }
            }
            return lines;
        }
    }
}
