using System.Collections.Generic;

namespace NSISInfoWriter.Parsers
{
    public interface IParser
    {
        /// <summary>
        /// Indicate, can parser be used with selected file or not
        /// </summary>
        /// <returns></returns>
        bool IsParseble();
        /// <summary>
        /// Parse information to dictionary
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> Generate();
    }
}
