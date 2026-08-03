using AsmResolver.PE.DotNet.ReadyToRun.Enumerations;

namespace AsmResolver.PE.DotNet.ReadyToRun
{
    public unsafe interface IReadyToRunSection
    {
        public ReadyToRunSectionType SectionType
        {
            get;
        }

        public abstract void ReadContent(SectionReader reader);

        public abstract void WriteContent(SectionWriter writer);
    }
}
