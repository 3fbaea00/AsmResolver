using System;

namespace AsmResolver.DotNet.ReadyToRun.Enumerations
{
    // https://github.com/dotnet/runtime/blob/main/src/coreclr/inc/readytorun.h
    [Flags]
    public enum ClassProfileFlags : uint
    {
        IsInterface = 0x40000000,
        IsClass = 0x80000000,
    }
}
