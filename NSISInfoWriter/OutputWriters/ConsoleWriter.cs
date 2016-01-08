using NSISInfoWriter.OutputGenerators;
using System;

namespace NSISInfoWriter.OutputWriters
{
    public class ConsoleWriter : IWriter
    {
        public void Write(ScriptGenerator generator) {
            Console.Out.Write(generator.GetOutput());
        }
    }

}
