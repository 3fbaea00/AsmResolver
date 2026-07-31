using System;

namespace AsmResolver.PE.DotNet.ReadyToRun.Enumerations
{
    // https://github.com/dotnet/runtime/blob/main/src/coreclr/inc/readytorun.h
    [Flags]
    public enum ReadyToRunImportSectionFlags : ushort
    {
        None  = 0x0000,
        Eager = 0x0001, // Section at module load time.
        PCode = 0x0004, // Section contains pointers to code
    }
}