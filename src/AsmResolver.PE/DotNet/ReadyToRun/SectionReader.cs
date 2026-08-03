namespace AsmResolver.PE.DotNet.ReadyToRun
{
    public unsafe struct SectionReader
    {
        public SectionReader(void* pointer, ulong length)
        {
            Pointer = pointer;
            Length = length;
        }

        public void* Pointer;
        public ulong Length;
    }
}
