namespace AsmResolver.DotNet.ReadyToRun.Amd64;

// todo: rename
public struct InterruptibleRange
{
    public InterruptibleRange(uint index, uint start, uint stop)
    {
        Index = index;
        StartOffset = start;
        StopOffset = stop;
    }

    public uint Index;
    public uint StartOffset;
    public uint StopOffset;
}
