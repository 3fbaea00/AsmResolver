using System;

namespace AsmResolver.PE.DotNet.ReadyToRun.Enumerations
{
    // https://github.com/dotnet/runtime/blob/main/src/coreclr/inc/readytorun.h
    [Flags]
    public enum ReadyToRunFieldSigFlags : byte
    {
        MemberRefToken = 0x10,
        OwnerType = 0x40,
    }
}