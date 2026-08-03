#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

using AsmResolver.DotNet.ReadyToRun.Enumerations;
using System;

namespace AsmResolver.DotNet.ReadyToRun.Sections
{
    public unsafe class CompilerIdentifierSection : IReadyToRunSection
    {
        public static ReadyToRunSectionType SectionType => ReadyToRunSectionType.CompilerIdentifier;

        public Utf8String Identifier;

        public void ReadContent(SectionReader reader, uint contentSize)
        {
            var lenght = contentSize - 1;
            var span = new PublicSpan<byte>()
            {
                Pointer = (byte*)reader.Pointer,
                Length = lenght,
            };
            Identifier = new Utf8String(*(ReadOnlySpan<byte>*)&span);
        }

        public ulong CalculateContentSize() => (ulong)Identifier.ByteCount;

        // maybe optimize for flat sections without references
        public void WriteContent(SegmentBuilder segmentBuilder)
        {
            var identifierSpan = Identifier.AsSpan();
            var length = identifierSpan.Length + 1;

            var byteArray = new byte[length];
            fixed (byte* bytes = byteArray)
            {
                var writer = new SectionWriter(bytes);
                writer.WriteBytes(identifierSpan);
                writer.WriteByte(0, (ulong)identifierSpan.Length);
            }

            var identifierSegment = new DataSegment(byteArray);
            segmentBuilder.Add(identifierSegment);
        }
    }
}
