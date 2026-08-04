using AsmResolver.DotNet.ReadyToRun.Enumerations;

namespace AsmResolver.DotNet.ReadyToRun
{
    public interface IReadyToRunAbstractSection
    {
        void ReadContent(SectionReader reader, uint contentSize);

        ulong CalculateContentSize();

        void WriteContent(SectionWriter writer, uint rva);
    }

    public interface IReadyToRunSection : IReadyToRunAbstractSection
    {
        public static abstract ReadyToRunSectionType SectionType
        {
            get;
        }
    }
}
