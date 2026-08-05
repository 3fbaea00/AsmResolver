using AsmResolver.DotNet.ReadyToRun.Enumerations;

namespace AsmResolver.DotNet.ReadyToRun
{
    public interface IReadyToRunAbstractSection
    {
        ulong GetSectionSize();

        void ReadSection(ReadyToRunDirectory directory, SectionReader reader, uint sectionSize);

        void WriteSection(ReadyToRunDirectory directory, SectionWriter writer, uint rva);

        virtual void PostInitialization(ReadyToRunDirectory directory) { }
    }

    public interface IReadyToRunSection : IReadyToRunAbstractSection
    {
        public static abstract ReadyToRunSectionType SectionType
        {
            get;
        }
    }

    public interface IReadyToRunImageSection : IReadyToRunSection;

    public interface IReadyToRunAssemblySection : IReadyToRunSection
    {
        AssemblyDescriptor TargetAssembly
        {
            get; 
            init;
        }
    }
}
