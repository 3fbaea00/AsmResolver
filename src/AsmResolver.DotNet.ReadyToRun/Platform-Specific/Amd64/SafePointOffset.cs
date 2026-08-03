namespace AsmResolver.DotNet.ReadyToRun.Amd64;

// todo: rename
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
