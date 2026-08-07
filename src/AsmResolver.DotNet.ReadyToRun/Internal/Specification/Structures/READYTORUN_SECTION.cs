using AsmResolver.DotNet.ReadyToRun.Enumerations;

namespace AsmResolver.DotNet.ReadyToRun.Internal.Structures
{
    internal struct READYTORUN_SECTION
    {
        public ReadyToRunSectionType Type; // READYTORUN_SECTION_XXX
        public IMAGE_DATA_DIRECTORY Section;
    }
}
