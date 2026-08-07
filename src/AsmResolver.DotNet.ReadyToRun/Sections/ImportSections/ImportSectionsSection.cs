using AsmResolver.DotNet.ReadyToRun.Enumerations;
using AsmResolver.DotNet.ReadyToRun.Internal.Structures;
using AsmResolver.DotNet.ReadyToRun.Reader;
using AsmResolver.DotNet.ReadyToRun.Sections.ImportSections;
using System.Runtime.CompilerServices;

namespace AsmResolver.DotNet.ReadyToRun.Sections
{
    public unsafe class ImportSectionsSection : IReadyToRunImageSection
    {
        public static ReadyToRunSectionType SectionType => ReadyToRunSectionType.ImportSections;

        public ulong GetSectionSize()
        {
            throw new System.NotImplementedException();
        }

        public void ReadSection(ReadyToRunDirectoryReader directoryReader, ref byte source, uint sourceSize)
        {
            var numOfSection = sourceSize / (uint)sizeof(READYTORUN_IMPORT_SECTION);
            var sections = new ImportSection[numOfSection];
            while (numOfSection-- != 0)
            {
                ref var reference = ref Unsafe.Add(ref source, numOfSection * (uint)sizeof(READYTORUN_IMPORT_SECTION));
                sections[numOfSection] = ImportSection.ReadImportSection(directoryReader, ref reference);
            }

        }

        public void WriteSection(ReadyToRunDirectory directory, SectionWriter writer, uint rva)
        {
            throw new System.NotImplementedException();
        }

        public void WriteSection(ReadyToRunDirectory directory, ref byte destination, uint rva)
        {
            throw new System.NotImplementedException();
        }
    }
}
