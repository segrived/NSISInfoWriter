using NSISInfoWriter.OutputGenerators;

namespace NSISInfoWriter.OutputWriters
{
    public interface IWriter
    {
        void Write(ScriptGenerator generator);
    }
}
