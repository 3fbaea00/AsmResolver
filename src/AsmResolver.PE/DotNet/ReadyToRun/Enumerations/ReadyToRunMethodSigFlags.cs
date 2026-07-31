using System;

namespace AsmResolver.PE.DotNet.ReadyToRun.Enumerations
{
    // https://github.com/dotnet/runtime/blob/main/src/coreclr/inc/readytorun.h
    /// <summary>
    /// Constants for method and field encoding
    /// </summary>
    [Flags]
    public enum ReadyToRunMethodSigFlags : uint
    {
        None                = 0x000,
        UnboxingStub        = 0x001,
        InstantiatingStub   = 0x002,
        MethodInstantiation = 0x004,
        SlotInsteadOfToken  = 0x008,
        MemberRefToken      = 0x010,
        Constrained         = 0x020,
        OwnerType           = 0x040,
        UpdateContext       = 0x080,
        AsyncVariant        = 0x100,
    }
}