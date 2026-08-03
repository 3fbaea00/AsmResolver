#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

using AsmResolver.PE.DotNet.ReadyToRun.Enumerations;
using System;
using System.Runtime.CompilerServices;

namespace AsmResolver.PE.DotNet.ReadyToRun.Sections
{
    public unsafe class CompilerIdentifierSection : IReadyToRunSection
    {
        public ReadyToRunSectionType SectionType => ReadyToRunSectionType.CompilerIdentifier;

        public Utf8String Identifier;

        public void ReadContent(SectionReader reader)
        {
            Identifier = new Utf8String(*(ReadOnlySpan<byte>*)Unsafe.AsPointer(ref reader));
        }

        public void WriteContent(SectionWriter writer)
        {

        }
    }
}
