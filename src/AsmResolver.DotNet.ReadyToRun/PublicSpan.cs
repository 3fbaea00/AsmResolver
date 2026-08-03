using System.Runtime.InteropServices;

namespace AsmResolver.DotNet.ReadyToRun
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe ref struct PublicSpan<T> where T : unmanaged
    {
        [FieldOffset(0x00)]
        public ref T Reference;

        [FieldOffset(0x00)]
        public T* Pointer;

        [FieldOffset(0x08)]
        public ulong Length;
    }
}
