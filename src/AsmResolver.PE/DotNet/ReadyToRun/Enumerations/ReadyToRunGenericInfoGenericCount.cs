namespace AsmResolver.PE.DotNet.ReadyToRun.Enumerations
{
    // https://github.com/dotnet/runtime/blob/main/src/coreclr/inc/readytorun.h
    public enum ReadyToRunGenericInfoGenericCount : uint
    {
        Zero = 0,
        One = 1,
        Two = 2,
        MoreThanTwo = 3
    }
}