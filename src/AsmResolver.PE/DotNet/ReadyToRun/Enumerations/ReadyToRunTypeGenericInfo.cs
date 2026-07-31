using System;

namespace AsmResolver.PE.DotNet.ReadyToRun.Enumerations 
{
    // https://github.com/dotnet/runtime/blob/main/src/coreclr/inc/readytorun.h
    [Flags]
    public enum ReadyToRunTypeGenericInfo : byte
    {
        GenericCountMask = 0x3,
        HasConstraints = 0x4,
        HasVariance = 0x8,
    }
}