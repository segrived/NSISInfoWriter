using System;
using System.IO;

namespace NSISInfoWriter
{
    public class SignatureReader
    {
        private string FileName { get; set; }

        public SignatureReader(string fileName) {
            this.FileName = fileName;
        }

        private T ReadSignatureInternal<T>(Func<BinaryReader, T> f) {
            using (var stream = new FileStream(this.FileName, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(stream)) {
                return f(reader);
            }
        }

        public Int16 ReadSignature16() => this.ReadSignatureInternal(r => r.ReadInt16());
        public Int32 ReadSignature32() => this.ReadSignatureInternal(r => r.ReadInt32());
        public Int64 ReadSignature64() => this.ReadSignatureInternal(r => r.ReadInt64());
    }
}
