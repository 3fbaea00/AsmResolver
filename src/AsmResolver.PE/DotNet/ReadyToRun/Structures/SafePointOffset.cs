namespace AsmResolver.PE.DotNet.ReadyToRun.Structures;

public struct SafePointOffset
{
    public SafePointOffset(int index, uint value)
    {
        Index = index;
        Value = value;
    }

    public int Index;
    public uint Value;
}
