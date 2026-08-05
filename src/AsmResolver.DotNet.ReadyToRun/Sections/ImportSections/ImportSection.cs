using AsmResolver.DotNet.ReadyToRun.Enumerations;

namespace AsmResolver.DotNet.ReadyToRun.Sections.ImportSections
{
    public class ImportSection
    {
        public bool LazyInitialize;
        public ReadyToRunImportSectionFlags Flags;
        public ReadyToRunImportSectionType Type;

        public static ImportSection ReadImportSection(ReadyToRunDirectory directory, SectionReader reader, uint size)
        {
            var section = reader.
        }
    }
}