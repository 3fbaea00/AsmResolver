namespace AsmResolver.DotNet.ReadyToRun.Enumerations
{
    // https://github.com/dotnet/runtime/blob/main/src/coreclr/inc/readytorun.h
    // Enum used for HFA type recognition.
    // Supported across architectures, so that it can be used in altjits and cross-compilation.
    public enum ReadyToRunHFAElemType
    {
        None = 0,
        Float32 = 1,
        Float64 = 2,
        Vector64 = 3,
        Vector128 = 4,
    }
}