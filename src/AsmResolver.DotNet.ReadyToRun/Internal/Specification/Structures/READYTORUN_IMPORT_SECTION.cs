using AsmResolver.DotNet.ReadyToRun.Enumerations;

namespace AsmResolver.DotNet.ReadyToRun.Internal.Structures
{
    internal struct READYTORUN_IMPORT_SECTION
    {
        public IMAGE_DATA_DIRECTORY Section;       // Section containing values to be fixed up
        public ReadyToRunImportSectionFlags Flags; // One or more of ReadyToRunImportSectionFlags
        public ReadyToRunImportSectionType Type;   // One of ReadyToRunImportSectionType
        public byte EntrySize;
        public uint Signatures;                    // RVA of optional signature descriptors
        public uint AuxiliaryData;                 // RVA of optional auxiliary data (typically GC info)
    }
}