using AsmResolver.IO;
using AsmResolver.PE.DotNet.ReadyToRun.Enumerations;

namespace AsmResolver.PE.DotNet.ReadyToRun
{
    /// <summary>
    /// Provides methods for parsing individual sections in a ReadyToRun directory of a .NET module.
    /// </summary>
    public interface IReadyToRunSectionReader
    {
        /// <summary>
        /// Parses a single ReadyToRun section.
        /// </summary>
        /// <param name="sectionType">The type of section to read.</param>
        /// <param name="context">The context in which the reader is situated in.</param>
        /// <param name="reader">The input stream containing the raw binary contents of the section.</param>
        /// <returns>The parsed section.</returns>
        IReadyToRunSection ReadSection(ReadyToRunSectionType sectionType, PEReaderContext context, ref BinaryStreamReader reader);
    }
}
