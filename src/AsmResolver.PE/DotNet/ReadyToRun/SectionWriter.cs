namespace AsmResolver.PE.DotNet.ReadyToRun
{
    public unsafe struct SectionWriter
    {
        public SectionWriter(void* pointer, ulong length)
        {
            Pointer = pointer;
            Length = length;
        }

        public void* Pointer;
        public ulong Length;
    }
}
