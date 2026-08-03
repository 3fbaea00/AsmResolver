using AsmResolver.IO;
using AsmResolver.PE.DotNet.ReadyToRun.Enumerations;
using AsmResolver.PE.DotNet.ReadyToRun.Sections;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AsmResolver.PE.DotNet.ReadyToRun
{
    /// <summary>
    /// Provides a default implementation for the <see cref="IReadyToRunSectionReader"/> interface.
    /// </summary>
    public unsafe class DefaultReadyToRunSectionReader : IReadyToRunSectionReader
    {
        /// <summary>
        /// Gets the singleton instance of the <see cref="DefaultReadyToRunSectionReader"/> class.
        /// </summary>
        public static DefaultReadyToRunSectionReader Instance { get; } = new();

        /// <inheritdoc />
        public IReadyToRunSection ReadSection(ReadyToRunSectionType sectionType, PEReaderContext context, ref BinaryStreamReader reader)
        {
            return sectionType switch
            {
                ReadyToRunSectionType.CompilerIdentifier => ReadSectionType<CompilerIdentifierSection>(ref reader),
                _ => throw new NotImplementedException()
            };

            static IReadyToRunSection ReadSectionType<TSection>(ref BinaryStreamReader streamReader)
                where TSection : IReadyToRunSection, new()
            {
                var rva = streamReader.Rva;
                var size = streamReader.Length;

                var section = new TSection()
                {
                    Rva = rva,
                    Size = size,
                };

                var dataSource = streamReader.DataSource;
                if (dataSource is ByteArrayDataSource byteDataSource)
                {
                    fixed (byte* bytes = byteDataSource.DELETEIT_DATA)
                    {
                        var sectionReader = new SectionReader(bytes);
                        section.ReadContent(sectionReader);
                    }
                }

                return section;
            }
        }
    }
}
