#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

using AsmResolver.DotNet.ReadyToRun.Enumerations;
using AsmResolver.DotNet.ReadyToRun.Internal;
using AsmResolver.DotNet.ReadyToRun.Reader;
using System;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;

namespace AsmResolver.DotNet.ReadyToRun.Sections
{
    public class CompilerIdentifierSection : IReadyToRunImageSection
    {
        public static ReadyToRunSectionType SectionType => ReadyToRunSectionType.CompilerIdentifier;

        public Utf8String Identifier;

        public ulong GetSectionSize() => (ulong)Identifier.ByteCount + 1;

        public void ReadSection(ReadyToRunDirectoryReader directoryReader, ref byte source, uint sourceSize)
        {
            var lenght = sourceSize - 1;
            var span = new OpenSpan<byte>()
            {
                Reference = ref source,
                Length = lenght,
            };
            Identifier = new Utf8String(Unsafe.As<OpenSpan<byte>, ReadOnlySpan<byte>>(ref span));
        }

        public void WriteSection(ReadyToRunDirectory directory, ref byte destination, uint rva)
        {
            Identifier.AsSpan().CopyTo(new Span<byte>(ref destination));
        }
    }
}
