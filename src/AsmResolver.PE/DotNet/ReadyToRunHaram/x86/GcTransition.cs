using AsmResolver.PE.DotNet.ReadyToRun.I386;
using System.Collections.Generic;

namespace AsmResolver.PE.DotNet.ReadyToRun.x86
{
    public class CalleeSavedRegister : BaseGcTransition
    {
        public I386.CalleeSavedRegister Register { get; set; }

        public CalleeSavedRegister() { }

        public CalleeSavedRegister(int codeOffset, I386.CalleeSavedRegister reg)
            : base(codeOffset)
        {
            Register = reg;
        }
    }

    public class IPtrMask : BaseGcTransition
    {
        public uint IMask { get; set; }

        public IPtrMask() { }

        public IPtrMask(int codeOffset, uint imask)
            : base(codeOffset)
        {
            IMask = imask;
        }
    }

    public class GcTransitionRegister : BaseGcTransition
    {
        public I386.Register Register { get; set; }
        public GCTransitionAction IsLive { get; set; }
        public int PushCountOrPopSize { get; set; }
        public bool IsThis { get; set; }
        public bool Iptr { get; set; }

        public GcTransitionRegister() { }

        public GcTransitionRegister(int codeOffset, I386.Register reg, GCTransitionAction isLive, bool isThis = false, bool iptr = false, int pushCountOrPopSize = -1)
            : base(codeOffset)
        {
            Register = reg;
            IsLive = isLive;
            PushCountOrPopSize = pushCountOrPopSize;
        }
    }

    public class GcTransitionPointer : BaseGcTransition
    {
        private bool _isEbpFrame;
        public uint ArgOffset { get; set; }
        public uint ArgCount { get; set; }
        public GCTransitionAction Act { get; set; }
        public bool IsPtr { get; set; }
        public bool IsThis { get; set; }
        public bool Iptr { get; set; }

        public GcTransitionPointer() { }

        public GcTransitionPointer(int codeOffset, uint argOffs, uint argCnt, GCTransitionAction act, bool isEbpFrame, bool isThis = false, bool iptr = false, bool isPtr = true)
            : base(codeOffset)
        {
            _isEbpFrame = isEbpFrame;
            CodeOffset = codeOffset;
            ArgOffset = argOffs;
            ArgCount = argCnt;
            Act = act;
            IsPtr = isPtr;
        }
    }

    public class GcTransitionCall : BaseGcTransition
    {
        public struct CallRegister
        {
            public I386.Register Register { get; set; }
            public bool IsByRef { get; set; }

            public CallRegister(I386.Register reg, bool isByRef)
            {
                Register = reg;
                IsByRef = isByRef;
            }
        }

        public struct PtrArg
        {
            public uint StackOffset { get; set; }
            public uint LowBit { get; set; }

            public PtrArg(uint stackOffset, uint lowBit)
            {
                StackOffset = stackOffset;
                LowBit = lowBit;
            }
        }

        public List<CallRegister> CallRegisters { get; set; }
        public List<PtrArg> PtrArgs { get; set; }
        public uint ArgMask { get; set; }
        public uint IArgs { get; set; }

        public GcTransitionCall() { }

        public GcTransitionCall(int codeOffset)
            : base(codeOffset)
        {
            CallRegisters = new List<CallRegister>();
            PtrArgs = new List<PtrArg>();
            ArgMask = 0;
            IArgs = 0;
        }

        public GcTransitionCall(int codeOffset, bool isEbpFrame, uint regMask, uint byRefRegMask)
            : base(codeOffset)
        {
            CallRegisters = new List<CallRegister>();
            PtrArgs = new List<PtrArg>();
            if ((regMask & 1) != 0)
            {
                I386.Register reg = I386.Register.EDI;
                bool isByRef = (byRefRegMask & 1) != 0;
                CallRegisters.Add(new CallRegister(reg, isByRef));
            }
            if ((regMask & 2) != 0)
            {
                I386.Register reg = I386.Register.ESI;
                bool isByRef = (byRefRegMask & 2) != 0;
                CallRegisters.Add(new CallRegister(reg, isByRef));
            }
            if ((regMask & 4) != 0)
            {
                I386.Register reg = I386.Register.EBX;
                bool isByRef = (byRefRegMask & 4) != 0;
                CallRegisters.Add(new CallRegister(reg, isByRef));
            }
            if (!isEbpFrame)
            {
                if ((regMask & 8) != 0)
                {
                    I386.Register reg = I386.Register.EBP;
                    CallRegisters.Add(new CallRegister(reg, false));
                }
            }
            ArgMask = 0;
            IArgs = 0;
        }
    }
}
