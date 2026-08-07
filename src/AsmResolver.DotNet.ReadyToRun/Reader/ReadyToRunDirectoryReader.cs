using AsmResolver.DotNet.ReadyToRun.Enumerations;
using AsmResolver.DotNet.ReadyToRun.Internal.Extensions;
using AsmResolver.DotNet.ReadyToRun.Internal.Structures;
using AsmResolver.DotNet.ReadyToRun.Sections;
using AsmResolver.PE.DotNet;
using AsmResolver.PE.File;
using System;
using System.Runtime.CompilerServices;

namespace AsmResolver.DotNet.ReadyToRun.Reader
{
    public unsafe class ReadyToRunDirectoryReader
    {
        internal CustomManagedNativeHeader RawHeader;
        internal IReadyToRunSection[] Sections;

        public ReadyToRunDirectoryReader(CustomManagedNativeHeader rawHeader)
        {
            RawHeader = rawHeader;
        }

        public void ReadRTRDirectory(ReadyToRunDirectory directory)
        {
            ref var contents = ref RawHeader.Contents.GetData(out nuint length);
            var stateOfSections = 0u;

            Unsafe.As<ushort, ulong>(ref directory.internalBody.majorVersion) = Unsafe.As<ushort, ulong>(ref Unsafe.As<byte, READYTORUN_HEADER>(ref contents).MajorVersion);
            var numOfSections = Unsafe.As<byte, READYTORUN_HEADER>(ref contents).CoreHeader.NumberOfSections;

            Sections = new IReadyToRunSection[numOfSections];

            var offset = (uint)sizeof(READYTORUN_HEADER);
            PESection peSection = null;
            ref var peSectionData = ref Unsafe.NullRef<byte>();
            var numOfLeftSections = numOfSections;
            while (numOfLeftSections-- != 0)
            {
                ref var content = ref Unsafe.As<byte, READYTORUN_SECTION>(ref Unsafe.Add(ref contents, offset));
                offset += (uint)sizeof(READYTORUN_SECTION);

                if (peSection is null || !peSection.ContainsRva(content.Section.VirtualAddress))
                {
                    peSection = RawHeader.File.GetSectionContainingRva(content.Section.VirtualAddress);
                    peSectionData = ref peSection.Contents.GetData(out var sectionLength);
                }

                try { var _ = CreateSection(content.Type); } catch { continue; } // temp
                var section = CreateSection(content.Type);
                Sections[numOfLeftSections] = section;

                var fileOffset = peSection.RvaToFileOffset(content.Section.VirtualAddress) - peSection.Offset;
                ref var sectionData = ref Unsafe.Add(ref peSectionData, (nuint)fileOffset);
                var reader = new SectionReader(ref sectionData);
                section.ReadSection(this, reader, content.Section.Size);

                var index = (int)(content.Type - 100);
                Unsafe.As<CompilerIdentifierSection, IReadyToRunAbstractSection>(ref Unsafe.Add(ref directory.compilerIdentifierSection, index)) = section;
                stateOfSections |= 1u << index;
            }

            directory.internalBody.stateOfSectionTypes = stateOfSections;

            // other stages

            static IReadyToRunSection CreateSection(ReadyToRunSectionType type) => type switch
            {
                ReadyToRunSectionType.CompilerIdentifier => new CompilerIdentifierSection(),
                ReadyToRunSectionType.ImportSections => new ImportSectionsSection(),
                _ => throw new NotImplementedException()
            };
        }
    }
}
