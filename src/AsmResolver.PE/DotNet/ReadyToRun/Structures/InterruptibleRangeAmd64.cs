namespace AsmResolver.PE.DotNet.ReadyToRun.Structures;

public struct InterruptibleRangeAmd64
{
    public InterruptibleRangeAmd64(uint index, uint start, uint stop)
    {
        Index = index;
        StartOffset = start;
        StopOffset = stop;
    }

    public uint Index;
    public uint StartOffset;
    public uint StopOffset;
}
