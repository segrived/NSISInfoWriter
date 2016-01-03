using System;

namespace NSISInfoWriter
{
    class Helpers
    {
        public enum FileArchitecture : ushort
        {
            Pe32 = 0x10b,
            Pe32P = 0x20b
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
