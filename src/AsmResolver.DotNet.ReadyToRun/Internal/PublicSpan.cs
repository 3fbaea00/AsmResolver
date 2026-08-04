namespace AsmResolver.DotNet.ReadyToRun.Internal
{
    internal unsafe ref struct PublicSpan
    {
        public void* Pointer;
        public ulong Length;
    }
}
