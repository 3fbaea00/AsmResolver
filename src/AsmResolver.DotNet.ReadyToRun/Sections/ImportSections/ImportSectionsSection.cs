using AsmResolver.DotNet.ReadyToRun.Enumerations;

namespace AsmResolver.DotNet.ReadyToRun.Sections
{
    public class ImportSectionsSection : IReadyToRunImageSection
    {
        public static ReadyToRunSectionType SectionType => ReadyToRunSectionType.ImportSections;

        public ulong GetSectionSize()
        {
            throw new System.NotImplementedException();
        }

        public void ReadSection(ReadyToRunDirectory directory, SectionReader reader, uint sectionSize)
        {
            throw new System.NotImplementedException();
        }

        public void WriteSection(ReadyToRunDirectory directory, SectionWriter writer, uint rva)
        {
            throw new System.NotImplementedException();
        }
    }
}
