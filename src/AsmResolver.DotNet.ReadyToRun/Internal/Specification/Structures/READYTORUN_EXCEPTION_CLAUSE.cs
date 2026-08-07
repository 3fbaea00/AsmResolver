using AsmResolver.DotNet.ReadyToRun.Enumerations;
using AsmResolver.PE.DotNet.Metadata.Tables;
using System.Runtime.InteropServices;

namespace AsmResolver.DotNet.ReadyToRun.Internal.Structures
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct READYTORUN_EXCEPTION_CLAUSE
    {
        [FieldOffset(0x00)] CorILExceptionClause Flags;
        [FieldOffset(0x04)] uint TryStartPC;
        [FieldOffset(0x08)] uint TryEndPC;
        [FieldOffset(0x0C)] uint HandlerStartPC;
        [FieldOffset(0x10)] uint HandlerEndPC;

        [FieldOffset(0x14)] MetadataToken ClassToken;
        [FieldOffset(0x14)] uint FilterOffset;
    }
}
