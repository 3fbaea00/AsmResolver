using System;
using System.Runtime.InteropServices;

namespace AsmResolver.PE.DotNet.ReadyToRun.MachO;

/// <summary>
/// A 16 byte buffer used to store names in Mach-O load commands.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct NameBuffer
{
    private ulong _nameLower;
    private ulong _nameUpper;

    private const int BufferLength = 16;

    private NameBuffer(ReadOnlySpan<byte> nameBytes)
    {
        byte[] buffer = new byte[BufferLength];
        nameBytes.CopyTo(buffer);

        if (BitConverter.IsLittleEndian)
        {
            _nameLower = BitConverter.ToUInt64(buffer, 0);
            _nameUpper = BitConverter.ToUInt64(buffer, 8);
        }
        else
        {
            _nameLower = BitConverter.ToUInt64(buffer, 8);
            _nameUpper = BitConverter.ToUInt64(buffer, 0);
        }
    }

    public static NameBuffer __TEXT = new NameBuffer("__TEXT"u8);
    public static NameBuffer __LINKEDIT = new NameBuffer("__LINKEDIT"u8);

    public unsafe string GetString()
    {
        fixed (ulong* ptr = &_nameLower)
        {
            byte* bytePtr = (byte*)ptr;
            int length = 0;
            while (length < BufferLength && bytePtr[length] != 0)
            {
                length++;
            }

            var bytes = new byte[length];
            new Span<byte>(bytePtr, length).CopyTo(bytes);

            return System.Text.Encoding.UTF8.GetString(bytes, 0, length);
        }
    }
}
