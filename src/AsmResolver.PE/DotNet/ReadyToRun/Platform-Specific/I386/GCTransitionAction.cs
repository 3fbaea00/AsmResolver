namespace AsmResolver.PE.DotNet.ReadyToRun.I386
{
    public enum GCTransitionAction
    {
        Pop = 0x00,
        Push = 0x01,
        Kill = 0x02,
        Live = 0x03,
        Dead = 0x04
    }
}