using System;

namespace AsmResolver.PE.DotNet.ReadyToRun.Enumerations
{
    // https://github.com/dotnet/runtime/blob/main/src/coreclr/inc/readytorun.h
    [Flags]
    public enum ReadyToRunTypeLayoutFlags : byte
    {
        HFA = 0x01,
        ManagedAlignment = 0x02,
        NativeAlignment = 0x04,
        GCLayout = 0x08,
        GCLayoutNoGCPointers = 0x10,
    }
}