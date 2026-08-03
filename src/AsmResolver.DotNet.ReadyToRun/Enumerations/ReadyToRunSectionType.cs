namespace AsmResolver.DotNet.ReadyToRun.Enumerations
{
    /// <summary>
    /// ReadyToRunSectionType IDs are used by the runtime to look up specific global data sections from each module linked into the final binary.
    /// New sections should be added at the bottom of the enum and deprecated sections should not be removed to preserve ID stability.
    /// </summary>
    public enum ReadyToRunSectionType : uint
    {
        // https://github.com/dotnet/runtime/blob/main/src/coreclr/inc/readytorun.h
        // CoreCLR ReadyToRun sections

        CompilerIdentifier        = 100,
        ImportSections            = 101,
        RuntimeFunctions          = 102,
        MethodDefEntryPoints      = 103,
        ExceptionInfo             = 104,
        DebugInfo                 = 105,
        DelayLoadMethodCallThunks = 106,
        // AvailableTypes         = 107, deprecated
        AvailableTypes            = 108,
        InstanceMethodEntryPoints = 109,
        // InliningInfo           = 110, // Added in v2.1, deprecated in 4.1
        ProfileDataInfo           = 111, // Added in v2.2
        ManifestMetadata          = 112, // Added in v2.3
        AttributePresence         = 113, // Added in V3.1
        InliningInfo2             = 114, // Added in 4.1
        ComponentAssemblies       = 115, // Added in 4.1
        OwnerCompositeExecutable  = 116, // Added in 4.1
        PgoInstrumentationData    = 117, // Added in 5.2
        ManifestAssemblyMvids     = 118, // Added in 5.3
        CrossModuleInlineInfo     = 119, // Added in 6.3
        HotColdMap                = 120, // Added in 8.0
        MethodIsGenericMap        = 121, // Added in V9.0
        EnclosingTypeMap          = 122, // Added in V9.0
        TypeGenericInfoMap        = 123, // Added in V9.0
        ExternalTypeMaps          = 124, // Added to CoreCLR in V18.3
        ProxyTypeMaps             = 125, // Added to CoreCLR in V18.3
        TypeMapAssemblyTargets    = 126, // Added in V18.3

        /*
        // https://github.com/dotnet/runtime/blob/main/src/coreclr/nativeaot/Runtime/inc/ModuleHeaders.h
        // NativeAOT ReadyToRun sections

        // StringTable                  = 200, unused
        GCStaticRegion                  = 201,
        ThreadStaticRegion              = 202,
        // 203 is unused
        TypeManagerIndirection          = 204,
        EagerCctor                      = 205,
        FrozenObjectRegion              = 206,
        // GCStaticDesc                 = 207,
        DehydratedData                  = 207,
        ThreadStaticOffsetRegion        = 208,
        InterfaceDispatchCellInfoRegion = 209,
        InterfaceDispatchCellRegion     = 210,
        // LoopHijackFlag               = 211,
        ImportAddressTables             = 212,
        ModuleInitializerList           = 213,
        GvmDispatchCellInfoRegion       = 214,
        GvmDispatchCellRegion           = 215,

        // Sections 300 - 399 are reserved for RhFindBlob backwards compatibility
        ReadonlyBlobRegionStart = 300,
        ReadonlyBlobRegionEnd   = 399,
        */
    }
}
