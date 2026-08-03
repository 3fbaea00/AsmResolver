using System;
namespace AsmResolver.DotNet.ReadyToRun.Enumerations
{
    // https://github.com/dotnet/runtime/blob/main/src/coreclr/inc/readytorun.h
    [Flags]
    public enum ReadyToRunAttributes : uint
    {
        PlatformNeutralSource    = 0x00000001, // Set if the original IL assembly was platform-neutral
        SkipTypeValidation       = 0x00000002, // Runtime should trust that the metadata for the types defined in this module is correct
        Partial                  = 0x00000004, // Set of methods with native code was determined using profile data
        NonSharedPInvokeStubs    = 0x00000008, // PInvoke stubs compiled into image are non-shareable (no secret parameter)
        EmbeddedMSIL             = 0x00000010, // MSIL is embedded in the composite R2R executable
        Component                = 0x00000020, // This is the header describing a component assembly of composite R2R
        MultiModuleVersionBubble = 0x00000040, // This R2R module has multiple modules within its version bubble (For versions before version 6.2, all modules are assumed to possibly have this characteristic)
        UnrelatedR2RCode         = 0x00000080, // This R2R module has code in it that would not be naturally encoded into this module
        PlatformNativeImage      = 0x00000100, // The owning composite executable is in the platform native format
        StrippedILBodies         = 0x00000200, // IL method bodies have been stripped from the image
        StrippedInliningInfo     = 0x00000400, // Inlining info has been stripped from the image
        StrippedDebugInfo        = 0x00000800, // Debug info has been stripped from the image
    }
}
