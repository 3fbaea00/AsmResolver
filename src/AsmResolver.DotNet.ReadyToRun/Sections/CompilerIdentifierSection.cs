#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

using AsmResolver.DotNet.ReadyToRun.Enumerations;
using AsmResolver.DotNet.ReadyToRun.Internal;
using System;

namespace AsmResolver.DotNet.ReadyToRun.Sections
{
    public unsafe class CompilerIdentifierSection : IReadyToRunImageSection
    {
        public static ReadyToRunSectionType SectionType => ReadyToRunSectionType.CompilerIdentifier;

        public Utf8String Identifier;

        public void ReadSection(ReadyToRunDirectory directory, SectionReader reader, uint sectionSize)
        {
            var lenght = sectionSize - 1;
            var span = new PublicSpan()
            {
                Pointer = reader.Pointer,
                Length = lenght,
            };
            Identifier = new Utf8String(*(ReadOnlySpan<byte>*)&span);
        }

        public ulong GetSectionSize() => (ulong)Identifier.ByteCount + 1;

        public void WriteSection(ReadyToRunDirectory directory, SectionWriter writer, uint rva)
        {
            var identifierSpan = Identifier.AsSpan();
            writer.WriteBytes(identifierSpan);
        }
    }
}
