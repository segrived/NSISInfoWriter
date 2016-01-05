using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace NSISInfoWriter.OutputGenerator
{
    public class FileWriter : IScriptWriter
    {
        private const string StartComment = @"; NSISINFOWRITER START";
        private const string EndComment   = @"; NSISINFOWRITER END";

        private readonly string fileName;
        private readonly bool prepend;

        public FileWriter(string fileName, bool prepend) {
            this.fileName = fileName;
            this.prepend = prepend;
        }

        public void Write(string content) {
            var f = prepend 
                ? (Action<string>)this.WritePrepend 
                : (Action<string>)this.WriteOverride;
            f(content);
        }

        private void WritePrepend(string content) {
            string currentFileContent;
            // if file isn't exists just init file content with empty string
            try {
                currentFileContent = File.ReadAllText(this.fileName);
            } catch (Exception) {
                currentFileContent = String.Empty;
            }
            var r = new Regex($"{StartComment}.*{EndComment}", RegexOptions.Singleline);
            // remove previously generated content
            var clearedContent = r.Replace(currentFileContent, String.Empty).Trim();
            // generate new file content
            var builder = new StringBuilder()
                .Append(StartComment)
                .Append(Environment.NewLine)
                .Append(content)
                .Append(Environment.NewLine)
                .Append(EndComment)
                .Append(Environment.NewLine)
                .Append(clearedContent);
            // write new content to file
            File.WriteAllText(this.fileName, builder.ToString());
        }

        private void WriteOverride(string content) {
            using (var writer = File.CreateText(this.fileName)) {
                writer.Write(content);
            }
        }
    }
}
