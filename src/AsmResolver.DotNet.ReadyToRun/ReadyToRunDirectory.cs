using AsmResolver.DotNet.ReadyToRun.Enumerations;
using AsmResolver.DotNet.ReadyToRun.Sections;
using AsmResolver.IO;
using AsmResolver.PE.DotNet;
using AsmResolver.PE.File;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace AsmResolver.DotNet.ReadyToRun
{
    /// <summary>
    /// Represents a managed native header of a .NET portable executable file that is in the ReadyToRun format.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public unsafe class ReadyToRunDirectory : IManagedNativeHeader
    {
        [FieldOffset(0x00)] private CompilerIdentifierSection compilerIdentifierSection;
        [FieldOffset(0x08)] private ulong importSectionsSection;
        [FieldOffset(0x10)] private ulong runtimeFunctionsSection;
        [FieldOffset(0x18)] private ulong methodDefEntryPointsSection;
        [FieldOffset(0x20)] private ulong exceptionInfoSection;
        [FieldOffset(0x28)] private ulong debugInfoSection;
        [FieldOffset(0x30)] private ulong delayLoadMethodCallThunksSection;
        [FieldOffset(0x38)] private ulong oldAvailableTypesSection;
        [FieldOffset(0x40)] private ulong availableTypesSection;
        [FieldOffset(0x48)] private ulong instanceMethodEntryPointsSection;
        [FieldOffset(0x50)] private ulong inliningInfoSection;
        [FieldOffset(0x58)] private ulong profileDataInfoSection;
        [FieldOffset(0x60)] private ulong manifestMetadataSection;
        [FieldOffset(0x68)] private ulong attributePresenceSection;
        [FieldOffset(0x70)] private ulong inliningInfo2Section;
        [FieldOffset(0x78)] private ulong componentAssembliesSection;
        [FieldOffset(0x80)] private ulong ownerCompositeExecutableSection;
        [FieldOffset(0x88)] private ulong pgoInstrumentationDataSection;
        [FieldOffset(0x90)] private ulong manifestAssemblyMvidsSection;
        [FieldOffset(0x98)] private ulong crossModuleInlineInfoSection;
        [FieldOffset(0xA0)] private ulong hotColdMapSection;
        [FieldOffset(0xA8)] private ulong methodIsGenericMapSection;
        [FieldOffset(0xB0)] private ulong enclosingTypeMapSection;
        [FieldOffset(0xB8)] private ulong typeGenericInfoMapSection;
        [FieldOffset(0xC0)] private ulong externalTypeMapsSection;
        [FieldOffset(0xC8)] private ulong proxyTypeMapsSection;
        [FieldOffset(0xD0)] private ulong typeMapAssemblyTargetsSection;
        [FieldOffset(0xD8)] private BitVector32 stateOfSections;
        [FieldOffset(0xE0)] private ISegment contents;
        [FieldOffset(0xE8)] private uint majorVersion;
        [FieldOffset(0xF0)] private uint minorVersion;
        [FieldOffset(0xF8)] private ReadyToRunAttributes attributes;

        public CompilerIdentifierSection CompilerIdentifierSection => compilerIdentifierSection;
        public ulong ImportSectionsSection => importSectionsSection;
        public ulong RuntimeFunctionsSection => runtimeFunctionsSection;
        public ulong MethodDefEntryPointsSection => methodDefEntryPointsSection;
        public ulong ExceptionInfoSection => exceptionInfoSection;
        public ulong DebugInfoSection => debugInfoSection;
        public ulong DelayLoadMethodCallThunksSection => delayLoadMethodCallThunksSection;
        public ulong AvailableTypesSection => availableTypesSection;
        public ulong InstanceMethodEntryPointsSection => instanceMethodEntryPointsSection;
        public ulong ProfileDataInfoSection => profileDataInfoSection;
        public ulong ManifestMetadataSection => manifestMetadataSection;
        public ulong AttributePresenceSection => attributePresenceSection;
        public ulong InliningInfo2Section => inliningInfo2Section;
        public ulong ComponentAssembliesSection => componentAssembliesSection;
        public ulong OwnerCompositeExecutableSection => ownerCompositeExecutableSection;
        public ulong PgoInstrumentationDataSection => pgoInstrumentationDataSection;
        public ulong ManifestAssemblyMvidsSection => manifestAssemblyMvidsSection;
        public ulong CrossModuleInlineInfoSection => crossModuleInlineInfoSection;
        public ulong HotColdMapSection => hotColdMapSection;
        public ulong MethodIsGenericMapSection => methodIsGenericMapSection;
        public ulong EnclosingTypeMapSection => enclosingTypeMapSection;
        public ulong TypeGenericInfoMapSection => typeGenericInfoMapSection;
        public ulong ExternalTypeMapsSection => externalTypeMapsSection;
        public ulong ProxyTypeMapsSection => proxyTypeMapsSection;
        public ulong TypeMapAssemblyTargetsSection => typeMapAssemblyTargetsSection;

        /// <inheritdoc />
        public ManagedNativeHeaderSignature Signature => ManagedNativeHeaderSignature.RTR;

        public ISegment Contents => contents;

        public uint SectionCount => Popcnt.PopCount((uint)stateOfSections.Data);

        public void SetSection<TSection>(TSection section)
            where TSection : IReadyToRunSection
        {
            var index = (int)(TSection.SectionType - 100);
            Unsafe.As<CompilerIdentifierSection, TSection>(ref Unsafe.Add(ref compilerIdentifierSection, index)) = section;

            stateOfSections[index] = section is not null;
        }

        private const uint HeaderSize =
            /* Signature    */ sizeof(ManagedNativeHeaderSignature) +
            /* MajorVersion */ sizeof(ushort) +
            /* MinorVersion */ sizeof(ushort) +
            /* Flags        */ sizeof(ReadyToRunAttributes) +
            /* SectionCount */ sizeof(uint);

        private const uint SectionCountOffset =
            /* Signature    */ sizeof(ManagedNativeHeaderSignature) +
            /* MajorVersion */ sizeof(ushort) +
            /* MinorVersion */ sizeof(ushort) +
            /* Flags        */ sizeof(ReadyToRunAttributes);

        private const uint UsefulHeaderSize =
            /* MajorVersion */ sizeof(ushort) +
            /* MinorVersion */ sizeof(ushort) +
            /* Flags        */ sizeof(ReadyToRunAttributes);

        private const uint UsefulHeaderOffset =
            /* Signature    */ sizeof(ManagedNativeHeaderSignature);

        private const uint SectionHeaderSize = sizeof(ReadyToRunSectionType) + DataDirectory.DataDirectorySize;

        private const uint SectionMaxCount = ReadyToRunSectionType.TypeMapAssemblyTargets - ReadyToRunSectionType.CompilerIdentifier;

        public void WriteContents()
        {
            contents = new SegmentBuilder();
            var sectionsHeader = new SegmentBuilder();
            var sectionsContent = new SegmentBuilder();
            var stateOfSections = (ulong)this.stateOfSections.Data;
            var sectionCount = Popcnt.PopCount((uint)stateOfSections);
            var sectionIndex = 0;
            while (stateOfSections != 0)
            {
                var relativeSectionIndex = BitOperations.TrailingZeroCount(stateOfSections);
                relativeSectionIndex++;
                sectionIndex += relativeSectionIndex;
                stateOfSections >>= relativeSectionIndex;
                var section = Unsafe.As<CompilerIdentifierSection, IReadyToRunAbstractSection>(ref Unsafe.Add(ref compilerIdentifierSection, sectionIndex));
                var sectionSize = section.CalculateContentSize();
                var sectionContent = new SegmentBuilder();
                var sectionContentReference = sectionContent.ToReference();
                section.WriteContent(sectionContent);

                var sectionHeader = new SegmentBuilder();
                var rvaPointer = new RelativeReference(sectionContentReference, 0);
                var a = ;
                sectionHeader.Add(rvaPointer);

            }
        }

        public static ReadyToRunDirectory FromCustomManagedNativeHeader(CustomManagedNativeHeader header)
        {
            var file = header.File;
            var contentsSegment = header.Contents;
            var contentsBytes = contentsSegment.WriteIntoArray();
            var length = (ulong)contentsBytes.Length;

            fixed (byte* contents = contentsBytes)
            {
                var directory = new ReadyToRunDirectory();

                if (length < HeaderSize)
                    throw new InvalidOperationException();

                Unsafe.CopyBlock(
                    Unsafe.AsPointer(ref directory.majorVersion),
                    contents + UsefulHeaderOffset,
                    UsefulHeaderSize
                );

                var numOfSections = *(uint*)(contents + SectionCountOffset);
                if (length < HeaderSize + numOfSections * SectionHeaderSize)
                    throw new InvalidOperationException();

                var offset = HeaderSize;
                var numOfLeftSections = numOfSections;
                var stateOfSections = new BitVector32();
                while (numOfLeftSections != 0)
                {
                    numOfLeftSections--;

                    var sectionType = *(ReadyToRunSectionType*)(contents + offset);
                    var rva = *(uint*)(contents + offset + sizeof(ReadyToRunSectionType));
                    var size = *(uint*)(contents + offset + sizeof(ReadyToRunSectionType) + sizeof(uint));
                    offset += sizeof(ReadyToRunSectionType) + sizeof(uint) + sizeof(uint);

                    var sectionStreamReader = file.CreateReaderAtRva(rva);
                    if (sectionStreamReader.Length < size)
                        throw new InvalidOperationException();

                    var sectionBytes = sectionStreamReader.ReadBytes((int)size);
                    fixed (byte* sectionData = sectionBytes)
                    {
                        var section = sectionType switch
                        {
                            ReadyToRunSectionType.CompilerIdentifier => new CompilerIdentifierSection(),
                            _ => throw new NotImplementedException()
                        };

                        var sectionReader = new SectionReader(sectionData);
                        section.ReadContent(sectionReader, size);

                        var index = (int)(sectionType - 100);
                        Unsafe.As<CompilerIdentifierSection, IReadyToRunAbstractSection>(ref Unsafe.Add(ref directory.compilerIdentifierSection, index)) = section;
                        stateOfSections[index] = true;
                    }
                }

                directory.stateOfSections = stateOfSections;

                return directory;
            }
        }
    }
}
