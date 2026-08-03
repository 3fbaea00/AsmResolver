#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
using System;
using System.Runtime.CompilerServices;

namespace AsmResolver.DotNet.ReadyToRun
{
    public unsafe struct SectionWriter
    {
        public SectionWriter(void* pointer)
        {
            Pointer = pointer;
        }

        public void* Pointer;

        /* WRITE BYTES */
        public void WriteBytes(byte[] bytes)
        {
            WriteBytes(ref bytes[0], (ulong)bytes.Length);
        }

        public void WriteBytes(Span<byte> span)
        {
            var writer = (PublicSpan<byte>*)&span;
            WriteBytes(writer->Pointer, writer->Length);
        }

        public void WriteBytes(ReadOnlySpan<byte> span)
        {
            var writer = (PublicSpan<byte>*)&span;
            WriteBytes(writer->Pointer, writer->Length);
        }

        public void WriteBytes(void* pointer, ulong length)
        {
            Unsafe.CopyBlock(ref Unsafe.AsRef<byte>(pointer), ref Unsafe.AsRef<byte>(Pointer), (uint)length);
        }

        public void WriteBytes(ref byte reference, ulong length)
        {
            Unsafe.CopyBlock(ref reference, ref Unsafe.AsRef<byte>(Pointer), (uint)length);
        }

        public void WriteBytes(byte[] bytes, ulong offset)
        {
            WriteBytes(ref bytes[0], (ulong)bytes.Length, offset);
        }

        public void WriteBytes(Span<byte> span, ulong offset)
        {
            var writer = (PublicSpan<byte>*)&span;
            WriteBytes(writer->Pointer, writer->Length, offset);
        }

        public void WriteBytes(ReadOnlySpan<byte> span, ulong offset)
        {
            var writer = (PublicSpan<byte>*)&span;
            WriteBytes(writer->Pointer, writer->Length, offset);
        }

        public void WriteBytes(void* pointer, ulong length, ulong offset)
        {
            Unsafe.CopyBlock(
                ref Unsafe.Add(ref Unsafe.AsRef<byte>(Pointer), (nuint)offset),
                ref Unsafe.AsRef<byte>(pointer),
                (uint)length);
        }

        public void WriteBytes(ref byte reference, ulong length, ulong offset)
        {
            Unsafe.CopyBlock(
                ref Unsafe.Add(ref Unsafe.AsRef<byte>(Pointer), (nuint)offset),
                ref reference,
                (uint)length);
        }

        /* WRITE BYTE */
        public void WriteByte(ulong value)
        {
            *(ulong*)Pointer = value;
        }

        public void WriteByte(ulong value, ulong offset)
        {
            *(ulong*)((byte*)Pointer + offset) = value;
        }
    }
}
