using AsmResolver.DotNet.ReadyToRun.Enumerations;

namespace AsmResolver.DotNet.ReadyToRun.Internal.Structures
{
    internal struct READYTORUN_CORE_HEADER
    {
        public ReadyToRunAttributes Flags; // READYTORUN_FLAG_XXX
        public uint NumberOfSections;

        // Array of sections follows. The array entries are sorted by Type
        // READYTORUN_SECTION   Sections[];
    };
}
