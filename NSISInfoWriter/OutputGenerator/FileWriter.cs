using System.IO;

namespace NSISInfoWriter.OutputGenerator
{
    public class FileWriter : IScriptWriter
    {
        private readonly string fileName;

        public FileWriter(string fileName) {
            this.fileName = fileName;
        }

        public void Write(string content) {
            using (var writer = File.CreateText(this.fileName)) {
                writer.Write(content);
            }
        }
    }
}
