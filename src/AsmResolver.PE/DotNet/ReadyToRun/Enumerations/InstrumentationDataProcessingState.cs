namespace AsmResolver.PE.DotNet.ReadyToRun.Enumerations
{
    public enum InstrumentationDataProcessingState
    {
        Done = 0,
        ILOffset = 0x1,
        Type = 0x2,
        Count = 0x4,
        Other = 0x8,
        UpdateProcessMask = 0xF,
        UpdateProcessMaskFlag = 0x100,
    }
}