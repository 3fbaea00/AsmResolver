#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
using AsmResolver.DotNet.ReadyToRun.Internal;
using System;
using System.Runtime.CompilerServices;

namespace AsmResolver.DotNet.ReadyToRun
{
    public unsafe ref struct SectionWriter
    {
        public SectionWriter(ref byte reference)
        {
            Reference = ref reference;
        }

        public ref byte Reference;

        /* WRITE BYTES */
        public void WriteBytes(byte[] bytes)
        {
            WriteBytes(ref bytes[0], (ulong)bytes.Length);
        }

        public void WriteBytes(Span<byte> span)
        {
            var publicSpan = Unsafe.As<Span<byte>, OpenSpan<byte>>(ref span);
            WriteBytes(ref publicSpan.Reference, publicSpan.Length);
        }

        public void WriteBytes(ReadOnlySpan<byte> span)
        {
            var publicSpan = Unsafe.As<ReadOnlySpan<byte>, OpenSpan<byte>>(ref span);
            WriteBytes(ref publicSpan.Reference, publicSpan.Length);
        }

        public void WriteBytes(void* pointer, ulong length)
        {
            Unsafe.CopyBlock(ref Reference, ref Unsafe.AsRef<byte>(pointer), (uint)length);
        }

        public void WriteBytes(ref byte reference, ulong length)
        {
            Unsafe.CopyBlock(ref Reference, ref reference, (uint)length);
        }

        public void WriteBytes(byte[] bytes, ulong offset)
        {
            WriteBytes(ref bytes[0], (ulong)bytes.Length, offset);
        }

        public void WriteBytes(Span<byte> span, ulong offset)
        {
            var publicSpan = Unsafe.As<Span<byte>, OpenSpan<byte>>(ref span);
            WriteBytes(ref publicSpan.Reference, publicSpan.Length, offset);
        }

        public void WriteBytes(ReadOnlySpan<byte> span, ulong offset)
        {
            var publicSpan = Unsafe.As<ReadOnlySpan<byte>, OpenSpan<byte>>(ref span);
            WriteBytes(ref publicSpan.Reference, publicSpan.Length, offset);
        }

        public void WriteBytes(void* pointer, ulong length, ulong offset)
        {
            Unsafe.CopyBlock(
                ref Unsafe.Add(ref Reference, (nuint)offset),
                ref Unsafe.AsRef<byte>(pointer),
                (uint)length);
        }

        public void WriteBytes(ref byte reference, ulong length, ulong offset)
        {
            Unsafe.CopyBlock(
                ref Unsafe.Add(ref Reference, (nuint)offset),
                ref reference,
                (uint)length);
        }

        /* WRITE BYTE */
        public void WriteByte(ulong value)
        {
            Unsafe.As<byte, ulong>(ref Reference) = value;
        }

        public void WriteByte(ulong value, ulong offset)
        {
            Unsafe.As<byte, ulong>(ref Unsafe.Add(ref Reference, (nuint)offset)) = value;
        }
    }
}
