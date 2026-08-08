using AsmResolver.DotNet.ReadyToRun.Enumerations;
using AsmResolver.DotNet.ReadyToRun.Internal.Extensions;
using AsmResolver.DotNet.ReadyToRun.Internal.Structures;
using AsmResolver.DotNet.ReadyToRun.Reader;
using AsmResolver.PE.File;
using System;
using System.Runtime.CompilerServices;

namespace AsmResolver.DotNet.ReadyToRun.Sections.ImportSections
{
    public abstract unsafe class ImportSection
    {
        public bool LazyInitialize;
        public bool ContainsCodePointers;

        private protected abstract void ReadContent(ref byte content, bool hasAuxData);

        public static ImportSection ReadImportSection(ReadyToRunDirectoryReader directoryReader, ref byte source)
        {
            ref var sectionReference = ref Unsafe.As<byte, READYTORUN_IMPORT_SECTION>(ref source);

            var sectionRvaSize = Unsafe.As<IMAGE_DATA_DIRECTORY, ulong>(ref sectionReference.Section);

            bool hasAuxData;
            var entrySize = (uint)sectionReference.EntrySize;
        TryAgain:
            if (entrySize == 8)
            {
                var entriesSize = sectionRvaSize >> 32;
                if (entriesSize != 0)
                {
                    const int InitialBufferSize = 1024 * 128;
                    var stackBuffer = stackalloc byte[InitialBufferSize];
                    ref var buffer = ref Unsafe.AsRef<byte>(stackBuffer);

                    if (entriesSize > InitialBufferSize * 8 / 24)
                    {
                        var managedBuffer = new byte[entriesSize + entriesSize + entriesSize];
                        buffer = ref managedBuffer[0];
                    }

                    var peFile = directoryReader.RawHeader.File;
                    hasAuxData = ReadImportEntries64Bit(peFile, ref buffer, sectionRvaSize, ref sectionReference);
                }
                else
                {
                    var section = CreateEmptyImportSection(ref sectionReference);
                    hasAuxData = false;
                }
            }
            else
            {
                if (entrySize == 4)
                {
                    hasAuxData = ReadImportEntries32Bit();
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

            // return: has aux data
            [MethodImpl(MethodImplOptions.NoInlining)]
            static bool ReadImportEntries64Bit(PEFile peFile, ref byte destination, ulong sectionsRvaSize, ref READYTORUN_IMPORT_SECTION sectionReference)
            {
                var sectionsSize = sectionsRvaSize >> 32;
                var sectionsRva = (uint)sectionsRvaSize;
                peFile.TryGetSectionContainingRva(sectionsRva, out var sectionsSection);
                ref var sectionsReference = ref Unsafe.Add(ref sectionsSection.Contents.GetData(), sectionsRva - sectionsSection.Rva);

                var size = (nuint)sectionsSize;
                while (size != 0)
                {
                    size -= 8;
                    Unsafe.As<byte, ulong>(ref Unsafe.Add(ref destination, size)) = Unsafe.As<byte, ulong>(ref Unsafe.Add(ref sectionsReference, size));
                }
                destination = ref Unsafe.Add(ref destination, (nuint)sectionsSize);

                var signaturesRvau32_auxiliaryDataRvau32 = Unsafe.As<uint, ulong>(ref sectionReference.Signatures);
                var signaturesRva = (uint)signaturesRvau32_auxiliaryDataRvau32;
                var signaturesSection = sectionsSection;
                if (!sectionsSection.ContainsRva(signaturesRva))
                {
                    peFile.TryGetSectionContainingRva(signaturesRva, out var section);
                    signaturesSection = section;
                }
                ref var signaturesReference = ref Unsafe.Add(ref signaturesSection.Contents.GetData(), signaturesRva - signaturesSection.Rva);

                sectionsSize >>= 1;
                size = (nuint)sectionsSize;
                while (size != 0)
                {
                    size -= 4;
                    Unsafe.As<byte, uint>(ref Unsafe.Add(ref destination, size)) = Unsafe.As<byte, uint>(ref Unsafe.Add(ref signaturesReference, size));
                }
                destination = ref Unsafe.Add(ref destination, (nuint)sectionsSize);

                var auxDatasRva = (uint)(signaturesRvau32_auxiliaryDataRvau32 >> 32);
                if (auxDatasRva != 0)
                {
                    var auxDatasSection = sectionsSection;
                    if (!sectionsSection.ContainsRva(auxDatasRva))
                    {
                        peFile.TryGetSectionContainingRva(auxDatasRva, out var signaturesSection_);
                        signaturesSection = signaturesSection_;
                    }
                    ref var auxDataReference = ref Unsafe.Add(ref auxDatasSection.Contents.GetData(), auxDatasRva - auxDatasSection.Rva);

                    size = (nuint)sectionsSize;
                    while (size != 0)
                    {
                        size -= 4;
                        Unsafe.As<byte, uint>(ref Unsafe.Add(ref destination, size)) = Unsafe.As<byte, uint>(ref Unsafe.Add(ref auxDataReference, size));
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }

            // add mass rva -> offset converter in ex of PEFile
            [MethodImpl(MethodImplOptions.NoInlining)]
            static bool ReadImportEntries32Bit()
            {
                return default;
            }

            static ImportSection CreateEmptyImportSection(ref READYTORUN_IMPORT_SECTION sectionReference)
            {
                ImportSection section = sectionReference.Type switch
                {
                    ReadyToRunImportSectionType.Unknown => new UnknownImportSection(),
                    ReadyToRunImportSectionType.StubDispatch => new StubDispatchImportSection(),
                    ReadyToRunImportSectionType.StringHandle => new StringHandleImportSection(),
                    ReadyToRunImportSectionType.ILBodyFixups => new ILBodyFixupsImportSection(),
                    _ => throw new NotSupportedException()
                };

                return section;
            }
        }
    }
}
