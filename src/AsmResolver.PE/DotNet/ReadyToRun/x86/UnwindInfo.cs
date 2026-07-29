namespace AsmResolver.PE.DotNet.ReadyToRun.x86
{
    /// <summary>
    /// based on <a href="https://github.com/dotnet/runtime/blob/main/src/coreclr/inc/win64unwind.h">src\inc\win64unwind.h</a> _UNWIND_INFO
    /// </summary>
    public class UnwindInfo : BaseUnwindInfo
    {
        public uint FunctionLength { get; set; }

        public UnwindInfo() { }

        public UnwindInfo(NativeReader imageReader, int offset)
        {
            int startOffset = offset;
            FunctionLength = imageReader.DecodeUnsignedGc(ref offset);
            Size = offset - startOffset;
        }
    }
}
