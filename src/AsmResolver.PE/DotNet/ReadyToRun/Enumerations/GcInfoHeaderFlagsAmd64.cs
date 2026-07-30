namespace AsmResolver.PE.DotNet.ReadyToRun.Enumerations;

// https://github.com/dotnet/runtime/blob/main/src/coreclr/inc/gcinfodecoder.h
public enum GcInfoHeaderFlags
{
    IsVarArg                          = 0x001,
    HasSecurityObject                 = 0x002,
    HasGSCookie                       = 0x004,
    HasPspSym                         = 0x008,
    HasGenericsInstContextMask        = 0x030,
    HasGenericsInstContextNone        = 0x000,
    HasGenericsInstContextMT          = 0x010,
    HasGenericsInstContextMD          = 0x020,
    HasGenericsInstContextThis        = 0x030,
    HasStackBaseRegister              = 0x040,
    WantsReportOnlyLeaf               = 0x080,
    HasTailCalls                      = 0x080, // for Arm64
    HasEditAndContinuesPreservedSlots = 0x100,
    ReversePInvokeFrame = 0x200,

    FlagsBitSizeVersion1 = 9,
    FlagsBitSize = 10,
};
