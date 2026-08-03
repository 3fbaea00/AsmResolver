namespace AsmResolver.DotNet.ReadyToRun.Enumerations
{
    // https://github.com/dotnet/runtime/blob/main/src/coreclr/inc/readytorun.h
    public enum ReadyToRunImportSectionType : byte
    {
        Unknown = 0,
        StubDispatch = 2,
        StringHandle = 3,
        ILBodyFixups = 7,
    }
}
