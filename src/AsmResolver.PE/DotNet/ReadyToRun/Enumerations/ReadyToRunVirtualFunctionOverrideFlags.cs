using System;

namespace AsmResolver.PE.DotNet.ReadyToRun.Enumerations
{
    // https://github.com/dotnet/runtime/blob/main/src/coreclr/inc/readytorun.h
    [Flags]
    public enum ReadyToRunVirtualFunctionOverrideFlags : uint
    {
        None = 0x00,
        VirtualFunctionOverridden = 0x01,
    }
}