namespace AsmResolver.PE.DotNet.ReadyToRun.LoongArch64
{
    public class Epilog
    {
        public int Index { get; set; }

        public uint EpilogStartOffset { get; set; }
        public uint Res { get; set; }
        public uint Condition { get; set; }
        public uint EpilogStartIndex { get; set; }
        public uint EpilogStartOffsetFromMainFunctionBegin { get; set; }

        public Epilog() { }

        public Epilog(int index, int dw, uint startOffset)
        {
            Index = index;

            EpilogStartOffset = UnwindInfo.ExtractBits(dw, 0, 18);
            Res = UnwindInfo.ExtractBits(dw, 18, 4);
            Condition = UnwindInfo.ExtractBits(dw, 20, 4);
            EpilogStartIndex = UnwindInfo.ExtractBits(dw, 22, 10);

            // Note that epilogStartOffset for a funclet is the offset from the beginning
            // of the current funclet, not the offset from the beginning of the main function.
            // To help find it when looking through JitDump output, also show the offset from
            // the beginning of the main function.
            EpilogStartOffsetFromMainFunctionBegin = EpilogStartOffset * 4 + startOffset;
        }
    }

    public class UnwindCode
    {
        public int Index { get; set; }

        public UnwindCode() { }

        public UnwindCode(int index)
        {
            Index = index;

        }
    }

    /// <summary>
    /// based on <a href="https://github.com/dotnet/runtime/src/coreclr/jit/unwindloongarch64.cpp">src/jit/unwindloongarch64.cpp</a> DumpUnwindInfo
    /// </summary>
    public class UnwindInfo : BaseUnwindInfo
    {
        public uint CodeWords { get; set; }
        public uint EpilogCount { get; set; }
        public uint EBit { get; set; }
        public uint XBit { get; set; }
        public uint Vers { get; set; }
        public uint FunctionLength { get; set; }

        public uint ExtendedCodeWords { get; set; }
        public uint ExtendedEpilogCount { get; set; }

        public Epilog[] Epilogs { get; set; }

        public UnwindInfo() { }

        public UnwindInfo(NativeReader imageReader, int offset)
        {
            uint startOffset = (uint)offset;

            int dw = imageReader.ReadInt32(ref offset);
            CodeWords = ExtractBits(dw, 27, 5);
            EpilogCount = ExtractBits(dw, 22, 5);
            EBit = ExtractBits(dw, 21, 1);
            XBit = ExtractBits(dw, 20, 1);
            Vers = ExtractBits(dw, 18, 2);
            FunctionLength = ExtractBits(dw, 0, 18) * 4;

            if (CodeWords == 0 && EpilogCount == 0)
            {
                // We have an extension word specifying a larger number of Code Words or Epilog Counts
                // than can be specified in the header word.
                dw = imageReader.ReadInt32(ref offset);
                ExtendedCodeWords = ExtractBits(dw, 16, 8);
                ExtendedEpilogCount = ExtractBits(dw, 0, 16);
            }

            bool[] epilogStartAt = new bool[1024]; // One byte per possible epilog start index; initialized to false

            if (EBit == 0)
            {
                Epilogs = new Epilog[EpilogCount];
                if (EpilogCount != 0)
                {
                    for (int scope = 0; scope < EpilogCount; scope++)
                    {
                        dw = imageReader.ReadInt32(ref offset);
                        Epilogs[scope] = new Epilog(scope, dw, startOffset);
                        epilogStartAt[Epilogs[scope].EpilogStartIndex] = true; // an epilog starts at this offset in the unwind codes
                    }
                }
            }
            else
            {
                Epilogs = new Epilog[0];
                epilogStartAt[EpilogCount] = true; // the one and only epilog starts its unwind codes at this offset
            }



            Size = offset - (int)startOffset + (int)CodeWords * 4;
            int alignmentPad = ((Size + sizeof(int) - 1) & ~(sizeof(int) - 1)) - Size;
            Size += (alignmentPad + sizeof(uint));
        }

        internal static uint ExtractBits(int dw, int start, int length)
        {
            return (uint)((dw >> start) & ((1 << length) - 1));
        }
    }
}
