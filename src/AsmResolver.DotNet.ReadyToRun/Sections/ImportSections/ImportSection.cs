using AsmResolver.DotNet.ReadyToRun.Enumerations;
using AsmResolver.DotNet.ReadyToRun.Internal.Structures;
using AsmResolver.DotNet.ReadyToRun.Reader;
using AsmResolver.PE.File;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

namespace AsmResolver.DotNet.ReadyToRun.Sections.ImportSections
{
    public abstract unsafe class ImportSection
    {
        public bool LazyInitialize;
        public bool ContainsCodePointers;

        public static ImportSection ReadImportSection(ReadyToRunDirectoryReader directoryReader, ref byte source)
        {
            ref var sectionReference = ref Unsafe.As<byte, READYTORUN_IMPORT_SECTION>(ref source);

            var sectionRvaSize = Unsafe.As<IMAGE_DATA_DIRECTORY, ulong>(ref sectionReference.Section);

            var entrySize = (uint)sectionReference.EntrySize;
        TryAgain:
            if (entrySize == 8)
            {

                const int InitialBufferSize = 1024 * 128;
                var stackBuffer = stackalloc byte[InitialBufferSize];
                ref var buffer = ref Unsafe.AsRef<byte>(stackBuffer);
                var entriesSize = sectionRvaSize >> 32;
                if (entriesSize > InitialBufferSize * 8 / 24)
                {
                    var managedBuffer = new byte[entriesSize + entriesSize + entriesSize];
                    buffer = ref managedBuffer[0];
                }

                var peFile = directoryReader.RawHeader.File;
                var signaturesRvau32_auxiliaryDataRvau32 = Unsafe.As<uint, ulong>(ref sectionReference.Signatures);
                ReadImportEntries64Bit(peFile, ref buffer, sectionRvaSize, signaturesRvau32_auxiliaryDataRvau32);
            }
            else
            {
                if (entrySize == 4)
                {
                    ReadImportEntries32Bit();
                }
                else
                {
                    entrySize = (uint)directoryReader.RawHeader.ReaderContext.Platform.PointerSize;
                    goto TryAgain;
                }
            }

            ImportSection importSection;
            switch (sectionReference.Type)
            {
                case ReadyToRunImportSectionType.Unknown:
                    importSection = new UnknownImportSection();
                    break;
                default:
                    throw new NotImplementedException();
            }

            var flags = (uint)sectionReference.Flags;
            Unsafe.As<bool, ushort>(ref importSection.LazyInitialize) = (ushort)(
                (flags & (uint)ReadyToRunImportSectionFlags.PCode) << 8 |
                (~flags & (uint)ReadyToRunImportSectionFlags.Eager));

            return importSection;

            [MethodImpl(MethodImplOptions.NoInlining)]
            static void ReadImportEntries64Bit(PEFile peFile, ref byte destination, ulong sectionRvaSize, ulong signaturesRvau32_auxiliaryDataRvau32)
            {
                var entriesSize = sectionRvaSize >> 32;
                var entriesRva = (uint)sectionRvaSize;
                var entriesOffset = peFile.RvaToFileOffset(entriesRva);
                var signaturesRva = (uint)signaturesRvau32_auxiliaryDataRvau32;
                var signaturesOffset = signaturesRva != 0 ? peFile.RvaToFileOffset(signaturesRva) : 0ul;
                var auxiliaryDatasRva = (uint)(signaturesRvau32_auxiliaryDataRvau32 >> 32);
                var auxiliaryDatasOffset = auxiliaryDatasRva != 0 ? peFile.RvaToFileOffset(auxiliaryDatasRva) : 0ul;

                ref byte source = ref Unsafe.NullRef<byte>();
                while (entriesSize != 0)
                {
                    entriesSize -= 8;
                    Unsafe.As<byte, ulong>(ref destination) = Unsafe.As<byte, ulong>(ref source);
                    destination = ref Unsafe.Add(ref destination, 8);

                    // todo: instead of it use avx
                    if (signaturesOffset != 0)
                    {
                        var signatureRva = Unsafe.As<byte, uint>(ref source);
                        Unsafe.As<byte, ulong>(ref destination) = signaturesOffset;
                        destination = ref Unsafe.Add(ref destination, 8);
                    }
                }
            }

            // add mass rva -> offset converter in ex of PEFile
            [MethodImpl(MethodImplOptions.NoInlining)]
            static void ReadImportEntries32Bit()
            {

            }
        }
    }
}
