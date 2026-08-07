using System.Runtime.CompilerServices;

namespace AsmResolver.DotNet.ReadyToRun
{
    public unsafe ref struct SectionReader
    {
        public SectionReader(ref byte reference)
        {
            Reference = ref reference;
        }

        public ref byte Reference;

        public byte* Pointer => (byte*)Unsafe.AsPointer(ref Reference);
    }
}
