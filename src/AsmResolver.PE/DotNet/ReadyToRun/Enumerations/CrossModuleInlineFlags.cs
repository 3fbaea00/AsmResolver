namespace AsmResolver.PE.DotNet.ReadyToRun.Enumerations
{
    public enum CrossModuleInlineFlags : uint
    {
        CrossModuleInlinee = 0x1,
        HasCrossModuleInliners = 0x2,
        CrossModuleInlinerIndexShift = 2,
        InlinerRidHasModule = 0x1,
        InlinerRidShift = 1,
    }
}