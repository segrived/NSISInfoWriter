using System;

namespace NSISInfoWriter.OutputGenerator
{
    public class ConsoleWriter : IScriptWriter
    {
        public void Write(string content) {
            Console.Out.Write(content);
        }
    }

}
