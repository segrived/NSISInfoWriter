using System;
using System.IO;

namespace NSISInfoWriter
{
    public class SignatureReader
    {
        private string FileName { get; }

        public SignatureReader(string fileName) {
            this.FileName = fileName;
        }

        private T ReadInternal<T>(Func<BinaryReader, T> f) {
            using (var stream = new FileStream(this.FileName, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(stream)) {
                return f(reader);
            }
        }

        public Int16 Read16() => this.ReadInternal(r => r.ReadInt16());
        public Int32 Read32() => this.ReadInternal(r => r.ReadInt32());
        public Int64 Read64() => this.ReadInternal(r => r.ReadInt64());
    }
}
