using System;
using System.Collections.Generic;

namespace AsmResolver.PE.DotNet.ReadyToRun.Amd64
{
    /// <summary>
    /// based on <a href="https://github.com/dotnet/runtime/blob/main/src/coreclr/inc/win64unwind.h">src\inc\win64unwind.h</a> _UNWIND_CODE
    /// </summary>
    public class UnwindCode
    {
        public byte CodeOffset;
        public UnwindOpCodes UnwindOp; //4 bits

        public byte OpInfo; //4 bits

        public byte OffsetLow;
        public byte OffsetHigh; //4 bits

        public int FrameOffset;
        public int NextFrameOffset;

        public bool IsOpInfo;

        public UnwindCode() { }

        /// <summary>
        /// Unwind code parsing is based on <a href="https://github.com/dotnet/runtime/blob/main/src/coreclr/jit/unwindamd64.cpp">src\jit\unwindamd64.cpp</a> DumpUnwindInfo
        /// </summary>
        public UnwindCode(NativeReader imageReader, ref int frameOffset, ref int offset)
        {
            CodeOffset = imageReader.ReadByte(ref offset);
            byte op = imageReader.ReadByte(ref offset);
            UnwindOp = (UnwindOpCodes)(op & 15);
            OpInfo = (byte)(op >> 4);

            OffsetLow = CodeOffset;
            OffsetHigh = OpInfo;

            FrameOffset = frameOffset;

            switch (UnwindOp)
            {
                case UnwindOpCodes.AllocateLarge:
                    if (OpInfo == 0)
                    {
                        NextFrameOffset = 8 * imageReader.ReadUInt16(ref offset);
                    }
                    else if (OpInfo == 1)
                    {
                        uint nextOffset = imageReader.ReadUInt16(ref offset);
                        NextFrameOffset = (int)((uint)(imageReader.ReadUInt16(ref offset) << 16) | nextOffset);
                    }
                    else
                    {
                        throw new BadImageFormatException();
                    }
                    break;
                case UnwindOpCodes.AllocateSmall:
                    int opInfo = OpInfo * 8 + 8;
                    break;
                case UnwindOpCodes.SetFramePointerRegisterLarge:
                {
                    uint nextOffset = imageReader.ReadUInt16(ref offset);
                    nextOffset = ((uint)(imageReader.ReadUInt16(ref offset) << 16) | nextOffset);
                    NextFrameOffset = (int)nextOffset * 16;
                    if ((NextFrameOffset & 0xF0000000) != 0)
                    {
                        throw new BadImageFormatException("Warning: Illegal unwindInfo unscaled offset: too large");
                    }
                }
                break;
                case UnwindOpCodes.SaveNonVolatile:
                {
                    NextFrameOffset = imageReader.ReadUInt16(ref offset) * 8;
                }
                break;
                case UnwindOpCodes.SaveNonVolatileFar:
                {
                    uint nextOffset = imageReader.ReadUInt16(ref offset);
                    NextFrameOffset = (int)((uint)(imageReader.ReadUInt16(ref offset) << 16) | nextOffset);
                }
                break;
                case UnwindOpCodes.SaveXmm128:
                {
                    NextFrameOffset = (int)imageReader.ReadUInt16(ref offset) * 16;
                }
                break;
                case UnwindOpCodes.SaveXmm128Far:
                {
                    uint nextOffset = imageReader.ReadUInt16(ref offset);
                    NextFrameOffset = (int)((uint)(imageReader.ReadUInt16(ref offset) << 16) | nextOffset);
                }
                break;
            }

            NextFrameOffset = frameOffset;
        }
    }

    /// <summary>
    /// based on <a href="https://github.com/dotnet/runtime/blob/main/src/coreclr/inc/win64unwind.h">src\inc\win64unwind.h</a> _UNWIND_INFO
    /// </summary>
    public class UnwindInfo : BaseUnwindInfo
    {
        private const int _sizeofUnwindCode = 2;
        private const int _offsetofUnwindCode = 4;

        public byte Version; //3 bits
        public byte Flags; //5 bits
        public byte SizeOfProlog;
        public byte CountOfUnwindCodes;
        public Register FrameRegister; //4 bits
        public byte FrameOffset; //4 bits
        public Dictionary<int, int> CodeOffsetToUnwindCodeIndex;
        public List<UnwindCode> UnwindCodes;
        public uint PersonalityRoutineRVA;

        public UnwindInfo() { }

        /// <summary>
        /// based on <a href="https://github.com/dotnet/coreclr/blob/master/src/zap/zapcode.cpp">ZapUnwindData::Save</a>
        /// </summary>
        public UnwindInfo(NativeReader imageReader, int offset)
        {
            byte versionAndFlags = imageReader.ReadByte(ref offset);
            Version = (byte)(versionAndFlags & 7);
            Flags = (byte)(versionAndFlags >> 3);
            SizeOfProlog = imageReader.ReadByte(ref offset);
            CountOfUnwindCodes = imageReader.ReadByte(ref offset);
            byte frameRegisterAndOffset = imageReader.ReadByte(ref offset);
            FrameRegister = (Register)(frameRegisterAndOffset & 15);
            FrameOffset = (byte)(frameRegisterAndOffset >> 4);

            UnwindCodes = new List<UnwindCode>(CountOfUnwindCodes);
            CodeOffsetToUnwindCodeIndex = new Dictionary<int, int>();
            int frameOffset = FrameOffset;
            int sizeOfUnwindCodes = CountOfUnwindCodes * _sizeofUnwindCode;
            int endOffset = offset + sizeOfUnwindCodes;
            while (offset < endOffset)
            {
                UnwindCode unwindCode = new UnwindCode(imageReader, ref frameOffset, ref offset);
                CodeOffsetToUnwindCodeIndex.Add(unwindCode.CodeOffset, UnwindCodes.Count);
                UnwindCodes.Add(unwindCode);
            }

            Size = _offsetofUnwindCode + sizeOfUnwindCodes;
            int alignmentPad = -Size & 3;
            Size += alignmentPad + sizeof(uint);

            // Personality routine RVA must be at 4-aligned address
            offset += alignmentPad;
            PersonalityRoutineRVA = imageReader.ReadUInt32(ref offset);
        }
    }
}
