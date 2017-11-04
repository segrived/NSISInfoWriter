using System;
using NSISInfoWriter.Generators;

namespace NSISInfoWriter.Writers
{
    public class ConsoleWriter : IWriter
    {
        public void Write(ScriptGenerator generator) {
            Console.Out.Write(generator.GetOutput());
        }
    }

}
