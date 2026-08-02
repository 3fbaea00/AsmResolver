using AsmResolver.PE.DotNet.ReadyToRun.Enumerations;

namespace AsmResolver.PE.DotNet.ReadyToRun
{
    public struct ReadyToRunSection
    {
        /// <summary>
        /// The ReadyToRun section type
        /// </summary>
        public ReadyToRunSectionType Type { get; set; }

        /// <summary>
        /// The RVA to the section
        /// </summary>
        public int RelativeVirtualAddress { get; set; }

        /// <summary>
        /// The size of the section
        /// </summary>
        public int Size { get; set; }

        public ReadyToRunSection(ReadyToRunSectionType type, int rva, int size)
        {
            Type = type;
            RelativeVirtualAddress = rva;
            Size = size;
        }
    }
}
