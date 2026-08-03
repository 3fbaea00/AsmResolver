namespace AsmResolver.DotNet.ReadyToRun.Enumerations
{
    // https://github.com/dotnet/runtime/blob/main/src/coreclr/inc/readytorun.h
    public enum DictionaryEntryKind
    {
        EmptySlot = 0,
        TypeHandleSlot = 1,
        MethodDescSlot = 2,
        MethodEntrySlot = 3,
        ConstrainedMethodEntrySlot = 4,
        DispatchStubAddrSlot = 5,
        FieldDescSlot = 6,
        DeclaringTypeHandleSlot = 7,
    }
}