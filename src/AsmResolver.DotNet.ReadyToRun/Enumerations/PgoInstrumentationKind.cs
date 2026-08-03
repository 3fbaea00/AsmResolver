namespace AsmResolver.DotNet.ReadyToRun.Enumerations
{
    // https://github.com/dotnet/runtime/blob/main/src/coreclr/inc/corjit.h
    public enum PgoInstrumentationKind
    {
        // This must be kept in sync with PgoInstrumentationKind in PgoFormat.cs

        // Schema data types
        None = 0,
        FourByte = 1,
        EightByte = 2,
        TypeHandle = 3,
        MethodHandle = 4,

        // Mask of all schema data types
        MarshalMask = 0xF,

        // ExcessAlignment
        Align4Byte = 0x10,
        Align8Byte = 0x20,
        AlignPointer = 0x30,

        // Mask of all schema alignment types
        AlignMask = 0x30,

        DescriptorMin = 0x40,
        DescriptorMask = ~(MarshalMask | AlignMask),

        Done = None, // All instrumentation schemas must end with a record which is "Done"
        BasicBlockIntCount = (DescriptorMin * 1) | FourByte, // basic block counter using unsigned 4 byte int
        BasicBlockLongCount = (DescriptorMin * 1) | EightByte, // basic block counter using unsigned 8 byte int
        HandleHistogramIntCount = (DescriptorMin * 2) | FourByte | AlignPointer, // 4 byte counter that is part of a type histogram. Aligned to match HandleHistogram32's alignment.
        HandleHistogramLongCount = (DescriptorMin * 2) | EightByte, // 8 byte counter that is part of a type histogram
        HandleHistogramTypes = (DescriptorMin * 3) | TypeHandle, // Histogram of type handles
        HandleHistogramMethods = (DescriptorMin * 3) | MethodHandle, // Histogram of method handles
        Version = (DescriptorMin * 4) | None, // Version is encoded in the Other field of the schema
        NumRuns = (DescriptorMin * 5) | None, // Number of runs is encoded in the Other field of the schema
        EdgeIntCount = (DescriptorMin * 6) | FourByte, // edge counter using unsigned 4 byte int
        EdgeLongCount = (DescriptorMin * 6) | EightByte, // edge counter using unsigned 8 byte int
        GetLikelyClass = (DescriptorMin * 7) | TypeHandle, // Compressed get likely class data
        GetLikelyMethod = (DescriptorMin * 7) | MethodHandle, // Compressed get likely method data

        // Same as type/method histograms, but for generic integer values
        ValueHistogramIntCount = (DescriptorMin * 8) | FourByte | AlignPointer,
        ValueHistogramLongCount = (DescriptorMin * 8) | EightByte,
        ValueHistogram = (DescriptorMin * 9) | EightByte,
    }
}