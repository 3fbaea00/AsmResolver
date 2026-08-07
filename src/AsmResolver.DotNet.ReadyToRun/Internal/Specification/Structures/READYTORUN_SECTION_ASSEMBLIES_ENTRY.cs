namespace AsmResolver.DotNet.ReadyToRun.Internal.Structures
{
    internal struct READYTORUN_SECTION_ASSEMBLIES_ENTRY
    {
        public IMAGE_DATA_DIRECTORY CorHeader;        // Input MSIL metadata COR header (for composite R2R images with embedded MSIL metadata)
        public IMAGE_DATA_DIRECTORY ReadyToRunHeader; // READYTORUN_CORE_HEADER of the assembly in question
    }
}
