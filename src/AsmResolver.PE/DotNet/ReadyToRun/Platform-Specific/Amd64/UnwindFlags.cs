namespace AsmResolver.PE.DotNet.ReadyToRun.Amd64;

// https://github.com/dotnet/runtime/blob/main/src/coreclr/inc/win64unwind.h
public enum UnwindFlags : byte
{
    /// <summary>
    /// UNW_FLAG_NHANDLER
    /// </summary>
    NoHandler,

    /// <summary>
    /// UNW_FLAG_EHANDLER
    /// </summary>
    ExceptionHandler,

    /// <summary>
    /// UNW_FLAG_UHANDLER
    /// </summary>
    UnwindHandler,

    /// <summary>
    /// UNW_FLAG_CHAININFO
    /// </summary>
    ChainInfo,
}
