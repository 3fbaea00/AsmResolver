namespace AsmResolver.DotNet.ReadyToRun.Enumerations
{
    // https://github.com/dotnet/runtime/blob/main/src/coreclr/inc/readytorun.h
    public enum ReadyToRunFixupKind
    {
        Invalid = 0x00,

        ThisObjDictionaryLookup = 0x07,
        TypeDictionaryLookup = 0x08,
        MethodDictionaryLookup = 0x09,

        TypeHandle = 0x10,
        MethodHandle = 0x11,
        FieldHandle = 0x12,

        MethodEntry = 0x13,                 // For calling a method entry point
        MethodEntry_DefToken = 0x14,        // Smaller version of MethodEntry - method is def token
        MethodEntry_RefToken = 0x15,        // Smaller version of MethodEntry - method is ref token

        VirtualEntry = 0x16,                // For invoking a virtual method
        VirtualEntry_DefToken = 0x17,       // Smaller version of VirtualEntry - method is def token
        VirtualEntry_RefToken = 0x18,       // Smaller version of VirtualEntry - method is ref token
        VirtualEntry_Slot = 0x19,           // Smaller version of VirtualEntry - type & slot - OBSOLETE, not currently used, and hasn't ever been used in R2R codegen since crossgen2 was introduced, and may not have ever been used.

        Helper = 0x1A,                      // Helper
        StringHandle = 0x1B,                // String handle

        NewObject = 0x1C,                   // Dynamically created new helper
        NewArray = 0x1D,

        IsInstanceOf = 0x1E,                // Dynamically created casting helper
        ChkCast = 0x1F,

        FieldAddress = 0x20,                // For accessing a cross-module static fields
        CctorTrigger = 0x21,                // Static constructor trigger

        StaticBaseNonGC = 0x22,             // Dynamically created static base helpers
        StaticBaseGC = 0x23,
        ThreadStaticBaseNonGC = 0x24,
        ThreadStaticBaseGC = 0x25,

        FieldBaseOffset = 0x26,             // Field base offset
        FieldOffset = 0x27,                 // Field offset

        TypeDictionary = 0x28,
        MethodDictionary = 0x29,

        Check_TypeLayout = 0x2A,            // size, alignment, HFA, reference map
        Check_FieldOffset = 0x2B,

        DelegateCtor = 0x2C,                // optimized delegate ctor
        DeclaringTypeHandle = 0x2D,

        IndirectPInvokeTarget = 0x2E,       // Target (indirect) of an inlined pinvoke
        PInvokeTarget = 0x2F,               // Target of an inlined pinvoke

        Check_InstructionSetSupport = 0x30, // Define the set of instruction sets that must be supported/unsupported to use the fixup

        Verify_FieldOffset = 0x31,  // Generate a runtime check to ensure that the field offset matches between compile and runtime. Unlike CheckFieldOffset, this will generate a runtime exception on failure instead of silently dropping the method
        Verify_TypeLayout = 0x32,  // Generate a runtime check to ensure that the type layout (size, alignment, HFA, reference map) matches between compile and runtime. Unlike Check_TypeLayout, this will generate a runtime failure instead of silently dropping the method

        Check_VirtualFunctionOverride = 0x33, // Generate a runtime check to ensure that virtual function resolution has equivalent behavior at runtime as at compile time. If not equivalent, code will not be used
        Verify_VirtualFunctionOverride = 0x34, // Generate a runtime check to ensure that virtual function resolution has equivalent behavior at runtime as at compile time. If not equivalent, generate runtime failure.

        Check_IL_Body = 0x35, /* Check to see if an IL method is defined the same at runtime as at compile time. A failed match will cause code not to be used. */
        Verify_IL_Body = 0x36, /* Verify an IL body is defined the same at compile time and runtime. A failed match will cause a hard runtime failure. */

        ContinuationLayout = 0x37, /* Layout of an async method continuation type */
        ResumptionStubEntryPoint = 0x38, /* Entry point of an async method resumption stub */

        InjectStringThunks = 0x39, /* Inject pregenerated string-to-code thunk mappings into the global lookup table */

        ModuleOverride = 0x80,
        // followed by sig-encoded UInt with assemblyref index into either the assemblyref
        // table of the MSIL metadata of the master context module for the signature or
        // into the extra assemblyref table in the manifest metadata R2R header table
        // (used in cases inlining brings in references to assemblies not seen in the MSIL).
    }
}