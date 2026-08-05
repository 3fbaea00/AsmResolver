namespace AsmResolver.DotNet.ReadyToRun
{
    public unsafe struct SectionReader
    {
        public SectionReader(void* pointer)
        {
            Pointer = pointer;
        }

        public void* Pointer;

        public ulong ReadUInt64(ulong offset) => *(ulong*)((byte*)Pointer + offset);
    }
}
