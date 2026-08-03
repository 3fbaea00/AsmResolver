using System;

namespace AsmResolver.DotNet.ReadyToRun.Enumerations
{
    // https://github.com/dotnet/runtime/blob/main/src/coreclr/inc/readytorun.h
    [Flags]
    internal enum ReadyToRunCrossModuleInlineFlags : uint
    {
        CrossModuleInlinee = 0x1,
        HasCrossModuleInliners = 0x2,
        CrossModuleInlinerIndexShift = 2,
        InlinerRidHasModule = 0x1,
        InlinerRidShift = 1,
    };
}