using AsmResolver.DotNet.ReadyToRun.Enumerations;
using AsmResolver.DotNet.ReadyToRun.Internal;
using AsmResolver.DotNet.ReadyToRun.Internal.Extensions;
using AsmResolver.DotNet.ReadyToRun.Internal.Structures;
using AsmResolver.DotNet.ReadyToRun.Sections;
using AsmResolver.IO;
using AsmResolver.PE.DotNet;
using AsmResolver.PE.File;
using System;
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
    // no support for per assembly sectoisn yet!
    [StructLayout(LayoutKind.Explicit)]
    public unsafe class ReadyToRunDirectory : IManagedNativeHeader
    {
        [FieldOffset(0x00)] internal CompilerIdentifierSection compilerIdentifierSection;
        [FieldOffset(0x08)] internal ImportSectionsSection importSectionsSection;
        [FieldOffset(0x10)] internal ulong runtimeFunctionsSection;
        [FieldOffset(0x18)] internal ulong methodDefEntryPointsSection;
        [FieldOffset(0x20)] internal ulong exceptionInfoSection;
        [FieldOffset(0x28)] internal ulong debugInfoSection;
        [FieldOffset(0x30)] internal ulong delayLoadMethodCallThunksSection;
        [FieldOffset(0x38)] internal ulong oldAvailableTypesSection;
        [FieldOffset(0x40)] internal ulong availableTypesSection;
        [FieldOffset(0x48)] internal ulong instanceMethodEntryPointsSection;
        [FieldOffset(0x50)] internal ulong inliningInfoSection;
        [FieldOffset(0x58)] internal ulong profileDataInfoSection;
        [FieldOffset(0x60)] internal ulong manifestMetadataSection;
        [FieldOffset(0x68)] internal ulong attributePresenceSection;
        [FieldOffset(0x70)] internal ulong inliningInfo2Section;
        [FieldOffset(0x78)] internal ulong componentAssembliesSection;
        [FieldOffset(0x80)] internal ulong ownerCompositeExecutableSection;
        [FieldOffset(0x88)] internal ulong pgoInstrumentationDataSection;
        [FieldOffset(0x90)] internal ulong manifestAssemblyMvidsSection;
        [FieldOffset(0x98)] internal ulong crossModuleInlineInfoSection;
        [FieldOffset(0xA0)] internal ulong hotColdMapSection;
        [FieldOffset(0xA8)] internal ulong methodIsGenericMapSection;
        [FieldOffset(0xB0)] internal ulong enclosingTypeMapSection;
        [FieldOffset(0xB8)] internal ulong typeGenericInfoMapSection;
        [FieldOffset(0xC0)] internal ulong externalTypeMapsSection;
        [FieldOffset(0xC8)] internal ulong proxyTypeMapsSection;
        [FieldOffset(0xD0)] internal ulong typeMapAssemblyTargetsSection;

        [FieldOffset(0xD8)] internal InternalNoGCPointersBody internalBody;

        private ReadyToRunDirectory()
        {

        }

        public ReadyToRunDirectory(ReadyToRunAttributes attributes)
        {
            internalBody.majorVersion = 16;
            internalBody.attributes = attributes;
        }

        public ReadyToRunDirectory(ulong majorVersion, ulong minorVersion, ReadyToRunAttributes attributes)
        {
            Unsafe.As<ushort, uint>(ref internalBody.majorVersion) = (uint)majorVersion | (uint)minorVersion;
            internalBody.attributes = attributes;
        }

        /// <inheritdoc />
        public ManagedNativeHeaderSignature Signature => ManagedNativeHeaderSignature.RTR;

        public bool CanUpdateOffsets => true;

        public ulong Offset => internalBody.offset;

        public uint Rva => internalBody.rva;

        public CompilerIdentifierSection CompilerIdentifierSection => compilerIdentifierSection;
        public ImportSectionsSection ImportSectionsSection => importSectionsSection;
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

        public ushort MajorVersion
        {
            get => internalBody.majorVersion;
            set => internalBody.majorVersion = value;
        }

        public ushort MinorVersion
        {
            get => internalBody.minorVersion;
            set => internalBody.minorVersion = value;
        }

        public uint SectionTypeCount => Popcnt.PopCount(internalBody.stateOfSectionTypes);

        public void SetSection<TSection>(TSection section)
            where TSection : IReadyToRunSection
        {
            var index = (int)(TSection.SectionType - 100);
            Unsafe.As<CompilerIdentifierSection, TSection>(ref Unsafe.Add(ref compilerIdentifierSection, index)) = section;

            internalBody.stateOfSectionTypes = (internalBody.stateOfSectionTypes & ~(1u << index)) | (section is not null ? 1u : 0u) << index;
        }

        public uint GetVirtualSize() => GetPhysicalSize();

        public void UpdateOffsets(in RelocationParameters parameters)
        {
            internalBody.rva = parameters.Rva;
            internalBody.offset = parameters.Offset;
        }

        public uint GetPhysicalSize()
        {
            if (internalBody.lastCalculatedSize != 0)
                return internalBody.lastCalculatedSize;

            var stateOfSections = (ulong)internalBody.stateOfSectionTypes;
            var sectionCount = Popcnt.PopCount((uint)stateOfSections);
            var size = (uint)sizeof(READYTORUN_HEADER) + sectionCount * (uint)sizeof(READYTORUN_SECTION);
            var sectionIndex = 0;
            while (stateOfSections != 0)
            {
                var relativeSectionIndex = BitOperations.TrailingZeroCount(stateOfSections);
                sectionIndex += relativeSectionIndex++;
                stateOfSections >>= relativeSectionIndex;

                var section = Unsafe.As<CompilerIdentifierSection, IReadyToRunAbstractSection>(ref Unsafe.Add(ref compilerIdentifierSection, sectionIndex));
                var sectionSize = (uint)section.GetSectionSize();

                internalBody.sectionSizes[sectionIndex] = sectionSize;
                size += sectionSize;
            }

            internalBody.lastCalculatedSize = size;
            return size;
        }

        public void Write(BinaryStreamWriter streamWriter)
        {
            var rva = internalBody.rva;
            var totalSize = GetPhysicalSize();
            var byteArray = new byte[totalSize + 8];
            ref var bytes = ref byteArray[0];
            var stateOfSections = (ulong)internalBody.stateOfSectionTypes;
            var sectionCount = Popcnt.PopCount((uint)stateOfSections);

            Unsafe.As<byte, ulong>(ref bytes) = ((ulong)internalBody.minorVersion << 16 | internalBody.majorVersion) << 32 | (uint)ManagedNativeHeaderSignature.RTR;
            Unsafe.As<byte, ulong>(ref Unsafe.Add(ref bytes, 8)) = (ulong)sectionCount << 32 | (uint)internalBody.attributes;

            var sectionIndex = 0;
            var sectionHeaderOffset = (uint)sizeof(READYTORUN_HEADER);
            var sectionContentOffset = sectionHeaderOffset + sectionCount * (uint)sizeof(READYTORUN_SECTION);
            while (stateOfSections != 0)
            {
                var relativeSectionIndex = BitOperations.TrailingZeroCount(stateOfSections);
                sectionIndex += relativeSectionIndex++;
                stateOfSections >>= relativeSectionIndex;

                var section = Unsafe.As<CompilerIdentifierSection, IReadyToRunAbstractSection>(ref Unsafe.Add(ref compilerIdentifierSection, sectionIndex));
                var sectionSize = internalBody.sectionSizes[sectionIndex];
                var sectionRva = rva + sectionContentOffset;
                var sectionType = (uint)ReadyToRunSectionType.CompilerIdentifier + (uint)sectionIndex;

                Unsafe.As<ReadyToRunSectionType, ulong>(ref Unsafe.As<byte, READYTORUN_SECTION>(ref Unsafe.Add(ref bytes, sectionHeaderOffset)).Type) = (ulong)sectionRva << 32 | sectionType;
                Unsafe.As<byte, READYTORUN_SECTION>(ref Unsafe.Add(ref bytes, sectionHeaderOffset)).Section.Size = sectionSize;
                sectionHeaderOffset += (uint)sizeof(READYTORUN_SECTION);

                var writer = new SectionWriter(ref Unsafe.Add(ref bytes, sectionContentOffset));
                section.WriteSection(this, writer, sectionRva);
            }

            streamWriter.WriteBytes(byteArray, 0, (int)totalSize);
        }
        public static ReadyToRunDirectory FromCustomManagedNativeHeader(CustomManagedNativeHeader header)
        {
            var directory = new ReadyToRunDirectory();
            var directoryReader = new ReadyToRunDirectoryReader(header);
            directoryReader.ReadRTRDirectory(directory);
            return directory;
        }

        private const uint SectionTypeMaxCount = ReadyToRunSectionType.TypeMapAssemblyTargets - ReadyToRunSectionType.CompilerIdentifier;

        // Bypasses clr's memory layout restrictions. absurd.
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct InternalNoGCPointersBody
        {
            public uint stateOfSectionTypes;
            public ushort majorVersion;
            public ushort minorVersion;
            public ReadyToRunAttributes attributes;
            public uint lastCalculatedSize;
            public uint rva;
            public ulong offset;
            public fixed uint sectionSizes[(int)SectionTypeMaxCount];
        }
    }
}
