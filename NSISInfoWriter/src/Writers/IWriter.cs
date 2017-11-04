using NSISInfoWriter.Generators;

namespace NSISInfoWriter.Writers
{
    public interface IWriter
    {
        void Write(ScriptGenerator generator);
    }
}
