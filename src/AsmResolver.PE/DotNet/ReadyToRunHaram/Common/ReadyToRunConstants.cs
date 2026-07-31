using System;

// If any of these constants change, update src/coreclr/inc/readytorun.h and
// src/coreclr/tools/Common/Internal/Runtime/ModuleHeaders.cs with the new R2R minor version

namespace AsmResolver.PE.DotNet.ReadyToRun
{
    public static class ReadyToRunRuntimeConstants
    {
        public const int READYTORUN_PInvokeTransitionFrameSizeInPointerUnits = 11;
        public const int READYTORUN_ReversePInvokeTransitionFrameSizeInPointerUnits_X86 = 5;
        public const int READYTORUN_ReversePInvokeTransitionFrameSizeInPointerUnits_Universal = 2;
    }
}
