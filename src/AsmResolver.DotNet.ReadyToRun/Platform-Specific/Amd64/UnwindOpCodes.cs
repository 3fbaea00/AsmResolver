namespace AsmResolver.DotNet.ReadyToRun.Amd64;

// https://github.com/dotnet/runtime/blob/main/src/coreclr/inc/win64unwind.h
public enum UnwindOpCodes : byte
{
    /// <summary>
    /// UWOP_PUSH_NONVOL
    /// </summary>
    PushNonVolatile,

    /// <summary>
    /// UWOP_ALLOC_LARGE
    /// </summary>
    AllocateLarge,

    /// <summary>
    /// UWOP_ALLOC_SMALL
    /// </summary>
    AllocateSmall,

    /// <summary>
    /// UWOP_SET_FPREG
    /// </summary>
    SetFramePointerRegister,

    /// <summary>
    /// UWOP_SAVE_NONVOL
    /// </summary>
    SaveNonVolatile,

    /// <summary>
    /// UWOP_SAVE_NONVOL_FAR
    /// </summary>
    SaveNonVolatileFar,

    /// <summary>
    /// UWOP_EPILOG
    /// </summary>
    Epilog,

    /// <summary>
    /// UWOP_SPARE_CODE
    /// </summary>
    SpareCode,

    /// <summary>
    /// UWOP_SAVE_XMM128
    /// </summary>
    SaveXmm128,

    /// <summary>
    /// UWOP_SAVE_XMM128_FAR
    /// </summary>
    SaveXmm128Far,

    /// <summary>
    /// UWOP_PUSH_MACHFRAME
    /// </summary>
    PushMachineFrame,

    /// <summary>
    /// UWOP_SET_FPREG_LARGE
    /// is only for Unix.
    /// </summary>
    SetFramePointerRegisterLarge,
}
