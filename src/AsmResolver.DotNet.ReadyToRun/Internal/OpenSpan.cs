namespace AsmResolver.DotNet.ReadyToRun.Internal
{
    internal ref struct OpenSpan<T>
    {
        public ref T Reference;
        public ulong Length;
    }
}
