using AsmResolver.DotNet.ReadyToRun.Enumerations;
using AsmResolver.DotNet.ReadyToRun.Reader;

namespace AsmResolver.DotNet.ReadyToRun
{
    public interface IReadyToRunAbstractSection 
    {
        ulong GetSectionSize();

        void ReadSection(ReadyToRunDirectoryReader directoryReader, ref byte source, uint sourceSize);

        void WriteSection(ReadyToRunDirectory directory, ref byte destination, uint rva);

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
