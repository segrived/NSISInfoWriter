using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using NSISInfoWriter.Generators;

namespace NSISInfoWriter.Writers
{
    public class FileWriter : IWriter
    {
        private const string CommentText = @"NSISINFOWRITER PRIVATE AREA";

        private readonly string _fileName;
        private readonly bool _prepend;

        public FileWriter(string fileName, bool prepend) {
            this._fileName = fileName;
            this._prepend = prepend;
        }

        public void Write(ScriptGenerator generator) {
            var f = this._prepend
                ? (Action<ScriptGenerator>)this.WritePrepend
                : (Action<ScriptGenerator>)this.WriteOverride;
            f(generator);
        }

        private void WritePrepend(ScriptGenerator generator) {
            var fullCommentLine = $"{generator.CommentChar} {CommentText}";

            string currentFileContent;
            // if file isn't exists just init file content with empty string
            try {
                currentFileContent = File.ReadAllText(this._fileName);
            } catch (Exception) {
                currentFileContent = String.Empty;
            }
            var r = new Regex($"{fullCommentLine}.*{fullCommentLine}", RegexOptions.Singleline);
            // remove previously generated content
            var clearedContent = r.Replace(currentFileContent, String.Empty).TrimStart();
            // generate new file content
            var builder = new StringBuilder()
                .Append(fullCommentLine)
                .Append(Environment.NewLine)
                .Append(generator.GetOutput().Trim())
                .Append(Environment.NewLine)
                .Append(fullCommentLine)
                .Append(Environment.NewLine)
                .Append(clearedContent);
            // write new content to file
            File.WriteAllText(this._fileName, builder.ToString());
        }

        private void WriteOverride(ScriptGenerator generator) {
            using (var writer = File.CreateText(this._fileName)) {
                writer.Write(generator.GetOutput());
            }
        }
    }
}
