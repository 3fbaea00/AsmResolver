using AsmResolver.DotNet.ReadyToRun.Enumerations;
using AsmResolver.DotNet.ReadyToRun.Internal;
using AsmResolver.DotNet.ReadyToRun.Internal.Extensions;
using AsmResolver.DotNet.ReadyToRun.Sections;
using AsmResolver.IO;
using AsmResolver.PE.DotNet;
using AsmResolver.PE.File;
using System;
using System.Diagnostics.Tracing;
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

        [FieldOffset(0xD8)] private InternalNoGCPointersBody internalBody;

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

        public uint SectionCount => Popcnt.PopCount(internalBody.stateOfSections);

        public void SetSection<TSection>(TSection section)
            where TSection : IReadyToRunSection
        {
            var index = (int)(TSection.SectionType - 100);
            Unsafe.As<CompilerIdentifierSection, TSection>(ref Unsafe.Add(ref compilerIdentifierSection, index)) = section;

            internalBody.stateOfSections = (internalBody.stateOfSections & ~(1u << index)) | (section is not null ? 1u : 0u) << index;
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

            var stateOfSections = (ulong)internalBody.stateOfSections;
            var sectionCount = Popcnt.PopCount((uint)stateOfSections);
            var size = HeaderSize + sectionCount * SectionHeaderSize;
            var sectionIndex = 0;
            while (stateOfSections != 0)
            {
                var relativeSectionIndex = BitOperations.TrailingZeroCount(stateOfSections);
                sectionIndex += relativeSectionIndex++;
                stateOfSections >>= relativeSectionIndex;
                var section = Unsafe.As<CompilerIdentifierSection, IReadyToRunAbstractSection>(ref Unsafe.Add(ref compilerIdentifierSection, sectionIndex));
                var sectionSize = (uint)section.CalculateContentSize();

                internalBody.sectionSizes[sectionIndex] = sectionSize;
                size += sectionSize;
            }

            internalBody.lastCalculatedSize = size;
            return size;
        }

        public void Write(BinaryStreamWriter streamWriter)
        {
            var rva = internalBody.rva;
            var byteArray = new byte[GetPhysicalSize()];
            fixed (byte* bytes = byteArray)
            {
                var stateOfSections = (ulong)internalBody.stateOfSections;
                var sectionCount = Popcnt.PopCount((uint)stateOfSections);

                *(ulong*)bytes = ((ulong)internalBody.minorVersion << 16 | internalBody.majorVersion) << 32 | (uint)ManagedNativeHeaderSignature.RTR;
                *(ulong*)(bytes + 8) = (ulong)sectionCount << 32 | (uint)internalBody.attributes;

                var sectionIndex = 0;
                var sectionHeaderOffset = HeaderSize;
                var sectionContentOffset = HeaderSize + sectionCount * SectionHeaderSize;
                while (stateOfSections != 0)
                {
                    var relativeSectionIndex = BitOperations.TrailingZeroCount(stateOfSections);
                    sectionIndex += relativeSectionIndex++;
                    stateOfSections >>= relativeSectionIndex;
                    var section = Unsafe.As<CompilerIdentifierSection, IReadyToRunAbstractSection>(ref Unsafe.Add(ref compilerIdentifierSection, sectionIndex));
                    var sectionSize = internalBody.sectionSizes[sectionIndex];
                    var sectionRva = rva + sectionContentOffset;
                    var sectionType = (uint)ReadyToRunSectionType.CompilerIdentifier + (uint)sectionIndex;
                    *(ulong*)(bytes + sectionHeaderOffset) = (ulong)sectionRva << 32 | sectionType;
                    *(uint*)(bytes + sectionHeaderOffset + /* Type */ sizeof(uint) + /* Rva */ sizeof(uint)) = sectionSize;

                    sectionHeaderOffset += SectionHeaderSize;

                    var sectionData = bytes + sectionContentOffset;
                    var writer = new SectionWriter(sectionData);
                    section.WriteContent(writer, sectionRva);
                }
            }

            streamWriter.WriteBytes(byteArray);
        }

        public static ReadyToRunDirectory FromCustomManagedNativeHeader(CustomManagedNativeHeader header)
        {
            var file = header.File;
            var contentsSegment = header.Contents;
            var offset = 0ul;
            var contentsBytes = contentsSegment.GetDataNoChecks(ref offset);
            var length = (ulong)contentsBytes.Length - offset;

            fixed (byte* contents = contentsBytes)
            {
                var directory = new ReadyToRunDirectory();

                if (length < HeaderSize)
                    throw new InvalidOperationException();

                Unsafe.As<ushort, ulong>(ref directory.internalBody.majorVersion) = *(ulong*)(contents + offset +
                    /* Signature */ sizeof(ManagedNativeHeaderSignature)
                );

                var numOfSections = *(uint*)(contents + offset +
                    /* Signature    */ sizeof(ManagedNativeHeaderSignature) +
                    /* MajorVersion */ sizeof(ushort) +
                    /* MinorVersion */ sizeof(ushort) +
                    /* Flags        */ sizeof(ReadyToRunAttributes)
                );

                var expectedSize = HeaderSize + numOfSections * SectionHeaderSize;
                if (length < expectedSize)
                    throw new InvalidOperationException();

                offset += HeaderSize;                
                var numOfLeftSections = numOfSections;
                var stateOfSections = 0u;
                PESection peSection = null;
                byte[] peSectionBytes = null;
                while (numOfLeftSections != 0)
                {
                    numOfLeftSections--;

                    var sectionType = *(ReadyToRunSectionType*)(contents + offset);
                    var rva = *(uint*)(contents + offset + sizeof(ReadyToRunSectionType));
                    var size = *(uint*)(contents + offset + sizeof(ReadyToRunSectionType) + sizeof(uint));
                    offset += sizeof(ReadyToRunSectionType) + sizeof(uint) + sizeof(uint);

                    if (peSection is null || !peSection.ContainsRva(rva))
                    {
                        peSection = file.GetSectionContainingRva(rva);
                        var peSectionOffset = 0ul;
                        peSectionBytes = peSection.Contents.GetData(ref peSectionOffset, rva, size);
                    }

                    fixed (byte* peSectionData = peSectionBytes)
                    {
                        var fileOffset = peSection.RvaToFileOffset(rva);
                        var sectionData = peSectionData + fileOffset;

                       if (sectionType is not ReadyToRunSectionType.CompilerIdentifier)
                            continue;

                        var section = sectionType switch
                        {
                            ReadyToRunSectionType.CompilerIdentifier => new CompilerIdentifierSection(),
                            _ => throw new NotImplementedException()
                        };

                        var sectionReader = new SectionReader(sectionData);
                        section.ReadContent(sectionReader, size);

                        var index = (int)(sectionType - 100);
                        Unsafe.As<CompilerIdentifierSection, IReadyToRunAbstractSection>(ref Unsafe.Add(ref directory.compilerIdentifierSection, index)) = section;
                        stateOfSections |= 1u << index;
                    }
                }

                directory.internalBody.stateOfSections = stateOfSections;

                return directory;
            }
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

        // Bypasses clr's memory layout restrictions. absurd.
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct InternalNoGCPointersBody
        {
            public uint stateOfSections;
            public ushort majorVersion;
            public ushort minorVersion;
            public ReadyToRunAttributes attributes;
            public uint lastCalculatedSize;
            public uint rva;
            public ulong offset;
            public fixed uint sectionSizes[(int)SectionMaxCount];
        }
    }
}
